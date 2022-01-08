using System.Windows;
using DesktopUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopUI.Views;

/// <summary>
/// Interaction logic for ShellView.xaml
/// </summary>
public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<MainViewModel>();
    }
}