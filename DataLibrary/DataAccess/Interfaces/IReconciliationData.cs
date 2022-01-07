using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IReconciliationData
    {
        Task<List<Reconciliation>> GetAsync(DateTime fromDate, string connStrKey);
    }
}