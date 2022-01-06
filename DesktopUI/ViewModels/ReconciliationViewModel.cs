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
using DesktopUI.Helpers;
using DesktopUI.Library;
using DesktopUI.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace DesktopUI.ViewModels
{
    /// <summary>
    /// Handles all logic for the <see cref="Views.ReconciliationView"/>.
    /// </summary>
    public class ReconciliationViewModel : ObservableObject
    {
        private readonly CollectionViewSource _viewSource;
        private readonly ReconciliationController _controller;
        private readonly ExecutionController _executionController;
        private readonly ExecutionAssociationHelper _associationHelper;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        private bool _showEmpty = false;
        private bool _showOk = false;
        private bool _showDisabled = true;
        private bool _showFailed = true;
        private bool _showFailMismatch = true;
        private string _searchTerm = string.Empty;
        private ExecutionDto? _selectedExecution;

        public ReconciliationViewModel(QueryTimerService queryTimerService,
                                       ReconciliationController reconciliationController,
                                       ExecutionController executionController,
                                       ExecutionAssociationHelper associationHelper)
        {
            _controller = reconciliationController;
            _executionController = executionController;
            _associationHelper = associationHelper;
            _viewSource = ConfigureViewSource();
            queryTimerService.ReconciliationTimer.Elapsed += (d) => UpdateDataCmd.Execute(d);
        }

        #region Properties
        public List<ReconciliationGrouping> Groupings { get; } = new();
        public ICollectionView View => _viewSource.View;
        public ObservableCollection<ExecutionDto> Executions { get; } = new();
        public ExecutionDto? SelectedExecution 
        {
            get => _selectedExecution;
            set
            {
                SetProperty(ref _selectedExecution, value);
                OnPropertyChanged(nameof(ClearSelectedExecutionCmd));
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
        public bool ShowEmpty
        {
            get => _showEmpty;
            set
            {
                SetProperty(ref _showEmpty, value);
                View.Refresh();
            }
        }
        public bool ShowOk
        {
            get => _showOk;
            set
            {
                SetProperty(ref _showOk, value);
                View.Refresh();
            }
        }
        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                SetProperty(ref _showDisabled, value);
                View.Refresh();
            }
        }
        public bool ShowFailed
        {
            get => _showFailed;
            set
            {
                SetProperty(ref _showFailed, value);
                View.Refresh();
            }
        }
        public bool ShowFailMismatch
        {
            get => _showFailMismatch;
            set
            {
                SetProperty(ref _showFailMismatch, value);
                View.Refresh();
            }
        }
        // Counters for reconciliations in the currently filtered view (per execution).
        public int TotalCount => View.Cast<ReconciliationGrouping>().Sum(x => x.TotalCount);
        public int OkCount => View.Cast<ReconciliationGrouping>().Sum(x => x.OkCount);
        public int DisabledCount => View.Cast<ReconciliationGrouping>().Sum(x => x.DisabledCount);
        public int FailedCount => View.Cast<ReconciliationGrouping>().Sum(x => x.FailedCount);
        public int FailMismatchCount => View.Cast<ReconciliationGrouping>().Sum(x => x.FailMismatchCount);
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
        /// Ensures that any data newer than <see cref="_lastUpdated"/> is fetched through the <see cref="ReconciliationController"/> and <see cref="ExecutionController"/>, and updates the view.
        /// </summary>
        /// <remarks>Since calls to this function may overlap, a lock is used to avoid duplicating data.</remarks>
        /// <param name="date">The date at which the update was requested.</param>
        private async Task UpdateData(DateTime date)
        {
            _semaphore.Wait();
            try
            {
                await UpdateExecutions();
                await UpdateReconciliations();
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
                newExecs.ForEach(x => Executions.Add(x));
            });
        }

        /// <summary>
        /// Gets reconciliations (grouped by their associated manager) newer than <see cref="_lastUpdated"/> from the <see cref="ReconciliationController"/>, and adds them to the view.
        /// </summary>
        private async Task UpdateReconciliations()
        {
            var newGroupsDict = await _controller.GetManagerReconciliationDict(_lastUpdated);

            _viewSource.Dispatcher.Invoke(() =>
            {
                foreach (var key in newGroupsDict.Keys)
                {
                    if (Groupings.FirstOrDefault(x => x.GroupName == key) is not ReconciliationGrouping grouping)
                    {
                        grouping = new(Reconciliations_Filter, key);
                        Groupings.Add(grouping);
                    }
                    grouping.AddReconciliations(newGroupsDict[key]);
                }
                View.Refresh();
            });
        }

        /// <summary>
        /// Configures a <see cref="CollectionViewSource"/> for <see cref="Groupings"/> with sorting and filtering.
        /// </summary>
        /// <returns>A fully-configured <see cref="CollectionViewSource"/>.</returns>
        private CollectionViewSource ConfigureViewSource()
        {
            var viewSource = new CollectionViewSource
            {
                Source = Groupings,
                SortDescriptions =
                {
                    new(nameof(ReconciliationGrouping.FailedTotalCount), ListSortDirection.Descending),
                    new(nameof(ReconciliationGrouping.StartTime), ListSortDirection.Ascending),
                }
            };
            viewSource.Filter += Groupings_Filter;
            viewSource.View.CollectionChanged += RefreshCounters;
            return viewSource;
        }

        /// <summary>
        /// Invokes the PropertyChanged events for count properties, which updates the view.
        /// It is called whenever the <see cref="View"/> is changed (i.e., when filtering or adding data).
        /// </summary>
        private void RefreshCounters(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(OkCount));
            OnPropertyChanged(nameof(DisabledCount));
            OnPropertyChanged(nameof(FailedCount));
            OnPropertyChanged(nameof(FailMismatchCount));
        }

        /// <summary>
        /// Filter for the <see cref="View"/>.
        /// </summary>
        private void Groupings_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is ReconciliationGrouping grouping)
            {
                // Refresh the actual list of reconciliations (to correctly remove groupings that are empty).
                grouping.View.Refresh();
                e.Accepted = grouping.GroupName.Contains(SearchTerm) && (!grouping.View.IsEmpty || ShowEmpty);
            }
        }

        /// <summary>
        /// Filter for the <see cref="ReconciliationGrouping.View"/> which is for <see cref="ReconciliationDto"/>. <br/>
        /// </summary>
        private void Reconciliations_Filter(object sender, FilterEventArgs e)
        {
            ReconciliationDto node = (ReconciliationDto)e.Item;
            ReconciliationResult result = node.Result;
            bool isInExecution = _associationHelper.IsInExecution(node, SelectedExecution);
            e.Accepted = isInExecution
                && ((ShowOk && result is ReconciliationResult.Ok)
                || (ShowDisabled && result is ReconciliationResult.Disabled)
                || (ShowFailed && result is ReconciliationResult.Failed)
                || (ShowFailMismatch && result is ReconciliationResult.FailMismatch));
        }
    }
}
