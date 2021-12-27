using DataLibrary.Helpers;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class AuditLogInfoDAO
    {
        private readonly IConfigHelper _configHelper;

        public AuditLogInfoDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<AuditLogInfo> Get(string connectionStringKey, DateTime fromDate)
        {
            string connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var output = (from entry in db.AUDIT_LOGINFO
                          join type in db.AUDIT_LOGINFO_TYPE
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
