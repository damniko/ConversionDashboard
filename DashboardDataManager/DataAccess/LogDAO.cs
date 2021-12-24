using Microsoft.EntityFrameworkCore;
using DataLibrary.Helpers;
using DataLibrary.Models;
using DataLibrary.Mappers;
using DataLibrary.Internal.EntityModels;

namespace DataLibrary.DataAccess
{
    public class LogDAO
    {
        private readonly IConfigHelper _configHelper;

        public LogDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<LogEntry> Get(string connectionStringKey, DateTime fromDate)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var output = (from m in db.LOGGING
                          where m.CREATED > fromDate
                          orderby m.CREATED
                          join context in db.LOGGING_CONTEXT
                          on new { m.CONTEXT_ID, m.EXECUTION_ID } equals new { CONTEXT_ID = (long?)context.CONTEXT_ID, EXECUTION_ID = (long?)context.EXECUTION_ID }
                          select new LogEntry
                          {
                              ContextId = m.CONTEXT_ID.GetValueOrDefault(),
                              Created = m.CREATED.GetValueOrDefault(),
                              ExecutionId = m.EXECUTION_ID.GetValueOrDefault(),
                              Level = GetLogLevel(m),
                              Message = m.LOG_MESSAGE ?? string.Empty,
                              Manager = context.CONTEXT
                          })
                          .ToList();

            return output;
        }

        private static LogLevel GetLogLevel(LOGGING input)
        {
            switch (input.LOG_LEVEL)
            {
                case "INFO":
                    return LogLevel.Info;
                case "WARN":
                    return LogLevel.Warn;
                case "ERROR" when input.LOG_MESSAGE!.Contains("Afstemning"):
                    return LogLevel.Error | LogLevel.Reconciliation;
                case "ERROR":
                    return LogLevel.Error;
                case "FATAL":
                    return LogLevel.Fatal;
                case "AFSTEMNING":
                    return LogLevel.Reconciliation;
                default:
                    throw new ArgumentException($"The LogLevel for { input.LOG_LEVEL } could not be found.");
            }
        }
    }
}
