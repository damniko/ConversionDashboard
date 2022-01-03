using System;
using System.ComponentModel;
using System.Diagnostics;
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
    public class LogViewModel : ObservableObject
    {
        private readonly CollectionViewSource _messagesViewSource;
        private readonly QueryTimerService _queryTimerService;
        private readonly LogController _controller;
        private DateTime _lastUpdated = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
        private bool _showInfo = true;
        private bool _showWarning = true;
        private bool _showError = true;
        private bool _showFatal = true;
        private bool _showReconciliation = true;
        private bool _autoScroll;

        public LogViewModel(QueryTimerService queryTimerService, LogController controller)
        {
            _messagesViewSource = new CollectionViewSource
            {
                Source = Entries,
            };
            _messagesViewSource.Filter += MessagesViewSource_Filter;

            _queryTimerService = queryTimerService;
            _controller = controller;
            _queryTimerService.LogTimer.Elapsed += LogTimer_Elapsed;
        }

        #region Properties
        public ObservableList<LogEntryDto> Entries { get; } = new();
        public ICollectionView MessagesView => _messagesViewSource.View;
        public bool AutoScroll
        {
            get => _autoScroll;
            set => SetProperty(ref _autoScroll, value);
        }
        public bool ShowInfo
        {
            get => _showInfo;
            set
            {
                if(SetProperty(ref _showInfo, value)) MessagesView.Refresh();
            }
        }
        public bool ShowWarning
        {
            get => _showWarning;
            set
            {
                if(SetProperty(ref _showWarning, value)) MessagesView.Refresh();
            }
        }
        public bool ShowError 
        {
            get => _showError;
            set
            {
                if(SetProperty(ref _showError, value)) MessagesView.Refresh();
            }
        }
        public bool ShowFatal
        {
            get => _showFatal;
            set
            {
                if(SetProperty(ref _showFatal, value)) MessagesView.Refresh();
            }
        }
        public bool ShowReconciliation
        {
            get => _showReconciliation;
            set
            {
                if(SetProperty(ref _showReconciliation, value)) MessagesView.Refresh();
            }
        }
        #endregion

        #region Commands
        public ICommand EnableAutoScrollCommand => new RelayCommand(() => 
        {
            AutoScroll = true;
            Trace.WriteLine("tru");
        }, () => AutoScroll == false);
        public ICommand DisableAutoScrollCommand => new RelayCommand(() =>
        {
            AutoScroll = false;
            Trace.WriteLine("fal");
        }, () => AutoScroll == true);
        #endregion

        private void MessagesViewSource_Filter(object sender, FilterEventArgs e)
        {
            LogLevel level = (e.Item as LogEntryDto)!.Level;
            e.Accepted = (ShowInfo && level.HasFlag(LogLevel.Info))
                || (ShowWarning && level.HasFlag(LogLevel.Warn))
                || (ShowError && level.HasFlag(LogLevel.Error))
                || (ShowFatal && level.HasFlag(LogLevel.Fatal))
                || (ShowReconciliation && level.HasFlag(LogLevel.Reconciliation));
        }

        private void LogTimer_Elapsed(DateTime date)
        {
            var newEntries = _controller.GetLogEntries(_lastUpdated);
            _lastUpdated = date;

            if(newEntries.Any() is false)
            {
                return;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                using (MessagesView.DeferRefresh())
                {
                    Entries.AddRange(newEntries);
                }
                MessagesView.Refresh();
            });
        }
    }
}
