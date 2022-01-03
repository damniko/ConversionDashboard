using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DesktopUI.Controllers;
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
        private readonly object _updateLock = new();
        private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        private bool _showEmpty = false;
        private bool _showOk = false;
        private bool _showDisabled = true;
        private bool _showFailed = true;
        private bool _showFailMismatch = true;
        private string _searchTerm = string.Empty;

        public ReconciliationViewModel(QueryTimerService queryTimerService, ReconciliationController controller)
        {
            _groupingsViewSource = ConfigureViewSource();
            queryTimerService.ReconciliationTimer.Elapsed += UpdateData;
            _controller = controller;
        }

        #region Properties
        public List<ReconciliationGroupingNode> Groupings { get; } = new();
        public ICollectionView GroupingsView => _groupingsViewSource.View;
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
                var newEntries = _controller.GetReconciliations(_lastUpdated);
                _lastUpdated = date;

                if (newEntries.Any() is false) return;

                // TODO - consider doing this in the controller instead (using AutoMapper)
                var groupDict = new Dictionary<string, List<ReconciliationDto>>();
                foreach (var entry in newEntries)
                {
                    if (groupDict.ContainsKey(entry.Manager))
                    {
                        groupDict[entry.Manager].Add(entry);
                    }
                    else
                    {
                        groupDict.Add(entry.Manager, new() { entry });
                    }
                }

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
                e.Accepted = grouping.Item.Contains(SearchTerm) && (!grouping.ReconciliationsView.IsEmpty || ShowEmpty);
            }
        }

        private void Reconciliations_Filter(object sender, FilterEventArgs e)
        {
            ReconciliationResult result = (e.Item as Node<ReconciliationDto>)!.Item.Result;
            e.Accepted = (ShowOk && result is ReconciliationResult.Ok)
                || (ShowDisabled && result is ReconciliationResult.Disabled)
                || (ShowFailed && result is ReconciliationResult.Failed)
                || (ShowFailMismatch && result is ReconciliationResult.FailMismatch);
        }
    }
}
