using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DesktopUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DesktopUI.Views;
/// <summary>
/// Interaction logic for SimpleManagerView.xaml
/// </summary>
public partial class SimpleManagerView : UserControl
{
    public SimpleManagerView()
    {
        InitializeComponent();
        DataContext = App.Current.Services.GetService<ManagerViewModel>();
    }
}
