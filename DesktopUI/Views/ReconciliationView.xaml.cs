using System.Windows.Controls;
using DesktopUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopUI.Views;

/// <summary>
/// Interaction logic for ReconciliationReportView.xaml
/// </summary>
public partial class ReconciliationView : UserControl
{
    public ReconciliationView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ReconciliationViewModel>();
    }
}