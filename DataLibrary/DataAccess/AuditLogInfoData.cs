using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class AuditLogInfoData : IAuditLogInfoData
    {
        private readonly IDataAccess _db;

        public AuditLogInfoData(IDataAccess db)
        {
            _db = db;
        }

        public List<AuditLogInfo> GetAuditLogInfoSinceDate(DateTime fromDate, string connStrKey)
        {
            var logInfoData = _db.GetAuditLogInfoTbl(connStrKey);
            var typesData = _db.GetAuditLogInfoTypesTbl(connStrKey);

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
