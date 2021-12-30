using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Internal;
using DataLibrary.Models;
using Microsoft.Extensions.Logging;

namespace DataLibrary.DataAccess
{
    public class MemoryData : IMemoryData
    {
        private readonly IDataAccess _db;

        public MemoryData(IDataAccess db, ILogger<MemoryData> logger)
        {
            _db = db;
        }

        public List<Reading> GetReadingsSinceDate(DateTime fromDate, string connStrKey)
        {
            var output = new List<Reading>();
            var allEntries = _db.GetHealthReportTbl(connStrKey)
                .OrderBy(x => x.LOG_TIME);

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
                    // TODO - Log this
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

        public bool TryGetUpdatedTotal(DateTime fromDate, out long total, string connStrKey)
        {
            total = 0;
            bool wasSuccessful = false;

            var entry = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .Where(x => x.REPORT_TYPE == "MEMORY_INIT" && x.REPORT_KEY == "TOTAL")
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault();

            if (entry?.REPORT_NUMERIC_VALUE != null)
            {
                total = entry.REPORT_NUMERIC_VALUE.Value;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }
    }
}
