using DataLibrary.Models;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models.Database;

namespace DataLibrary.DataAccess;

public class LogData : ILogData
{
    private readonly IDataAccess _db;

    public LogData(IDataAccess db)
    {
        _db = db;
    }

    public async Task<List<LogEntry>> GetSinceAsync(DateTime fromDate, string connStrKey)
    {
        var contextData = await _db.GetLoggingContextAsync(connStrKey);
        var loggingData = await _db.GetLoggingAsync(connStrKey);

        var output = (from m in loggingData
                where m.CREATED > fromDate
                orderby m.CREATED
                join context in contextData
                    on new { m.CONTEXT_ID, m.EXECUTION_ID } equals new { CONTEXT_ID = (long?)context.CONTEXT_ID, context.EXECUTION_ID }
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