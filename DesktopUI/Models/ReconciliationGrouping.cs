using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DesktopUI.Tools;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models
{
    public class ReconciliationGrouping : ObservableObject
    {
        private readonly CollectionViewSource _reconciliationsViewSource;
        private string _name = string.Empty;

        public ReconciliationGrouping(FilterEventHandler filter)
        {
            _reconciliationsViewSource = ConfigureViewSource(filter);
        }

        public string Name
        {
            get => _name; 
            set
            {
                string shortName = string.Join('.', value.Split('.').TakeLast(2));
                SetProperty(ref _name, shortName);
            }
        }
        public List<ReconciliationDto> Reconciliations { get; } = new();
        public ICollectionView ReconciliationsView => _reconciliationsViewSource.View;

        public void AddReconciliations(IEnumerable<ReconciliationDto> items)
        {
            _reconciliationsViewSource.Dispatcher.Invoke(() =>
            {
                using (_reconciliationsViewSource.DeferRefresh())
                {
                    Reconciliations.AddRange(items);
                }
                ReconciliationsView.Refresh();
            });
        }

        private CollectionViewSource ConfigureViewSource(FilterEventHandler filter)
        {
            var viewSource = new CollectionViewSource()
            {
                Source = Reconciliations,
            };
            viewSource.Filter += filter;

            return viewSource;
        }
    }
}
