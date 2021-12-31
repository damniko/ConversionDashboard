using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DataLibrary.Models;
using DesktopUI.Library;
using DesktopUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.ViewModels
{
    public class LogViewModel : ObservableObject
    {
        private readonly CollectionViewSource _messagesViewSource;
        private readonly QueryTimerService _queryTimerService;
        private bool _showInfo;
        private bool _showWarning;
        private bool _showError;
        private bool _showFatal;
        private bool _showReconciliation;

        public LogViewModel(QueryTimerService queryTimerService)
        {
            _messagesViewSource = new CollectionViewSource
            {
                Source = Entries,
                SortDescriptions = { new SortDescription(nameof(LogEntry.Created), ListSortDirection.Ascending) }
            };
            _messagesViewSource.Filter += MessagesViewSource_Filter;

            _queryTimerService = queryTimerService;
            _queryTimerService.LogTimer.Elapsed += LogTimer_Elapsed;
        }

        public List<LogEntry> Entries { get; } = new();
        public ICollectionView MessagesView => _messagesViewSource.View;
        #region Filter buttons
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

        private void MessagesViewSource_Filter(object sender, FilterEventArgs e)
        {
            LogLevel level = (e.Item as LogEntry)!.Level;
            e.Accepted = (ShowInfo && level.HasFlag(LogLevel.Info))
                || (ShowWarning && level.HasFlag(LogLevel.Warn))
                || (ShowError && level.HasFlag(LogLevel.Error))
                || (ShowFatal && level.HasFlag(LogLevel.Fatal))
                || (ShowReconciliation && level.HasFlag(LogLevel.Reconciliation));
        }

        private void LogTimer_Elapsed(DateTime date)
        {
            Trace.WriteLine($"Log timer elapsed at {date}");

            // get updated data from the data service

            App.Current.Dispatcher.Invoke(() =>
            {
                using (MessagesView.DeferRefresh())
                {
                    // Entries.AddRange(...);
                }
                MessagesView.Refresh();
            });
        }
    }
}
