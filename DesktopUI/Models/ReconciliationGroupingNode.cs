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
    public class ReconciliationGroupingNode : Node<string>
    {
        private readonly CollectionViewSource _reconciliationsViewSource;
        private string _name = string.Empty;

        public ReconciliationGroupingNode(string item, FilterEventHandler filter) 
            : base(string.Join('.', item.Split('.').TakeLast(2)))
        {
            _reconciliationsViewSource = ConfigureViewSource(filter);
        }

        public List<Node<ReconciliationDto>> Reconciliations { get; } = new();
        public ICollectionView ReconciliationsView => _reconciliationsViewSource.View;

        public void AddReconciliations(IEnumerable<ReconciliationDto> items)
        {
            var nodes = items.Select(x => new Node<ReconciliationDto>(x));

            _reconciliationsViewSource.Dispatcher.Invoke(() =>
            {
                using (_reconciliationsViewSource.DeferRefresh())
                {
                    Reconciliations.AddRange(nodes);
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
