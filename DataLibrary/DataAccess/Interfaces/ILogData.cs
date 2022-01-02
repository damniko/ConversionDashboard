using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface ILogData
    {
        List<LogEntry> GetLogEntries(DateTime fromDate, string connStrKey);
    }
}