using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class StatementData : IStatementData
    {
        private readonly IDataAccess _db;

        public StatementData(IDataAccess db)
        {
            _db = db;
        }

        public async Task<List<StatementTable>> GetAsync(DateTime fromDate, string connStrKey)
        {
            var output = (from e in await _db.GetStatementTablesAsync(connStrKey)
                          where e.CREATED > fromDate
                          orderby e.CREATED
                          select new StatementTable
                          {
                              Date = e.CREATED,
                              Manager = e.MGR,
                              Identifier = e.IDENTIFIER,
                              IdentifierShort = e.IDENTIFIER_SHORT,
                              Schema = e.SCHEMA_NAME_FULL,
                              SchemaShort = e.SCHEMA_NAME,
                              Table = e.TABLE_NAME
                          }).ToList();

            return output;
        }
    }
}
