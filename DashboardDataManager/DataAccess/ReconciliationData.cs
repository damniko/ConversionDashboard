using DataLibrary.Internal;
using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class ReconciliationData
    {
        internal IDataAccess Db { get; } = new EfDataAccess();

        public List<Reconciliation> GetReconciliationsSinceDate(DateTime fromDate, string connStrKey)
        {
            var output = (from r in Db.GetAfstemningTbl(connStrKey)
                          where r.AFSTEMTDATO > fromDate
                          orderby r.AFSTEMTDATO
                          select new Reconciliation
                          {
                              Id = Guid.Parse(r.ID),
                              Date = r.AFSTEMTDATO,
                              Manager = r.MANAGER ?? string.Empty,
                              Context = r.CONTEXT,
                              Description = r.DESCRIPTION,
                              Result = GetReconciliationResult(r),
                              RunJob = r.RUN_JOB,
                              ToolkitId = r.TOOLKIT_ID,
                              StartTime = r.START_TIME,
                              EndTime = r.END_TIME,
                              CustomCount = r.CUSTOMANTAL,
                              CustomSqlCost = r.CUSTOM_SQL_COST,
                              CustomSqlString = r.CUSTOM_SQL,
                              CustomSqlTime = r.CUSTOM_SQL_TIME,
                              SrcCount = r.SRCANTAL,
                              SrcSqlCost = r.SRC_SQL_COST,
                              SrcSqlString = r.SRC_SQL,
                              SrcSqlTime = r.SRC_SQL_TIME,
                              DstCount = r.DSTANTAL,
                              DstSqlCost = r.DST_SQL_COST,
                              DstSqlString = r.DST_SQL,
                              DstSqlTime = r.DST_SQL_TIME
                          })
                          .ToList();

            return output;
        }

        private static ReconciliationResult GetReconciliationResult(AFSTEMNING input)
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
