using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IExecutionData
    {
        List<Execution> GetExecutionsSinceDate(DateTime fromDate, string connStrKey);
    }
}