using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models
{
    public class ContextIdContainer : ObservableObject
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked; 
            set => SetProperty(ref _isChecked, value);
        }
        public long ContextId { get; set; }
    }
}
