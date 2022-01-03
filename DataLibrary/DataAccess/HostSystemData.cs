using DataLibrary.DataAccess.Interfaces;

namespace DataLibrary.DataAccess
{
    public class HostSystemData : IHostSystemData
    {
        private readonly IDataAccess _db;

        internal readonly string _targetReportType = "INIT";
        internal readonly string _hostNameKey = "Hostname";
        internal readonly string _monitorNameKey = "Monitor Name";

        public HostSystemData(IDataAccess db)
        {
            _db = db;
        }

        public bool TryGetUpdatedHostName(DateTime fromDate, out string hostname, string connStrKey)
        {
            hostname = string.Empty;
            bool success = false;

            var entry = (from e in _db.GetHealthReportTbl(connStrKey)
                         where e.LOG_TIME > fromDate
                         where e.REPORT_TYPE == _targetReportType && e.REPORT_KEY == _hostNameKey
                         where string.IsNullOrEmpty(e.REPORT_STRING_VALUE) is false
                         orderby e.LOG_TIME
                         select e).LastOrDefault();

            if (entry != null)
            {
                success = true;
                hostname = entry.REPORT_STRING_VALUE!;
            }

            return success;
        }

        public bool TryGetUpdatedMonitorName(DateTime fromDate, out string monitorName, string connStrKey)
        {
            monitorName = string.Empty;
            bool success = false;

            var entry = (from e in _db.GetHealthReportTbl(connStrKey)
                         where e.LOG_TIME > fromDate
                         where e.REPORT_TYPE == _targetReportType && e.REPORT_KEY == _monitorNameKey
                         where string.IsNullOrEmpty(e.REPORT_STRING_VALUE) is false
                         orderby e.LOG_TIME
                         select e).LastOrDefault();

            if (entry != null)
            {
                success = true;
                monitorName = entry.REPORT_STRING_VALUE!;
            }

            return success;
        }
    }
}
