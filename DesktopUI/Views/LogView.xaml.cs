using System.Windows.Controls;
using DesktopUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopUI.Views;

/// <summary>
/// Interaction logic for LogView.xaml
/// </summary>
public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<LogViewModel>();
    }
}