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

        public List<StatementTable> GetStatementTablesSinceDate(DateTime fromDate, string connStrKey)
        {
            var output = _db.GetStatementTablesTbl(connStrKey)
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
