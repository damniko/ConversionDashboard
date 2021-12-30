using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface ICpuData
    {
        List<Reading> GetReadingsSince(DateTime fromDate, string connStrKey);
        bool TryGetUpdatedLogicalCores(DateTime fromDate, out long logicalCores, string connStrKey);
        bool TryGetUpdatedMaxFrequency(DateTime fromDate, out long maxFrequency, string connStrKey);
        bool TryGetUpdatedName(DateTime fromDate, out string name, string connStrKey);
        bool TryGetUpdatedPhysicalCores(DateTime fromDate, out long physicalCores, string connStrKey);
    }
}