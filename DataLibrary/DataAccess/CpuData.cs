using DataLibrary.Internal;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class CpuData : ICpuData
    {
        private readonly IDataAccess _db;

        public CpuData(IDataAccess db)
        {
            _db = db;
        }

        public List<Reading> GetReadingsSinceDate(DateTime fromDate, string connStrKey)
        {
            var output = from e in _db.GetHealthReportTbl(connStrKey)
                         where e.LOG_TIME > fromDate
                         where e.REPORT_TYPE == "CPU" && e.REPORT_KEY == "LOAD"
                         where e.LOG_TIME.HasValue && e.REPORT_NUMERIC_VALUE.HasValue
                         orderby e.LOG_TIME
                         select new Reading
                         {
                             Date = e.LOG_TIME!.Value,
                             Value = e.REPORT_NUMERIC_VALUE!.Value / 100.0d
                         };

            return output.ToList();
        }

        public bool TryGetUpdatedName(DateTime fromDate, out string name, string connStrKey)
        {
            name = "";
            bool wasSuccessful = false;

            var entry = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault(x => x.REPORT_TYPE == "CPU_INIT" && x.REPORT_KEY == "CPU Name");

            if (entry?.REPORT_STRING_VALUE != null)
            {
                name = entry.REPORT_STRING_VALUE!;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        public bool TryGetUpdatedLogicalCores(DateTime fromDate, out long logicalCores, string connStrKey)
        {
            logicalCores = 0;
            bool wasSuccessful = false;

            var entry = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault(x => x.REPORT_TYPE == "CPU_INIT" && x.REPORT_KEY == "LogicalCores");

            if (entry?.REPORT_NUMERIC_VALUE != null)
            {
                logicalCores = entry.REPORT_NUMERIC_VALUE.Value;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        public bool TryGetUpdatedPhysicalCores(DateTime fromDate, out long physicalCores, string connStrKey)
        {
            physicalCores = 0;
            bool wasSuccessful = false;

            var entry = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault(x => x.REPORT_TYPE == "CPU_INIT" && x.REPORT_KEY == "PhysicalCores");

            if (entry?.REPORT_NUMERIC_VALUE != null)
            {
                physicalCores = entry.REPORT_NUMERIC_VALUE.Value;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }

        public bool TryGetUpdatedMaxFrequency(DateTime fromDate, out long maxFrequency, string connStrKey)
        {
            maxFrequency = 0;
            bool wasSuccessful = false;

            var entry = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault(x => x.REPORT_TYPE == "CPU_INIT" && x.REPORT_KEY == "CPU Max frequency");

            if (entry?.REPORT_NUMERIC_VALUE != null)
            {
                maxFrequency = entry.REPORT_NUMERIC_VALUE.Value;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }
    }
}
