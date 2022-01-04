using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Tools
{
    public class Node<T> : ObservableObject
    {
        private bool _isSelected;
        private bool _isExpanded;

        public Node(T? item, string displayValue)
        {
            Item = item;
            DisplayValue = displayValue;
        }
        public Node(T item) : this(item, string.Empty) { }
        public Node(string displayValue) : this(default, displayValue) { }

        public T? Item { get; }
        public string DisplayValue { get; } = string.Empty;

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
