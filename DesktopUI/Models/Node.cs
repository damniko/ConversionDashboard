using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models
{
    public class Node<T> : ObservableObject
    {
        private bool _isSelected;
        private bool _isExpanded;

        public Node(T item)
        {
            Item = item;
        }

        public T Item { get; }
        public bool IsSelected
        {
            get => _isSelected; 
            set => SetProperty(ref _isSelected, value);
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
    }
}
