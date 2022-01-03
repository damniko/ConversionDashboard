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

        public ReconciliationGrouping(FilterEventHandler filter)
        {
            _reconciliationsViewSource = ConfigureViewSource(filter);    
        }

        public string Name { get; set; } = string.Empty;
        public List<ReconciliationDto> Reconciliations { get; } = new();
        public ICollectionView ReconciliationsView => _reconciliationsViewSource.View;

        public void AddReconciliations(IEnumerable<ReconciliationDto> items)
        {
            using (ReconciliationsView.DeferRefresh())
            {
                foreach(var item in items)
                {
                    Reconciliations.Add(item);
                }
            }
            ReconciliationsView.Refresh();
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
