using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public interface IMemoryData
    {
        List<Reading> GetReadingsSinceDate(DateTime fromDate, string connStrKey);
        bool TryGetUpdatedTotal(DateTime fromDate, out long total, string connStrKey);
    }
}