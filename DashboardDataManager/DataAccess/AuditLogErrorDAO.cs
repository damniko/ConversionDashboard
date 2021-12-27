using DataLibrary.Helpers;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class AuditLogErrorDAO
    {
        private readonly IConfigHelper _configHelper;

        public AuditLogErrorDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<AuditLogError> Get(string connectionStringKey, DateTime fromDate)
        {
            string connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var output = (from entry in db.AUDIT_LOGERROR
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
