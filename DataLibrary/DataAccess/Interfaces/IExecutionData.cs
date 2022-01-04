using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IExecutionData
    {
        List<Execution> GetSince(DateTime fromDate, string connStrKey);
        List<Execution> GetAll(string connStrKey);
    }
}