using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DataLibrary.Models;
using DesktopUI.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.ViewModels
{
    public class ReconciliationViewModel : ObservableObject
    {
        private readonly CollectionViewSource _reconciliationsViewSource;
        private bool _showOk;
        private bool _showDisabled;
        private bool _showFailed;
        private bool _showFailMismatch;

        public ReconciliationViewModel(QueryTimerService queryTimerService)
        {
            _reconciliationsViewSource = new CollectionViewSource
            {
                Source = Reconciliations,
                SortDescriptions =
                {
                    new(nameof(Reconciliation.Result), ListSortDirection.Descending),
                    new(nameof(Reconciliation.Date), ListSortDirection.Ascending)
                },
                GroupDescriptions =
                {
                    new PropertyGroupDescription(nameof(Reconciliation.Manager))
                }
            };
            _reconciliationsViewSource.Filter += ReconciliationsViewSource_Filter;

            queryTimerService.ReconciliationTimer.Elapsed += ReconciliationTimer_Elapsed;
        }

        #region Properties
        public List<Reconciliation> Reconciliations { get; } = new();
        public ICollectionView ReconciliationsView => _reconciliationsViewSource.View;
        public bool ShowOk
        {
            get => _showOk;
            set
            {
                SetProperty(ref _showOk, value);
                ReconciliationsView.Refresh();
            }
        }
        public bool ShowDisabled
        {
            get => _showDisabled;
            set
            {
                SetProperty(ref _showDisabled, value);
                ReconciliationsView.Refresh();
            }
        }
        public bool ShowFailed
        {
            get => _showFailed;
            set
            {
                SetProperty(ref _showFailed, value);
                ReconciliationsView.Refresh();
            }
        }
        public bool ShowFailMismatch
        {
            get => _showFailMismatch;
            set
            {
                SetProperty(ref _showFailMismatch, value);
                ReconciliationsView.Refresh();
            }
        }
        #endregion

        private void ReconciliationTimer_Elapsed(DateTime date)
        {

        }

        private void ReconciliationsViewSource_Filter(object sender, FilterEventArgs e)
        {
            ReconciliationResult result = (e.Item as Reconciliation)!.Result;
            e.Accepted = (ShowOk && result is ReconciliationResult.Ok)
                || (ShowDisabled && result is ReconciliationResult.Disabled)
                || (ShowFailed && result is ReconciliationResult.Failed)
                || (ShowFailMismatch && result is ReconciliationResult.FailMismatch);
        }
    }
}
