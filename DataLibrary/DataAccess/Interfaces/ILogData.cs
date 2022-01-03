using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface ILogData
    {
        List<LogEntry> GetSince(DateTime fromDate, string connStrKey);
    }
}