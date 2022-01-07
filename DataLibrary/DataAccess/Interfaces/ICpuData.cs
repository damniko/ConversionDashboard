using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public interface ICpuData
    {
        Task<List<Reading>> GetReadingsAsync(DateTime fromDate, string connStrKey);
        Task<long?> GetLogicalCoresAsync(DateTime fromDate, string connStrKey);
        Task<long?> GetMaxFrequencyAsync(DateTime fromDate, string connStrKey);
        Task<string?> GetNameAsync(DateTime fromDate, string connStrKey);
        Task<long?> GetPhysicalCoresAsync(DateTime fromDate, string connStrKey);
    }
}