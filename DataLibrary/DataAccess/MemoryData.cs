using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;
using Microsoft.Extensions.Logging;

namespace DataLibrary.DataAccess;

public class MemoryData : IMemoryData
{
    private readonly IDataAccess _db;
    private readonly ILogger<MemoryData> _logger;

    public MemoryData(IDataAccess db, ILogger<MemoryData> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Reading>> GetReadingsAsync(DateTime fromDate, string connStrKey)
    {
        var output = new List<Reading>();
        var allEntries = from e in await _db.GetHealthReportAsync(connStrKey)
            orderby e.LOG_TIME
            select e;

        var memoryEntries = from e in allEntries
            where e.LOG_TIME > fromDate
            where e.REPORT_TYPE == "MEMORY"
            where e.LOG_TIME.HasValue && e.REPORT_NUMERIC_VALUE.HasValue
            orderby e.LOG_TIME
            select e;

        foreach (var entry in memoryEntries)
        {
            var totalEntry = allEntries.LastOrDefault(e => e.REPORT_TYPE == "MEMORY_INIT" && e.LOG_TIME < entry.LOG_TIME!.Value);
            if (totalEntry is null)
            {
                _logger.LogWarning("Could not find a HEALTH_REPORT entry with [REPORT_TYPE]=MEMORY_INIT and [REPORT_KEY]=TOTAL with a [LOG_TIME] earlier than {LogTime}. Will skip memory reading.", entry.LOG_TIME);
                continue;
            }
            var reading = new MemoryReading
            {
                Date = entry.LOG_TIME!.Value,
                Available = entry.REPORT_NUMERIC_VALUE!.Value,
                Total = totalEntry.REPORT_NUMERIC_VALUE!.Value
            };
            output.Add(reading);
        }
        return output;
    }

    public async Task<long?> GetTotalAsync(DateTime fromDate, string connStrKey)
    {
        var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
            where e.LOG_TIME > fromDate
            where e.REPORT_TYPE == "MEMORY_INIT" && e.REPORT_KEY == "TOTAL"
            orderby e.LOG_TIME
            select e).LastOrDefault();

        return entry?.REPORT_NUMERIC_VALUE;
    }
}