using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class CpuData : ICpuData
    {
        private readonly IDataAccess _db;

        // Init entries
        internal readonly string _initReportType = "CPU_INIT";
        internal readonly string _initNameKey = "CPU Name";
        internal readonly string _initLogicalCoresKey = "LogicalCores";
        internal readonly string _initPhysicalCoresKey = "PhysicalCores";
        internal readonly string _initMaxFreqKey = "CPU Max frequency";
        // Reading entries
        internal readonly string _readingReportType = "CPU";
        internal readonly string _loadKey = "LOAD";

        public CpuData(IDataAccess db)
        {
            _db = db;
        }

        public List<Reading> GetReadingsSince(DateTime fromDate, string connStrKey)
        {
            var output = from e in _db.GetHealthReportTbl(connStrKey)
                         where e.LOG_TIME > fromDate
                         where e.REPORT_TYPE == _readingReportType && e.REPORT_KEY == _loadKey
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
                .LastOrDefault(x => x.REPORT_TYPE == _initReportType && x.REPORT_KEY == _initNameKey);

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
                .LastOrDefault(x => x.REPORT_TYPE == _initReportType && x.REPORT_KEY == _initLogicalCoresKey);

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
                .LastOrDefault(x => x.REPORT_TYPE == _initReportType && x.REPORT_KEY == _initPhysicalCoresKey);

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
                .LastOrDefault(x => x.REPORT_TYPE == _initReportType && x.REPORT_KEY == _initMaxFreqKey);

            if (entry?.REPORT_NUMERIC_VALUE != null)
            {
                maxFrequency = entry.REPORT_NUMERIC_VALUE.Value;
                wasSuccessful = true;
            }

            return wasSuccessful;
        }
    }
}
