using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DesktopUI.Controllers;
using DesktopUI.Library;
using DesktopUI.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace DesktopUI.ViewModels;

public class ManagerViewModel : ObservableObject
{
    #region Fields
    private readonly ManagerController _controller;
    private readonly ExecutionController _executionController;
    private readonly CollectionViewSource _viewSource;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
    private string _searchTerm = string.Empty;
    private ExecutionDto? _selectedExecution;
    private CancellationTokenSource _cts = new();
    private string _currentStatus = string.Empty;
    private bool _isUpdating;
    private ManagerDto? _selectedManager;
    private bool _showNameColumn = true;
    private bool _showStartTimeColumn = true;
    private bool _showEndTimeColumn = true;
    private bool _showRuntimeColumn = true;
    private bool _showReadColumn = true;
    private bool _showWrittenColumn = true;
    #endregion

    public ManagerViewModel(QueryTimerService timerService,
                            ManagerController controller,
                            ExecutionController executionController)
    {
        _controller = controller;
        _executionController = executionController;
        _viewSource = ConfigureViewSource();
        ComparisonVM = new(Managers);
        timerService.ManagerTimer.Elapsed += d => Task.Run(() => UpdateData(d));
    }

    public ManagerComparisonViewModel ComparisonVM { get; }

    #region Properties
    // Manager View
    public ObservableCollection<ManagerDto> Managers { get; } = new();
    public ICollectionView View => _viewSource.View;
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            SetProperty(ref _searchTerm, value);
            View.Refresh();
        }
    }
    public long? ExpectedCount => SelectedExecution?.ContextDict.Keys.Last();
    public long? CurrentCount => GetCurrentCount();
    public ManagerDto? SelectedManager { get => _selectedManager; set => SetProperty(ref _selectedManager, value); }
    public bool ShowNameColumn { get => _showNameColumn; set => SetProperty(ref _showNameColumn, value); }
    public bool ShowStartTimeColumn { get => _showStartTimeColumn; set => SetProperty(ref _showStartTimeColumn, value); }
    public bool ShowEndTimeColumn { get => _showEndTimeColumn; set => SetProperty(ref _showEndTimeColumn, value); }
    public bool ShowRuntimeColumn { get => _showRuntimeColumn; set => SetProperty(ref _showRuntimeColumn, value); }
    public bool ShowReadColumn { get => _showReadColumn; set => SetProperty(ref _showReadColumn, value); }
    public bool ShowWrittenColumn { get => _showWrittenColumn; set => SetProperty(ref _showWrittenColumn, value); }
    // Executions
    public ObservableCollection<ExecutionDto> Executions { get; } = new();
    public ExecutionDto? SelectedExecution
    {
        get => _selectedExecution;
        set
        {
            if (SetProperty(ref _selectedExecution, value))
            {
                OnPropertyChanged(nameof(ClearSelectedExecutionCmd));
                OnPropertyChanged(nameof(ExpectedCount));
                OnPropertyChanged(nameof(CurrentCount));
                SelectedManager = null;
                View.Refresh();
            }
        }
    }
    // Progress
    public string CurrentStatus { get => _currentStatus; set => SetProperty(ref _currentStatus, value); }
    public bool IsUpdating
    {
        get => _isUpdating; 
        set
        {
            SetProperty(ref _isUpdating, value);
            OnPropertyChanged(nameof(StopUpdateCmd));
            OnPropertyChanged(nameof(UpdateDataCmd));
        }
    }
    #endregion

    #region Commands
    /// <summary>
    /// Gets any new data and triggers an update for the view.
    /// </summary>
    public ICommand UpdateDataCmd
        => new RelayCommand<DateTime?>(d => ClearAndUpdateData(DateTime.Now), d => !IsUpdating);
    public ICommand StopUpdateCmd
        => new RelayCommand(() => _cts.Cancel(), () => IsUpdating);
    public ICommand SelectManagerCmd => new RelayCommand<ManagerDto>(m => SelectedManager = m);
    /// <summary>
    /// Clears the value of the current <see cref="SelectedExecution"/>.
    /// </summary>
    public ICommand ClearSelectedExecutionCmd
        => new RelayCommand(() => SelectedExecution = null, () => SelectedExecution != null);
    #endregion

    private async Task UpdateData(DateTime date)
    {
        bool canExecute = await _semaphore.WaitAsync(0);
        if (canExecute)
        {
            try
            {
                _cts = new CancellationTokenSource();
                Progress<ProgressReportModel> progress = new();
                progress.ProgressChanged += ReportProgress;
                IsUpdating = true;
                try
                {
                    await UpdateExecutions(progress, _cts.Token);
                    await UpdateManagers(progress, _cts.Token);
                    _lastUpdated = date;
                }
                catch (OperationCanceledException)
                {
                    CurrentStatus = "Update was cancelled.";
                }
                IsUpdating = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    private void ClearAndUpdateData(DateTime date)
    {
        _cts.Cancel();
        Executions.Clear();
        Managers.Clear();
        SelectedExecution = null;
        SelectedManager = null;
        _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        Task.Run(() => UpdateData(date));
    }

    private async Task UpdateExecutions(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
    {
        var report = new ProgressReportModel { Progress = $"Getting executions..." };
        progress.Report(report);

        var newExecs = await _executionController.GetSinceAsync(_lastUpdated);

        cancellationToken.ThrowIfCancellationRequested();

        report.Progress = $"Found {newExecs.Count} executions";
        progress.Report(report);

        App.Current.Dispatcher.Invoke(() =>
        {
            newExecs.ForEach(x => Executions.Add(x));
        });
    }

    private async Task UpdateManagers(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
    {
        var report = new ProgressReportModel { Progress = $"Getting managers..." };
        progress.Report(report);

        var managers = await _controller.GetSince(_lastUpdated);
        
        cancellationToken.ThrowIfCancellationRequested();
        report.Progress = $"Found {managers.Count} managers";
        progress.Report(report);
        
        if (managers.Any() is false) return;

        _viewSource.Dispatcher.Invoke(() =>
        {
            foreach (var entry in managers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (Managers.FirstOrDefault(x => x.Name == entry.Name && x.StartTime == entry.StartTime) is { } manager)
                {
                    UpdateManagerProperties(entry, ref manager);
                }
                else
                {
                    Managers.Add(entry);
                }
            }
        });
        OnPropertyChanged(nameof(CurrentCount));
    }

    private void UpdateManagerProperties(ManagerDto input, ref ManagerDto output)
    {
        // TODO - consider moving this into a helper class (if more methods can be grouped into it)
        output.StartTime ??= input.StartTime;
        output.EndTime ??= input.EndTime;
        output.RowsRead ??= input.RowsRead;
        output.RowsWritten ??= input.RowsWritten;
        output.Runtime ??= input.Runtime;
        foreach (KeyValuePair<string, int> pair in output.RowsReadDict)
        {
            output.RowsReadDict.TryAdd(pair.Key, pair.Value);
        }
        foreach (KeyValuePair<string, int> pair in output.RowsWrittenDict)
        {
            output.RowsWrittenDict.TryAdd(pair.Key, pair.Value);
        }
        foreach (KeyValuePair<string, int> pair in output.TimeDict)
        {
            output.TimeDict.TryAdd(pair.Key, pair.Value);
        }
        foreach (KeyValuePair<string, int> pair in output.SqlCostDict)
        {
            output.SqlCostDict.TryAdd(pair.Key, pair.Value);
        }
    }

    private CollectionViewSource ConfigureViewSource()
    {
        var viewSource = new CollectionViewSource
        {
            Source = Managers,
            SortDescriptions =
            {
                new SortDescription(nameof(ManagerDto.StartTime), ListSortDirection.Ascending)
            }
        };
        viewSource.Filter += Managers_Filter;
        return viewSource;
    }

    private long? GetCurrentCount()
    {
        if (SelectedExecution is null) return null;

        (long key, string? value) = SelectedExecution.ContextDict.Last();

        if (Managers.Any(x => x.Name.Split(',').First().ToUpper() == value))
        {
            return key;
        }

        var fixedName = Managers.LastOrDefault()?.Name.Split(',').First().ToUpper();
        var contextId = SelectedExecution?.ContextDict.FirstOrDefault(x => x.Value == fixedName).Key;
        return contextId;
    }

    private void Managers_Filter(object sender, FilterEventArgs e)
    {
        ManagerDto item = (ManagerDto)e.Item;
        e.Accepted = IsInExecution(item) && item.Name.Contains(SearchTerm);
    }

    private void ReportProgress(object? sender, ProgressReportModel e)
    {
        CurrentStatus = e.Progress;
    }

    private bool IsInExecution(ManagerDto item)
    {
        if (SelectedExecution is null)
        {
            return true;
        }
        else
        {
            return SelectedExecution.EndTime.HasValue
                ? item.StartTime >= SelectedExecution.StartTime && item.EndTime <= SelectedExecution.EndTime.Value
                : item.StartTime >= SelectedExecution.StartTime;
        }
    }
}