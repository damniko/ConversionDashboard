using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models
{
    /// <summary>
    /// A container for a number of <see cref="ReconciliationDto"/>s that share the same manager.
    /// </summary>
    public class ReconciliationGrouping : ObservableObject
    {
        private readonly CollectionViewSource _viewSource;
        private string _groupName = string.Empty;
        private bool _isExpanded;
        private bool _isSelected;

        public ReconciliationGrouping(FilterEventHandler filter)
        {
            _viewSource = ConfigureViewSource(filter);
        }

        public ReconciliationGrouping(FilterEventHandler filter, string groupName) : this(filter)
        {
            GroupName = groupName;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, false); // TODO - Figure out how to handle this (showing details in the Details section)
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
        public ICollectionView View => _viewSource.View;
        public string GroupName
        {
            get => _groupName;
            set
            {
                SetProperty(ref _groupName, value);
                OnPropertyChanged(nameof(GroupNameShort));
            }
        }
        public string GroupNameShort => string.Join('.', GroupName.Split('.').TakeLast(2));
        public List<ReconciliationDto> Reconciliations { get; } = new();
        public int TotalCount => Reconciliations.Count;
        public int OkCount => Reconciliations.Count(x => x.Result is ReconciliationResult.Ok);
        public int DisabledCount => Reconciliations.Count(x => x.Result is ReconciliationResult.Disabled);
        public int FailedCount => Reconciliations.Count(x => x.Result is ReconciliationResult.Failed);
        public int FailMismatchCount => Reconciliations.Count(x => x.Result is ReconciliationResult.FailMismatch);
        public int FailedTotalCount => FailedCount + FailMismatchCount;
        public DateTime? StartTime => Reconciliations.FirstOrDefault()?.StartTime.GetValueOrDefault();

        /// <summary>
        /// Adds the specified <paramref name="newReconciliations"/> to the <see cref="ReconciliationGrouping"/> and notifies the view that the count properties have changed.
        /// </summary>
        /// <param name="newReconciliations">The list of new reconciliations to add.</param>
        public void AddReconciliations(IEnumerable<ReconciliationDto> newReconciliations)
        {
            Reconciliations.AddRange(newReconciliations);
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(OkCount));
            OnPropertyChanged(nameof(DisabledCount));
            OnPropertyChanged(nameof(FailedCount));
            OnPropertyChanged(nameof(FailMismatchCount));
        }

        /// <summary>
        /// Configures a <see cref="CollectionViewSource"/> for <see cref="Reconciliations"/> with sorting and filtering.
        /// </summary>
        /// <returns>A fully-configured <see cref="CollectionViewSource"/>.</returns>
        private CollectionViewSource ConfigureViewSource(FilterEventHandler filter)
        {
            var viewSource = new CollectionViewSource()
            {
                Source = Reconciliations,
                SortDescriptions =
                {
                    new(nameof(ReconciliationDto.Result), ListSortDirection.Descending)
                }
            };
            viewSource.Filter += filter;
            
            return viewSource;
        }
    }
}
