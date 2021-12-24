using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;

namespace DataLibrary.Internal.Mappers
{
    internal class ReconciliationMapper : IReconciliationMapper
    {
        [Obsolete]
        public Reconciliation Map(AFSTEMNING input)
        {
            Reconciliation output = new()
            {
                Id = Guid.Parse(input.ID),
                Date = input.AFSTEMTDATO,
                Description = input.DESCRIPTION,
                Result = GetReconciliationResult(input),
                RunJob = input.RUN_JOB,
                ToolkitId = input.TOOLKIT_ID,
                Manager = input.MANAGER ?? string.Empty,
                Context = input.CONTEXT,
                CustomCount = input.CUSTOMANTAL,
                CustomSqlCost = input.CUSTOM_SQL_COST,
                CustomSqlString = input.CUSTOM_SQL,
                CustomSqlTime = input.CUSTOM_SQL_TIME,
                DstCount = input.DSTANTAL,
                DstSqlCost = input.CUSTOM_SQL_COST,
                DstSqlString = input.CUSTOM_SQL,
                DstSqlTime = input.DST_SQL_TIME,
                SrcCount = input.SRCANTAL,
                SrcSqlCost = input.SRC_SQL_COST,
                SrcSqlString = input.SRC_SQL,
                SrcSqlTime = input.SRC_SQL_TIME,
                StartTime = input.START_TIME.GetValueOrDefault(),
                EndTime = input.END_TIME.GetValueOrDefault(),
            };
            return output;
        }

        public List<Reconciliation> Map(IEnumerable<AFSTEMNING> input)
        {
            return input.Select(Map).ToList();
        }

        private ReconciliationResult GetReconciliationResult(AFSTEMNING input)
        {
            switch (input.AFSTEMRESULTAT)
            {
                case "OK":
                    return ReconciliationResult.Ok;
                case "DISABLED":
                    return ReconciliationResult.Disabled;
                case "FAILED":
                    return ReconciliationResult.Failed;
                case "FAIL MISMATCH":
                    return ReconciliationResult.FailMismatch;
                default:
                    throw new ArgumentException($"The ReconciliationResult for { input.AFSTEMRESULTAT } could not be found.");
            }
        }
    }
}
