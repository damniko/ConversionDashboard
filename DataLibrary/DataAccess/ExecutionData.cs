using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class ExecutionData : IExecutionData
    {
        private readonly IDataAccess _db;

        public ExecutionData(IDataAccess db)
        {
            _db = db;
        }

        public List<Execution> GetExecutionsSinceDate(DateTime fromDate, string connStrKey)
        {
            var contextData = _db.GetLoggingContextTbl(connStrKey).ToList();
            var executionData = _db.GetExecutionTbl(connStrKey);

            var output = (from e in executionData
                          where e.CREATED > fromDate
                          orderby e.CREATED
                          select new Execution
                          {
                              Id = e.EXECUTION_ID.GetValueOrDefault(),
                              Uuid = Guid.Parse(e.EXECUTION_UUID ?? string.Empty),
                              StartTime = e.CREATED.GetValueOrDefault(),
                          }).ToList();

            // Try to assign end-times and context ID dictionaries to each execution
            foreach (var execution in output)
            {
                var prev = output.FirstOrDefault(e => e.Id == execution.Id - 1);
                if (prev != null)
                {
                    prev.EndTime = execution.StartTime;
                }
                var dict = (from c in contextData
                            where c.EXECUTION_ID == execution.Id
                            select c)
                            .ToDictionary(c => c.CONTEXT_ID, c => c.CONTEXT);
                execution.ContextDict = dict;
            }
            return output;
        }
    }
}
