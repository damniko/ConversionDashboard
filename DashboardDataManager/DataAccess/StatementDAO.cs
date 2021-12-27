using DataLibrary.Helpers;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class StatementDAO
    {
        private readonly IConfigHelper _configHelper;

        public StatementDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<StatementTable> Get(string connectionStringKey, DateTime fromDate)
        {
            string connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var output = db.STATEMENT_TABLES
                .Where(x => x.CREATED > fromDate)
                .OrderBy(x => x.CREATED)
                .Select(x => new StatementTable
                {
                    Date = x.CREATED,
                    Manager = x.MGR,
                    Identifier = x.IDENTIFIER,
                    IdentifierShort = x.IDENTIFIER_SHORT,
                    Schema = x.SCHEMA_NAME_FULL,
                    SchemaShort = x.SCHEMA_NAME,
                    Table = x.TABLE_NAME
                })
                .ToList();

            return output;
        }
    }
}
