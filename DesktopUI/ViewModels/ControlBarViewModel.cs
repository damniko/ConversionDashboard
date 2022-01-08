using System.Windows.Input;
using DesktopUI.Library;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace DesktopUI.ViewModels;

public class ControlBarViewModel : ObservableObject
{
    private readonly QueryTimerService _timerService;
    private readonly ILogger<ControlBarViewModel> _logger;
    private bool _isMonitoring;

    public ControlBarViewModel(QueryTimerService timerService, ILogger<ControlBarViewModel> logger)
    {
        _timerService = timerService;
        _logger = logger;
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
        _logger.LogInformation("Starting all timers");
        _timerService.StartAll();
        IsMonitoring = true;
    }

    private void StopMonitoring()
    {
        _logger.LogInformation("Stopping all timers");
        _timerService.StopAll();
        IsMonitoring = false;
    }
}