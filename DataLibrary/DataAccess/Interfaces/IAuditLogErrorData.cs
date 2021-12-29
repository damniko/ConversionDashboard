using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface IAuditLogErrorData
{
    List<AuditLogError> GetAuditLogErrorsSinceDate(DateTime fromDate, string connStrKey);
}