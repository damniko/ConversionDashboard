using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IExecutionData
    {
        Task<List<Execution>> GetSinceAsync(DateTime fromDate, string connStrKey);
        Task<List<Execution>> GetAllAsync(string connStrKey);
    }
}