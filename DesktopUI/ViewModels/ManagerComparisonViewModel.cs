using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DesktopUI.Models;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using DesktopUI.ViewModels.Charts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using DesktopUI.ViewModels.Charts;

namespace DesktopUI.ViewModels
{
    public class ManagerComparisonViewModel : ObservableObject
    {
        private readonly CollectionViewSource _viewSource;
        private string _searchTerm = string.Empty;

        public ManagerComparisonViewModel(IEnumerable<ManagerDto> managers)
        {
            //RowsChartVM = App.Current.Services.GetService<IBarChartViewModel>()!;
            RowsChartVM = new ManagerRowsBarChartVM(); // TODO - fix this up for dependency injection
            RuntimeChartVM = new ManagerRuntimeBarChartVM();
            _viewSource = ConfigureViewSource(managers);
        }

        public IBarChartViewModel RowsChartVM { get; }
        public IBarChartViewModel RuntimeChartVM { get; }

        public ICollectionView View => _viewSource.View;
        public ObservableCollection<ManagerDto> SelectedManagers { get; } = new();
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                SetProperty(ref _searchTerm, value);
                View.Refresh();
            }
        }

        public ICommand AddManagerCmd => new RelayCommand<object>(param =>
        {
            IList<object> items = (IList<object>)param!;
            var list = items.Cast<ManagerDto>().ToList();
            foreach (var manager in list.ToList())
            {
                if (!SelectedManagers.Contains(manager))
                {
                    SelectedManagers.Add(manager);
                }
                else
                {
                    list.Remove(manager);
                }
            }
            RowsChartVM.AddSeries(list);
            RuntimeChartVM.AddSeries(list);
        });
        public ICommand ToggleManagerCmd => new RelayCommand<object>(param => 
        {
            IList<object> items = (IList<object>)param!;
            var list = items.Cast<ManagerDto>().ToList();
            var added = new List<ManagerDto>();
            var removed = new List<ManagerDto>();
            foreach (var manager in list)
            {
                if (SelectedManagers.Contains(manager))
                {
                    SelectedManagers.Remove(manager);
                    removed.Add(manager);
                }
                else
                {
                    SelectedManagers.Add(manager);
                    added.Add(manager);
                }
            }
            RowsChartVM.AddSeries(added);
            RowsChartVM.RemoveSeries(removed);
            RuntimeChartVM.AddSeries(added);
            RuntimeChartVM.RemoveSeries(removed);
        });
        public ICommand RemoveManagerCmd => new RelayCommand<object>(param =>
        {
            IList<object> items = (IList<object>)param!;
            var list = items.Cast<ManagerDto>().ToList();
            foreach (var manager in list)
            {
                SelectedManagers.Remove(manager);
            }
            RowsChartVM.RemoveSeries(list);
            RuntimeChartVM.RemoveSeries(list);
        });

        private CollectionViewSource ConfigureViewSource(IEnumerable<ManagerDto> managers)
        {
            var viewSource = new CollectionViewSource
            {
                Source = managers,
                SortDescriptions =
                {
                    new SortDescription(nameof(ManagerDto.StartTime), ListSortDirection.Ascending)
                }
            };
            viewSource.Filter += Managers_Filter;
            return viewSource;
        }

        private void Managers_Filter(object sender, FilterEventArgs e)
        {
            var item = (ManagerDto)e.Item;
            e.Accepted = /*!SelectedManagers.Contains(item) &&*/ item.Name.Contains(SearchTerm);
        }
    }
}
