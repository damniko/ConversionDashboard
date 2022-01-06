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
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace DesktopUI.ViewModels
{
    public class LogViewModel : ObservableObject
    {
        private readonly CollectionViewSource _viewSource;
        private readonly ExecutionAssociationHelper _associationHelper;
        private readonly LogController _controller;
        private readonly ExecutionController _executionController;
        private readonly List<LogEntryDto> _entries = new();
        private readonly object _updateLock = new();
        private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        private bool _showInfo = true;
        private bool _showWarning = true;
        private bool _showError = true;
        private bool _showFatal = true;
        private bool _showReconciliation = true;
        private bool _autoScroll = true;
        private ExecutionDto? _selectedExecution;
        private string _searchTerm = string.Empty;
        private long? _shownContextId;

        public LogViewModel(QueryTimerService queryTimerService,
                            ExecutionAssociationHelper associationHelper,
                            LogController controller,
                            ExecutionController executionController)
        {
            _associationHelper = associationHelper;
            _controller = controller;
            _executionController = executionController;
            _viewSource = ConfigureViewSource();
            queryTimerService.LogTimer.Elapsed += (d) => UpdateDataCmd.Execute(d);
        }

        #region Properties
        public ICollectionView View => _viewSource.View;
        public ObservableCollection<ExecutionDto> Executions { get; } = new();
        public ExecutionDto? SelectedExecution
        {
            get => _selectedExecution;
            set
            {
                SetProperty(ref _selectedExecution, value);
                UpdateContextIdFilter();
                OnPropertyChanged(nameof(ClearSelectedExecutionCmd));
                View.Refresh();
            }
        }
        public List<ContextIdContainer> ContextIdFilters { get; } = new();
        public bool ShowAllContextIds => ContextIdFilters.TrueForAll(x => x.IsChecked is false);
        public bool AutoScroll
        {
            get => _autoScroll;
            set => SetProperty(ref _autoScroll, value);
        }
        public long? ShownContextId
        {
            get => _shownContextId;
            set
            {
                SetProperty(ref _shownContextId, value);
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
        public bool ShowInfo
        {
            get => _showInfo;
            set
            {
                SetProperty(ref _showInfo, value);
                View.Refresh();
            }
        }
        public bool ShowWarning
        {
            get => _showWarning;
            set
            {
                SetProperty(ref _showWarning, value);
                View.Refresh();
            }
        }
        public bool ShowError 
        {
            get => _showError;
            set
            {
                SetProperty(ref _showError, value);
                View.Refresh();
            }
        }
        public bool ShowFatal
        {
            get => _showFatal;
            set
            {
                SetProperty(ref _showFatal, value);
                View.Refresh();
            }
        }
        public bool ShowReconciliation
        {
            get => _showReconciliation;
            set
            {
                SetProperty(ref _showReconciliation, value);
                View.Refresh();
            }
        }
        public int InfoCount
            => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Info));
        public int WarnCount
            => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Warn));
        public int ErrorCount
            => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Error));
        public int FatalCount
            => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Fatal));
        public int ReconcCount
            => _entries.Count(x => IsInExecution(x) && x.Level.HasFlag(LogLevel.Reconciliation));
        public int TotalCount => _entries.Count(x => IsInExecution(x));
        #endregion

        /// <summary>
        /// Gets any new data and triggers an update for the view.
        /// </summary>
        public ICommand UpdateDataCmd => new RelayCommand(() => UpdateData(DateTime.Now));
        /// <summary>
        /// Clears the value of the current <see cref="SelectedExecution"/>.
        /// </summary>
        public ICommand ClearSelectedExecutionCmd
            => new RelayCommand(() => SelectedExecution = null, () => SelectedExecution != null);
        public ICommand AddContextIdCmd => new RelayCommand<LogEntryDto>(e =>
        {
            if (ContextIdFilters.FirstOrDefault(x => x.ContextId == e?.ContextId) is { } x)
            {
                x.IsChecked = true;
            }
        });
        public ICommand ClearContextIdCmd => new RelayCommand(() => ContextIdFilters.ForEach(x => x.IsChecked = false));
        public ICommand EnableAutoScrollCommand
            => new RelayCommand(() => AutoScroll = true, () => AutoScroll == false);
        public ICommand DisableAutoScrollCommand 
            => new RelayCommand(() => AutoScroll = false, () => AutoScroll == true);

        private void UpdateData(DateTime date)
        {
            lock (_updateLock)
            {
                UpdateExecutions();
                UpdateEntries();
                _lastUpdated = date;
            }
        }

        /// <summary>
        /// Gets executions newer than <see cref="_lastUpdated"/> from the <see cref="ExecutionController"/>, and adds them to the view.
        /// </summary>
        private void UpdateExecutions()
        {
            var newExecs = _executionController.GetExecutions(_lastUpdated);
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach(var exec in newExecs)
                {
                    Executions.Add(exec);
                    // Assign 'Manager' values to entries by using the execution's Context ID dictionary.
                    foreach (var entry in _entries.Where(x => x.ExecutionId == exec.Id))
                    {
                        if (exec.ContextDict.ContainsKey(entry.ContextId))
                        {
                            entry.Manager = exec.ContextDict[entry.ContextId];
                        }
                    }
                }
            });
        }

        private void UpdateEntries()
        {
            var newEntries = _controller.GetLogEntries(_lastUpdated);

            if (newEntries.Any() is false) return;

            App.Current.Dispatcher.Invoke(() =>
            {
                using (View.DeferRefresh())
                {
                    _entries.AddRange(newEntries);
                }
                View.Refresh();
            });
        }

        private CollectionViewSource ConfigureViewSource()
        {
            var viewSource = new CollectionViewSource
            {
                Source = _entries,
            };
            viewSource.Filter += Entries_Filter;
            viewSource.View.CollectionChanged += RefreshCounters;
            return viewSource;
        }

        private void UpdateContextIdFilter()
        {
            ContextIdFilters.Clear();
            if (SelectedExecution != null)
            {
                foreach (var contextId in SelectedExecution.ContextDict.Keys)
                {
                    var container = new ContextIdContainer { ContextId = contextId };
                    container.PropertyChanged += (_, _) => View.Refresh();
                    ContextIdFilters.Add(container);
                }
            }
            OnPropertyChanged(nameof(ContextIdFilters));
        }

        private void RefreshCounters(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(InfoCount));
            OnPropertyChanged(nameof(WarnCount));
            OnPropertyChanged(nameof(ErrorCount));
            OnPropertyChanged(nameof(FatalCount));
            OnPropertyChanged(nameof(ReconcCount));
        }

        private void Entries_Filter(object sender, FilterEventArgs e)
        {
            LogEntryDto item = (LogEntryDto)e.Item;
            LogLevel level = item.Level;
            bool acceptContextId = ShowAllContextIds || ContextIdFilters.Any(x => x.IsChecked && x.ContextId == item.ContextId);
            e.Accepted = (IsInExecution(item) && acceptContextId && item.Message.Contains(SearchTerm))
                && ((ShowInfo && level.HasFlag(LogLevel.Info))
                || (ShowWarning && level.HasFlag(LogLevel.Warn))
                || (ShowError && level.HasFlag(LogLevel.Error))
                || (ShowFatal && level.HasFlag(LogLevel.Fatal))
                || (ShowReconciliation && level.HasFlag(LogLevel.Reconciliation)));
        }

        private bool IsInExecution(LogEntryDto item)
        {
            return SelectedExecution is null || item.ExecutionId == SelectedExecution.Id;
        }
    }
}
