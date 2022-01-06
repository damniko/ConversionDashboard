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

        public async Task<List<Reading>> GetReadingsAsync(DateTime fromDate, string connStrKey)
        {
            var output = from e in await _db.GetHealthReportAsync(connStrKey)
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

        public async Task<string?> GetNameAsync(DateTime fromDate, string connStrKey)
        {
            var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
                         where e.REPORT_KEY == _initNameKey && e.LOG_TIME > fromDate
                         orderby e.LOG_TIME
                         select e).LastOrDefault();

            return entry?.REPORT_STRING_VALUE;
        }

        public async Task<long?> GetLogicalCoresAsync(DateTime fromDate, string connStrKey)
        {
            var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
                         where e.REPORT_KEY == _initLogicalCoresKey && e.LOG_TIME > fromDate
                         orderby e.LOG_TIME
                         select e).LastOrDefault();

            return entry?.REPORT_NUMERIC_VALUE;
        }

        public async Task<long?> GetPhysicalCoresAsync(DateTime fromDate, string connStrKey)
        {
            var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
                         where e.REPORT_KEY == _initPhysicalCoresKey && e.LOG_TIME > fromDate
                         orderby e.LOG_TIME
                         select e).LastOrDefault();

            return entry?.REPORT_NUMERIC_VALUE;
        }

        public async Task<long?> GetMaxFrequencyAsync(DateTime fromDate, string connStrKey)
        {
            var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
                         where e.REPORT_KEY == _initMaxFreqKey && e.LOG_TIME > fromDate
                         orderby e.LOG_TIME
                         select e)
                         .LastOrDefault();

            return entry?.REPORT_NUMERIC_VALUE;
        }
    }
}
