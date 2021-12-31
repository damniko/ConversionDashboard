using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DesktopUI.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace DesktopUI.ViewModels
{
    public class ControlBarViewModel : ObservableObject
    {
        private readonly QueryTimerService _timerService;
        private bool _isMonitoring;

        public ControlBarViewModel(QueryTimerService timerService)
        {
            _timerService = timerService;
        }

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set
            {
                SetProperty(ref _isMonitoring, value);
                OnPropertyChanged(nameof(StartCommand));
                OnPropertyChanged(nameof(StopCommand));
            }
        }

        public ICommand StartCommand 
            => new RelayCommand(StartMonitoring, () => IsMonitoring is false);

        public ICommand StopCommand
            => new RelayCommand(StopMonitoring, () => IsMonitoring is true);

        private void StartMonitoring()
        {
            // TODO - add logging service here
            Trace.WriteLine("Starting all timers");
            _timerService.StartAll();
            IsMonitoring = true;
        }

        private void StopMonitoring()
        {
            Trace.WriteLine("Stopping all timers");
            _timerService.StopAll();
            IsMonitoring = false;
        }
    }
}
