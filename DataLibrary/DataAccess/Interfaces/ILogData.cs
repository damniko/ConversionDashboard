using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface ILogData
{
    Task<List<LogEntry>> GetSinceAsync(DateTime fromDate, string connStrKey);
}