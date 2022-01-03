using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IReconciliationData
    {
        List<Reconciliation> GetSince(DateTime fromDate, string connStrKey);
    }
}