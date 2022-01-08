using System.Windows.Controls;
using DesktopUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopUI.Views;

/// <summary>
/// Interaction logic for ControlBarView.xaml
/// </summary>
public partial class ControlBarView : UserControl
{
    public ControlBarView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ControlBarViewModel>();
    }
}