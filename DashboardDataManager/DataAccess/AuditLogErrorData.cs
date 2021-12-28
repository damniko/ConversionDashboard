using DataLibrary.Internal;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class AuditLogErrorData
    {
        internal IDataAccess Db { get; } = new EfDataAccess();
        
        public List<AuditLogError> GetAuditLogErrorsSinceDate(DateTime fromDate, string connStrKey)
        {
            var data = Db.GetAuditLogErrorTbl(connStrKey);
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
