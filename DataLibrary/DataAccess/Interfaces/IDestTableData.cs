using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IDestTableData
    {
        List<DestTable> GetDestTablesForManager(string manager, string connStrKey);
    }
}