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
        private bool _autoScroll;
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
                OnPropertyChanged(nameof(ClearSelectedExecutionCmd));
                View.Refresh();
            }
        }
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
        public ICommand SetContextIdCmd
            => new RelayCommand<LogEntryDto>(e => ShownContextId = e?.ContextId);
        public ICommand ClearContextIdCmd => new RelayCommand(() => ShownContextId = null);
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
                newExecs.ForEach(x => Executions.Add(x));
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
            bool acceptContextId = ShownContextId is null || item.ContextId == ShownContextId.Value;  
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
