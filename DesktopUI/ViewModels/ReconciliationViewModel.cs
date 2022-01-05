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
        private Node<ExecutionDto> _selectedExecution;

        public ReconciliationViewModel(QueryTimerService queryTimerService,
                                       ReconciliationController reconciliationController,
                                       ExecutionController executionController,
                                       ExecutionAssociationHelper associationHelper)
        {
            _controller = reconciliationController;
            _executionController = executionController;
            _associationHelper = associationHelper;
            _viewSource = ConfigureViewSource();
            queryTimerService.ReconciliationTimer.Elapsed += UpdateExecutions;
            queryTimerService.ReconciliationTimer.Elapsed += UpdateReconciliations;

            Executions.Add(new Node<ExecutionDto>("(All)")); // TODO - find less hacky way to achieve this
            _selectedExecution = Executions.First();
        }

        #region Properties
        public List<ReconciliationGrouping> Groupings { get; } = new();
        public ICollectionView View => _viewSource.View;
        public ObservableCollection<Node<ExecutionDto>> Executions { get; } = new();
        public Node<ExecutionDto> SelectedExecution 
        {
            get => _selectedExecution;
            set
            {
                SetProperty(ref _selectedExecution, value);
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
        #endregion

        public ICommand RefreshCommand => new RelayCommand(() => Refresh(DateTime.Now));

        public void Refresh(DateTime date)
        {
            lock (_updateLock)
            {
                UpdateExecutions(_lastUpdated);
                UpdateReconciliations(_lastUpdated);
                _lastUpdated = date;
            }
        }

        private void UpdateExecutions(DateTime date)
        {
            var executions = _executionController.GetExecutions(_lastUpdated);
            App.Current.Dispatcher.Invoke(() =>
            {
                executions.ForEach(e => Executions.Add(new Node<ExecutionDto>(e, $"Execution {e.Id}")));
            });
        }

        public void UpdateReconciliations(DateTime date)
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
            return viewSource;
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
            bool isInExecution = _associationHelper.IsInExecution(node, SelectedExecution?.Item);
            e.Accepted = isInExecution
                && ((ShowOk && result is ReconciliationResult.Ok)
                || (ShowDisabled && result is ReconciliationResult.Disabled)
                || (ShowFailed && result is ReconciliationResult.Failed)
                || (ShowFailMismatch && result is ReconciliationResult.FailMismatch));
        }
    }
}
