using DataLibrary.Internal;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class AuditLogInfoData
    {
        internal IDataAccess Db { get; } = new EfDataAccess();
        
        public List<AuditLogInfo> GetAuditLogInfoSinceDate(DateTime fromDate, string connStrKey)
        {
            var logInfoData = Db.GetAuditLogInfoTbl(connStrKey);
            var typesData = Db.GetAuditLogInfoTypesTbl(connStrKey);
            
            var output = (from entry in logInfoData
                          join type in typesData
                          on entry.BUSINESSID equals type.ID
                          where entry.CREATED > fromDate
                          select new AuditLogInfo
                          {
                              Id = Guid.Parse(entry.ID),
                              Date = entry.CREATED,
                              BusinessId = type.ID,
                              Manager = entry.MGRNAME,
                              ErrorDescription = type.DESCRIPTION,
                              Message = entry.MESSAGE,
                              CprNumber = entry.CPRNR,
                              ReconciliationValue = entry.RECONCILIATIONVALUE
                          }).ToList();

            return output;
        }
    }
}
