using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface IAuditLogInfoData
{
    Task<List<AuditLogInfo>> GetAuditLogInfoAsync(DateTime fromDate, string connStrKey);
}