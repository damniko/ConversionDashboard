using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface IAuditLogErrorData
{
    Task<List<AuditLogError>> GetAsync(DateTime fromDate, string connStrKey);
}