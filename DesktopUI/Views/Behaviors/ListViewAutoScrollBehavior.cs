using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DesktopUI.Views.Controls;
using Microsoft.Xaml.Behaviors;

namespace DesktopUI.Views.Behaviors
{
    public class ListViewAutoScrollBehavior : Behavior<ListView>
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.Register(
                "AutoScroll",
                typeof(bool),
                typeof(ListViewAutoScrollBehavior),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //public static readonly DependencyProperty AutoScroll = DependencyProperty.RegisterAttached(nameof(AutoScroll), typeof(Binding), typeof(ListBoxAutoScrollBehavior));
        [Category("Common")]
        public bool AutoScrollEnabled
        {
            get => (bool)GetValue(AutoScrollProperty);
            set => SetValue(AutoScrollProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged += ListBoxAutoScrollBehavior_CollectionChanged;
        }

        private void ListBoxAutoScrollBehavior_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (AutoScrollEnabled && sender is ItemCollection items && items.Count > 0)
            {
                AssociatedObject.Dispatcher.Invoke(() =>
                {
                    AssociatedObject.UpdateLayout();
                    AssociatedObject.ScrollIntoView(AssociatedObject.Items[^1]);
                });
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            ((INotifyCollectionChanged)AssociatedObject.Items).CollectionChanged -= ListBoxAutoScrollBehavior_CollectionChanged;
        }
    }
}
