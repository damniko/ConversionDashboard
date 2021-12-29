using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface ILogData
    {
        List<LogEntry> GetLogEntriesSinceDate(DateTime fromDate, string connStrKey);
    }
}