using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DesktopUI.Controllers;
using DesktopUI.Library;
using DesktopUI.Models;
using DesktopUI.Tools;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.ViewModels
{
    public class ReconciliationViewModel : ObservableObject
    {
        private readonly CollectionViewSource _groupingsViewSource;
        private readonly ReconciliationController _controller;
        private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        private bool _showOk;
        private bool _showDisabled = true;
        private bool _showFailed = true;
        private bool _showFailMismatch = true;

        public ReconciliationViewModel(QueryTimerService queryTimerService, ReconciliationController controller)
        {
            _groupingsViewSource = ConfigureViewSource();
            queryTimerService.ReconciliationTimer.Elapsed += ReconciliationTimer_Elapsed;
            _controller = controller;
        }

        #region Properties
        public ObservableList<ReconciliationGrouping> Groupings { get; } = new();
        public ICollectionView GroupingsView => _groupingsViewSource.View;
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
            if(e.Item is ReconciliationGrouping grouping)
            {
                grouping.ReconciliationsView.Refresh();
                e.Accepted = grouping.ReconciliationsView.IsEmpty is false;
            }
        }

        private void ReconciliationTimer_Elapsed(DateTime date)
        {
            var newEntries = _controller.GetReconciliations(_lastUpdated);
            _lastUpdated = date;

            if(newEntries.Any() is false) return;

            // TODO - consider doing this in the controller instead (using AutoMapper)
            var groupDict = new Dictionary<string, List<ReconciliationDto>>();
            foreach(var entry in newEntries)
            {
                if(groupDict.ContainsKey(entry.Manager))
                {
                    groupDict[entry.Manager].Add(entry);
                }
                else
                {
                    groupDict.Add(entry.Manager, new() { entry });
                }
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                using (GroupingsView.DeferRefresh())
                {
                    foreach(var key in groupDict.Keys)
                    {
                        if(Groupings.Find(x => x.Name == key) is ReconciliationGrouping grouping)
                        {
                            grouping.AddReconciliations(groupDict[key]);
                        }
                        else
                        {
                            grouping = new ReconciliationGrouping(Reconciliations_Filter)
                            {
                                Name = key,
                            };
                            grouping.AddReconciliations(groupDict[key]);
                            Groupings.Add(grouping);
                        }
                    }
                }
                GroupingsView.Refresh();
            });
        }

        private void Reconciliations_Filter(object sender, FilterEventArgs e)
        {
            ReconciliationResult result = (e.Item as ReconciliationDto)!.Result;
            e.Accepted = (ShowOk && result is ReconciliationResult.Ok)
                || (ShowDisabled && result is ReconciliationResult.Disabled)
                || (ShowFailed && result is ReconciliationResult.Failed)
                || (ShowFailMismatch && result is ReconciliationResult.FailMismatch);
        }
    }
}
