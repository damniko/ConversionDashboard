using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;

namespace DataLibrary.Internal.Mappers
{
    internal interface IReconciliationMapper
    {
        Reconciliation Map(AFSTEMNING input);
        List<Reconciliation> Map(IEnumerable<AFSTEMNING> input);
    }
}