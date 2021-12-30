using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IAuditLogInfoData
    {
        List<AuditLogInfo> GetAuditLogInfoSinceDate(DateTime fromDate, string connStrKey);
    }
}