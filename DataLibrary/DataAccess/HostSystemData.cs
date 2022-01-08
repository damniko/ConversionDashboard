using DataLibrary.DataAccess.Interfaces;

namespace DataLibrary.DataAccess;

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

    public async Task<string?> GetHostNameAsync(DateTime fromDate, string connStrKey)
    {
        var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
            where e.LOG_TIME > fromDate
            where e.REPORT_TYPE == _targetReportType && e.REPORT_KEY == _hostNameKey
            where string.IsNullOrEmpty(e.REPORT_STRING_VALUE) is false
            orderby e.LOG_TIME
            select e).LastOrDefault();

        return entry?.REPORT_STRING_VALUE;
    }

    public async Task<string?> GetMonitorNameAsync(DateTime fromDate, string connStrKey)
    {
        var entry = (from e in await _db.GetHealthReportAsync(connStrKey)
            where e.LOG_TIME > fromDate
            where e.REPORT_TYPE == _targetReportType && e.REPORT_KEY == _monitorNameKey
            where string.IsNullOrEmpty(e.REPORT_STRING_VALUE) is false
            orderby e.LOG_TIME
            select e).LastOrDefault();

        return entry?.REPORT_STRING_VALUE;
    }
}