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
    private readonly ManagerController _controller;
    private readonly ExecutionController _executionController;
    private readonly CollectionViewSource _viewSource;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
    private string _searchTerm = string.Empty;
    private ExecutionDto? _selectedExecution;

    public ManagerViewModel(QueryTimerService timerService,
        ManagerController controller,
        ExecutionController executionController)
    {
        _controller = controller;
        _executionController = executionController;
        _viewSource = ConfigureViewSource();
        timerService.ManagerTimer.Elapsed += (d) => UpdateDataCmd.Execute(d);
    }

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

    // Executions
    public ObservableCollection<ExecutionDto> Executions { get; } = new();
    public ExecutionDto? SelectedExecution
    {
        get => _selectedExecution;
        set
        {
            SetProperty(ref _selectedExecution, value);
            OnPropertyChanged(nameof(ClearSelectedExecutionCmd));
            OnPropertyChanged(nameof(ExpectedCount));
            OnPropertyChanged(nameof(CurrentCount));
            View.Refresh();
        }
    }

    /// <summary>
    /// Gets any new data and triggers an update for the view.
    /// </summary>
    public ICommand UpdateDataCmd 
        => new RelayCommand<DateTime?>(d => Task.Run(() => UpdateData(d ?? DateTime.Now)));
    /// <summary>
    /// Clears the value of the current <see cref="SelectedExecution"/>.
    /// </summary>
    public ICommand ClearSelectedExecutionCmd
        => new RelayCommand(() => SelectedExecution = null, () => SelectedExecution != null);

    private async Task UpdateData(DateTime date)
    {
        bool canExecute = await _semaphore.WaitAsync(0);
        if (canExecute)
        {
            try
            {
                await UpdateExecutions();
                await UpdateManagers();
                _lastUpdated = date;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    private async Task UpdateExecutions()
    {
        var newExecs = await _executionController.GetSinceAsync(_lastUpdated);
        App.Current.Dispatcher.Invoke(() =>
        {
            newExecs.ForEach(x => Executions.Add(x));
        });
    }

    private async Task UpdateManagers()
    {
        var managers = await _controller.GetSince(_lastUpdated);
        if (managers.Any() is false) return;

        _viewSource.Dispatcher.Invoke(() =>
        {
            foreach (var entry in managers)
            {
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

    private void Managers_Filter(object sender, FilterEventArgs e)
    {
        ManagerDto item = (ManagerDto)e.Item;
        e.Accepted = IsInExecution(item) && item.Name.Contains(SearchTerm);
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