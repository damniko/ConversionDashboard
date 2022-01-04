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
        private readonly CollectionViewSource _groupingsViewSource;
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
            _groupingsViewSource = ConfigureViewSource();
            queryTimerService.ReconciliationTimer.Elapsed += UpdateData;

            Executions.Add(new Node<ExecutionDto>("(All)"));
            _selectedExecution = Executions.First();
        }

        #region Properties
        public List<ReconciliationGroupingNode> Groupings { get; } = new();
        public ICollectionView GroupingsView => _groupingsViewSource.View;
        public ObservableCollection<Node<ExecutionDto>> Executions { get; } = new();
        public Node<ExecutionDto> SelectedExecution 
        {
            get => _selectedExecution;
            set
            {
                SetProperty(ref _selectedExecution, value);
                GroupingsView.Refresh();
            }
        }
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                SetProperty(ref _searchTerm, value);
                GroupingsView.Refresh();
            }
        }
        public bool ShowEmpty
        {
            get => _showEmpty;
            set
            {
                SetProperty(ref _showEmpty, value);
                GroupingsView.Refresh();
            }
        }
        public bool ShowOk
        {
            get => _showOk;
            set
            {
                SetProperty(ref _showOk, value);
                GroupingsView.Refresh();
            }
        }
        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                SetProperty(ref _showDisabled, value);
                GroupingsView.Refresh();
            }
        }
        public bool ShowFailed
        {
            get => _showFailed;
            set
            {
                SetProperty(ref _showFailed, value);
                GroupingsView.Refresh();
            }
        }
        public bool ShowFailMismatch
        {
            get => _showFailMismatch;
            set
            {
                SetProperty(ref _showFailMismatch, value);
                GroupingsView.Refresh();
            }
        }
        #endregion

        public ICommand RefreshCommand => new RelayCommand(() => UpdateData(DateTime.Now));

        public void UpdateData(DateTime date)
        {
            lock (_updateLock)
            {
                var executions = _executionController.GetExecutions(_lastUpdated);
                App.Current.Dispatcher.Invoke(() =>
                {
                    executions.ForEach(e => Executions.Add(new Node<ExecutionDto>(e, $"Execution {e.Id}")));
                });

                var groupDict = _controller.GetManagerReconciliationDict(_lastUpdated);
                _lastUpdated = date;

                if (groupDict.Any() is false) return;

                _groupingsViewSource.Dispatcher.Invoke(() =>
                {
                    using (_groupingsViewSource.DeferRefresh())
                    {
                        foreach (var key in groupDict.Keys)
                        {
                            var grouping = Groupings.FirstOrDefault(x => x.Item == string.Join('.', key.Split('.').TakeLast(2))); // TODO - refactor this
                            if (grouping is null)
                            {
                                grouping = new ReconciliationGroupingNode(key, Reconciliations_Filter);
                                Groupings.Add(grouping);
                            }
                            grouping.AddReconciliations(groupDict[key]);
                        }
                    }
                    GroupingsView.Refresh();
                });
            }
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
            if(e.Item is ReconciliationGroupingNode grouping)
            {
                grouping.ReconciliationsView.Refresh();
                e.Accepted = grouping.DisplayValue.Contains(SearchTerm) && (!grouping.ReconciliationsView.IsEmpty || ShowEmpty);
            }
        }

        private void Reconciliations_Filter(object sender, FilterEventArgs e)
        {
            Node<ReconciliationDto> node = (Node<ReconciliationDto>)e.Item;
            ReconciliationResult result = node.Item!.Result;
            bool isInExecution = _associationHelper.IsInExecution(node.Item, SelectedExecution?.Item);
            e.Accepted = isInExecution
                && ((ShowOk && result is ReconciliationResult.Ok)
                || (ShowDisabled && result is ReconciliationResult.Disabled)
                || (ShowFailed && result is ReconciliationResult.Failed)
                || (ShowFailMismatch && result is ReconciliationResult.FailMismatch));
        }
    }
}
