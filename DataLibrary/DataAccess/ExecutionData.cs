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

        public List<Execution> GetSince(DateTime fromDate, string connStrKey)
        {
            var executionData = _db.GetExecutionTbl(connStrKey);

            var entries = (from e in executionData
                          where e.CREATED > fromDate
                          orderby e.CREATED
                          select new Execution
                          {
                              Id = e.EXECUTION_ID.GetValueOrDefault(),
                              Uuid = Guid.Parse(e.EXECUTION_UUID ?? string.Empty),
                              StartTime = e.CREATED.GetValueOrDefault(),
                          }).ToList();

            var output = AssignEndTimesAndContextDictionaries(entries, connStrKey);
            
            return output;
        }

        public List<Execution> GetAll(string connStrKey) 
            => GetSince(System.Data.SqlTypes.SqlDateTime.MinValue.Value, connStrKey);
    
        private List<Execution> AssignEndTimesAndContextDictionaries(List<Execution> entries, string connStrKey)
        {
            var contextData = _db.GetLoggingContextTbl(connStrKey).ToList();
            foreach (var execution in entries)
            {
                var prev = entries.FirstOrDefault(e => e.Id == execution.Id - 1);
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
            return entries;
        }
    }
}
