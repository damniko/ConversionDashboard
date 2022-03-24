using System.Windows;
using System.Windows.Controls;
using DesktopUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopUI.Views;

public partial class ManagerView : UserControl
{
    public ManagerView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ManagerViewModel>();
    }

    public void SelectManagerHandler(object sender, RoutedEventArgs e)
    {
        // TODO - use behaviors to do this more elegantly
        ((ManagerViewModel)DataContext).SelectManagerCmd.Execute(((DataGridRow)e.Source).DataContext);
    }

    private void SelectManagerHandler(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter)
        {
            ((ManagerViewModel)DataContext).SelectManagerCmd.Execute(((DataGridCell)e.OriginalSource).DataContext);
        }
    }
}