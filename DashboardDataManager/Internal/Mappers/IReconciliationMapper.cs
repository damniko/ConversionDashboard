using DataAccessLibrary.Internal.EntityModels;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Internal.Mappers
{
    internal interface IReconciliationMapper
    {
        Reconciliation Map(AFSTEMNING input);
        List<Reconciliation> Map(IEnumerable<AFSTEMNING> input);
    }
}