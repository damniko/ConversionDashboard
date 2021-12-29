using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Internal;
using DataLibrary.Models;
using Microsoft.Extensions.Configuration;

namespace DataLibrary.DataAccess
{
    public class AuditLogErrorData : IAuditLogErrorData
    {
        private readonly IDataAccess _db;

        public AuditLogErrorData(IDataAccess db)
        {
            _db = db;
        }

        public List<AuditLogError> GetAuditLogErrorsSinceDate(DateTime fromDate, string connStrKey)
        {
            var data = _db.GetAuditLogErrorTbl(connStrKey);
            
            var output = (from entry in data
                          where entry.CREATED > fromDate
                          select new AuditLogError
                          {
                              Id = Guid.Parse(entry.ID),
                              Date = entry.CREATED,
                              Manager = entry.MGRNAME,
                              SourceRows = entry.SOURCEROWS,
                              Message = entry.MESSAGE
                          }).ToList();

            return output;
        }
    }
}
