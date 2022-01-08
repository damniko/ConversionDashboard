using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using DesktopUI.Controllers;
using DesktopUI.Helpers;
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
    public List<ManagerDto> Managers { get; } = new();
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

        var last = SelectedExecution.ContextDict.Last();

        if (Managers.Any(x => x.Name.Split(',').First().ToUpper() == last.Value))
        {
            return last.Key;
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
        await _semaphore.WaitAsync();
        try
        {
            await UpdateExecutions();
            await UpdateManagers();
        }
        finally
        {
            _lastUpdated = date;
            _semaphore.Release();
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
        var newManagers = await _controller.GetSince(_lastUpdated);
        if (newManagers.Any() is false) return;

        _viewSource.Dispatcher.Invoke(() =>
        {
            using (View.DeferRefresh())
            {
                ManagerHelper.MergeProperties(newManagers, Managers);
            }
            View.Refresh();
        });
        OnPropertyChanged(nameof(CurrentCount));
    }

    private CollectionViewSource ConfigureViewSource()
    {
        var viewSource = new CollectionViewSource
        {
            Source = Managers,
            SortDescriptions =
            {
                new(nameof(ManagerDto.StartTime), System.ComponentModel.ListSortDirection.Ascending)
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