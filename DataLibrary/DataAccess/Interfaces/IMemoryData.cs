using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface IMemoryData
{
    Task<List<Reading>> GetReadingsAsync(DateTime fromDate, string connStrKey);
    Task<long?> GetTotalAsync(DateTime fromDate, string connStrKey);
}