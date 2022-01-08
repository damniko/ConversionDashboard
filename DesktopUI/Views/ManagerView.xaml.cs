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
}