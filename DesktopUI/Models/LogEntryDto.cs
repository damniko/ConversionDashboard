using System;

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

    public class LogEntryDto
    {
        public DateTime Created { get; set; }
        public string Message { get; set; } = string.Empty;
        public LogLevel Level { get; set; }
        public long ExecutionId { get; set; }
        public long ContextId { get; set; }
        public string? Manager { get; set; }
    }
}
