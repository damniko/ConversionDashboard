using DataLibrary.Helpers;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class ExecutionDAO
    {
        private readonly IConfigHelper _configHelper;

        public ExecutionDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<Execution> Get(string connectionStringKey, DateTime fromDate)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            // This could probably be simplified with some fingerfærdigheder in LINQ,
            // e.g. by using the 'join into' statement to get the list of manager names
            // from LOGGING_CONTEXT for each EXECUTION_ID
            using var db = new DefaultDbContext(options);

            var output = (from e in db.EXECUTIONS
                         orderby e.CREATED
                         select new Execution
                         {
                             Id = e.EXECUTION_ID.GetValueOrDefault(),
                             Uuid = Guid.Parse(e.EXECUTION_UUID ?? string.Empty),
                             StartTime = e.CREATED.GetValueOrDefault(),
                         }).ToList();

            foreach (var execution in output)
            {
                var prev = output.FirstOrDefault(e => e.Id == execution.Id - 1);
                if (prev != null)
                {
                    prev.EndTime = execution.StartTime;
                }
                var dict = (from c in db.LOGGING_CONTEXT
                            where c.EXECUTION_ID == execution.Id
                            select c)
                            .ToDictionary(c => c.CONTEXT_ID, c => c.CONTEXT);
                execution.ContextDict = dict;
            }
            return output;
        }
    }
}
