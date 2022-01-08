using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces;

public interface IStatementData
{
    Task<List<StatementTable>> GetAsync(DateTime fromDate, string connStrKey);
}