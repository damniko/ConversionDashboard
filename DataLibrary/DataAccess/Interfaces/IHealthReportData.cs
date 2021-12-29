using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IHealthReportData
    {
        HealthReport GetHealthReportFromDate(DateTime fromDate, string connStrKey);
    }
}