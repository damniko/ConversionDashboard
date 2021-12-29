using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IStatementData
    {
        List<StatementTable> GetStatementTablesSinceDate(DateTime fromDate, string connStrKey);
    }
}