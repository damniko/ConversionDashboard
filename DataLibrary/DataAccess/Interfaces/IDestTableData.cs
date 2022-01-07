using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IDestTableData
    {
        Task<List<DestTable>> GetDestTablesForManagerAsync(string manager, string connStrKey);
    }
}