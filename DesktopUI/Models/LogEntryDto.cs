using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace DesktopUI.Models
{
    [Flags]
    public enum LogLevel : byte
    {
        None = 0,
        Info = 1,
        Warn = 2,
        Error = 4,
        Fatal = 8,
        Reconciliation = 16
    }

    public class LogEntryDto : ObservableObject
    {
        private string _message = string.Empty;

        public DateTime Created { get; set; }
        public string Message
        {
            get => _message;
            set
            {
                // Strips the message of color-coding (so it can be handled by the view)
                _message = Regex.Replace(value, @"\u001b\[\d*;?\d+m", "");
            }
        }
        public LogLevel Level { get; set; }
        public long ExecutionId { get; set; }
        public long ContextId { get; set; }
        public string? Manager { get; set; }
        public string? ManagerShort => Manager is null 
            ? Manager
            : string.Join('.', Manager.Split('.').TakeLast(2));
    }
}
