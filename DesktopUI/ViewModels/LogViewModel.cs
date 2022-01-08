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

/// <summary>
/// Handles all logic for the <see cref="Views.LogView"/>.
/// </summary>
public class LogViewModel : ObservableObject
{
    private readonly CollectionViewSource _viewSource;
    private readonly LogController _controller;
    private readonly ExecutionController _executionController;
    private readonly List<LogEntryDto> _entries = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
    private bool _showInfo = true;
    private bool _showWarning = true;
    private bool _showError = true;
    private bool _showFatal = true;
    private bool _showReconciliation = true;
    private bool _autoScroll = true;
    private ExecutionDto? _selectedExecution;
    private string _searchTerm = string.Empty;
    private long? _shownContextId;

    public LogViewModel(QueryTimerService queryTimerService,
        LogController controller,
        ExecutionController executionController)
    {
        _controller = controller;
        _executionController = executionController;
        _viewSource = ConfigureViewSource();
        queryTimerService.LogTimer.Elapsed += (d) => UpdateDataCmd.Execute(d);
    }

    #region Properties
    public ICollectionView View => _viewSource.View;
    public ObservableCollection<ExecutionDto> Executions { get; } = new();
    public ExecutionDto? SelectedExecution
    {
        get => _selectedExecution;
        set
        {
            SetProperty(ref _selectedExecution, value);
            UpdateContextIdFilter();
            OnPropertyChanged(nameof(ClearSelectedExecutionCmd));
            View.Refresh();
        }
    }
    public List<ContextIdContainer> ContextIdFilters { get; } = new();
    public bool ShowAllContextIds => ContextIdFilters.TrueForAll(x => x.IsChecked is false);
    public bool AutoScroll
    {
        get => _autoScroll;
        set => SetProperty(ref _autoScroll, value);
    }
    public long? ShownContextId
    {
        get => _shownContextId;
        set
        {
            SetProperty(ref _shownContextId, value);
            View.Refresh();
        }
    }
    public string SearchTerm
    {
        get => _searchTerm;
        set
        {
            SetProperty(ref _searchTerm, value);
            View.Refresh();
        }
    }
    public bool ShowInfo
    {
        get => _showInfo;
        set
        {
            SetProperty(ref _showInfo, value);
            View.Refresh();
        }
    }
    public bool ShowWarning
    {
        get => _showWarning;
        set
        {
            SetProperty(ref _showWarning, value);
            View.Refresh();
        }
    }
    public bool ShowError 
    {
        get => _showError;
        set
        {
            SetProperty(ref _showError, value);
            View.Refresh();
        }
    }
    public bool ShowFatal
    {
        get => _showFatal;
        set
        {
            SetProperty(ref _showFatal, value);
            View.Refresh();
        }
    }
    public bool ShowReconciliation
    {
        get => _showReconciliation;
        set
        {
            SetProperty(ref _showReconciliation, value);
            View.Refresh();
        }
    }
    public int InfoCount
        => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Info));
    public int WarnCount
        => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Warn));
    public int ErrorCount
        => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Error));
    public int FatalCount
        => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Fatal));
    public int ReconcCount
        => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Reconciliation));
    public int TotalCount => _entries.Count(x => IsInExecution(x));
    #endregion

    /// <summary>
    /// Gets any new data and triggers an update for the view.
    /// </summary>
    public ICommand UpdateDataCmd => new RelayCommand(() => Task.Run(() => UpdateData(DateTime.Now)));
    /// <summary>
    /// Clears the value of the current <see cref="SelectedExecution"/>.
    /// </summary>
    public ICommand ClearSelectedExecutionCmd
        => new RelayCommand(() => SelectedExecution = null, () => SelectedExecution != null);
    /// <summary>
    /// Sets <see cref="ContextIdContainer.IsChecked"/> to true for the filter associated with the specified entry.
    /// </summary>
    public ICommand AddContextIdCmd => new RelayCommand<LogEntryDto>(e =>
    {
        if (ContextIdFilters.FirstOrDefault(x => x.ContextId == e?.ContextId) is { } x)
        {
            x.IsChecked = true;
        }
    });
    /// <summary>
    /// Sets <see cref="ContextIdContainer.IsChecked"/> to false for all containers in <see cref="ContextIdFilters"/>.
    /// </summary>
    public ICommand ClearContextIdCmd => new RelayCommand(() => ContextIdFilters.ForEach(x => x.IsChecked = false));
    /// <summary>
    /// Enables auto-scrolling to the last element whenever a new entry is added.
    /// </summary>
    public ICommand EnableAutoScrollCommand
        => new RelayCommand(() => AutoScroll = true, () => AutoScroll == false);
    /// <summary>
    /// Disables auto-scrolling to the last element whenever a new entry is added.
    /// </summary>
    public ICommand DisableAutoScrollCommand 
        => new RelayCommand(() => AutoScroll = false, () => AutoScroll == true);

    /// <summary>
    /// Ensures that any data newer than <see cref="_lastUpdated"/> is fetched through the <see cref="LogController"/> and <see cref="ExecutionController"/>, and updates the view.
    /// </summary>
    /// <remarks>Since calls to this function may overlap, a lock is used to avoid duplicating data.</remarks>
    /// <param name="date">The date at which the update was requested.</param>
    private async Task UpdateData(DateTime date)
    {
        _semaphore.Wait();
        try
        {
            await UpdateExecutions();
            await UpdateEntries();
        }
        finally
        {
            _lastUpdated = date;
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Gets executions newer than <see cref="_lastUpdated"/> from the <see cref="ExecutionController"/>, and adds them to the view.
    /// </summary>
    private async Task UpdateExecutions()
    {
        var newExecs = await _executionController.GetSinceAsync(_lastUpdated);
        App.Current.Dispatcher.Invoke(() =>
        {
            foreach(var exec in newExecs)
            {
                Executions.Add(exec);
                // Assign 'Manager' values to entries by using the execution's Context ID dictionary.
                foreach (var entry in _entries.Where(x => x.ExecutionId == exec.Id))
                {
                    if (exec.ContextDict.ContainsKey(entry.ContextId))
                    {
                        entry.Manager = exec.ContextDict[entry.ContextId];
                    }
                }
            }
        });
    }

    /// <summary>
    /// Gets log entries newer than <see cref="_lastUpdated"/> from the <see cref="LogController"/>, and adds them to the view.
    /// </summary>
    private async Task UpdateEntries()
    {
        var newEntries = await _controller.GetSinceAsync(_lastUpdated);

        if (newEntries.Any() is false) return;

        App.Current.Dispatcher.Invoke(() =>
        {
            using (View.DeferRefresh())
            {
                _entries.AddRange(newEntries);
            }
            View.Refresh();
        });
    }

    /// <summary>
    /// Configures a <see cref="CollectionViewSource"/> for <see cref="_entries"/> with filtering.
    /// </summary>
    /// <returns>A fully-configured <see cref="CollectionViewSource"/>.</returns>
    private CollectionViewSource ConfigureViewSource()
    {
        var viewSource = new CollectionViewSource
        {
            Source = _entries,
        };
        viewSource.Filter += Entries_Filter;
        viewSource.View.CollectionChanged += RefreshCounters;
        return viewSource;
    }

    /// <summary>
    /// Populates <see cref="ContextIdFilters"/> with data from the <see cref="SelectedExecution"/>, if any.
    /// </summary>
    private void UpdateContextIdFilter()
    {
        ContextIdFilters.Clear();
        if (SelectedExecution != null)
        {
            foreach (var contextId in SelectedExecution.ContextDict.Keys)
            {
                var container = new ContextIdContainer { ContextId = contextId };
                container.PropertyChanged += (_, _) => View.Refresh();
                ContextIdFilters.Add(container);
            }
        }
        OnPropertyChanged(nameof(ContextIdFilters));
    }

    /// <summary>
    /// Invokes the PropertyChanged events for count properties, which updates the view.
    /// It is called whenever the <see cref="View"/> is changed (i.e., when filtering or adding data).
    /// </summary>
    private void RefreshCounters(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(InfoCount));
        OnPropertyChanged(nameof(WarnCount));
        OnPropertyChanged(nameof(ErrorCount));
        OnPropertyChanged(nameof(FatalCount));
        OnPropertyChanged(nameof(ReconcCount));
    }

    /// <summary>
    /// Filter for the <see cref="View"/>.
    /// </summary>
    private void Entries_Filter(object sender, FilterEventArgs e)
    {
        LogEntryDto item = (LogEntryDto)e.Item;
        LogLevel level = item.Level;
        bool acceptContextId = ShowAllContextIds || ContextIdFilters.Any(x => x.IsChecked && x.ContextId == item.ContextId);
        e.Accepted = (IsInExecution(item) && acceptContextId && item.Message.Contains(SearchTerm))
                     && ((ShowInfo && level.HasFlag(LogLevel.Info))
                         || (ShowWarning && level.HasFlag(LogLevel.Warn))
                         || (ShowError && level.HasFlag(LogLevel.Error))
                         || (ShowFatal && level.HasFlag(LogLevel.Fatal))
                         || (ShowReconciliation && level.HasFlag(LogLevel.Reconciliation)));
    }

    /// <summary>
    /// Determines whether the specified <paramref name="item"/> is in the current <see cref="SelectedExecution"/>.
    /// </summary>
    /// <param name="item">The entry to check.</param>
    /// <returns>True if the entry is in <see cref="SelectedExecution"/>, and false otherwise.</returns>
    private bool IsInExecution(LogEntryDto item)
    {
        return SelectedExecution is null || item.ExecutionId == SelectedExecution.Id;
    }
}