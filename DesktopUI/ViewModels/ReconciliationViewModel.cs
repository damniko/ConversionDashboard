using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DesktopUI.Controllers;
using DesktopUI.Helpers;
using DesktopUI.Library;
using DesktopUI.Models;
using DesktopUI.Tools;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace DesktopUI.ViewModels
{
    public class ReconciliationViewModel : ObservableObject
    {
        private readonly CollectionViewSource _viewSource;
        private readonly ReconciliationController _controller;
        private readonly ExecutionController _executionController;
        private readonly ExecutionAssociationHelper _associationHelper;
        private readonly object _updateLock = new();
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
            queryTimerService.ReconciliationTimer.Elapsed += Refresh;
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
                OnPropertyChanged(nameof(ClearSelectedExecutionCommand));
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
        public int TotalCount => View.Cast<ReconciliationGrouping>().Sum(x => x.TotalCount);
        public int OkCount => View.Cast<ReconciliationGrouping>().Sum(x => x.OkCount);
        public int DisabledCount => View.Cast<ReconciliationGrouping>().Sum(x => x.DisabledCount);
        public int FailedCount => View.Cast<ReconciliationGrouping>().Sum(x => x.FailedCount);
        public int FailMismatchCount => View.Cast<ReconciliationGrouping>().Sum(x => x.FailMismatchCount);
        #endregion

        public ICommand RefreshCommand => new RelayCommand(() => Refresh(DateTime.Now));
        public ICommand ClearSelectedExecutionCommand => new RelayCommand(() => SelectedExecution = null, () => SelectedExecution != null);

        public void Refresh(DateTime date)
        {
            lock (_updateLock)
            {
                UpdateExecutions();
                UpdateReconciliations();
                _lastUpdated = date;
            }
        }

        private void UpdateExecutions()
        {
            var executions = _executionController.GetExecutions(_lastUpdated);
            App.Current.Dispatcher.Invoke(() =>
            {
                executions.ForEach(x => Executions.Add(x));
            });
        }

        private void UpdateReconciliations()
        {
            var groupDict = _controller.GetManagerReconciliationDict(_lastUpdated);

            if (groupDict.Any() is false) return;

            _viewSource.Dispatcher.Invoke(() =>
            {
                foreach (var key in groupDict.Keys)
                {
                    if (Groupings.FirstOrDefault(x => x.GroupName == key) is not ReconciliationGrouping grouping)
                    {
                        grouping = new(Reconciliations_Filter, key);
                        Groupings.Add(grouping);
                    }
                    grouping.AddReconciliations(groupDict[key]);
                }
                View.Refresh();
            });
        }

        private CollectionViewSource ConfigureViewSource()
        {
            var viewSource = new CollectionViewSource
            {
                Source = Groupings,
            };
            viewSource.Filter += Groupings_Filter;
            viewSource.View.CollectionChanged += RefreshCounters;
            return viewSource;
        }

        private void RefreshCounters(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(OkCount));
            OnPropertyChanged(nameof(DisabledCount));
            OnPropertyChanged(nameof(FailedCount));
            OnPropertyChanged(nameof(FailMismatchCount));
        }

        private void Groupings_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is ReconciliationGrouping grouping)
            {
                grouping.View.Refresh();
                e.Accepted = grouping.GroupName.Contains(SearchTerm) && (!grouping.View.IsEmpty || ShowEmpty);
            }
        }

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
