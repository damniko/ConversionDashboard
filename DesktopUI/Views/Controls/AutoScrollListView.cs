using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DesktopUI.Views.Behaviors;
using System.Windows.Data;
using System.Reflection;
using DesktopUI.ViewModels;

namespace DesktopUI.Views.Controls;

public class AutoScrollListView : ListView
{
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        // TODO - figure out how to have AutoScroll as a dependency property to decouple it from the viewmodel
        base.OnItemsChanged(e);
        if (((LogViewModel)DataContext).AutoScroll is true && Items.Count > 0)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateLayout();
                ScrollIntoView(Items[^1]);
            });
        }
    }
}