using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface IReconciliationData
    {
        List<Reconciliation> GetReconciliationsSinceDate(DateTime fromDate, string connStrKey);
    }
}