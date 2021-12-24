using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;

namespace DataLibrary.Mappers
{
    [Obsolete]
    internal class LogEntryMapper : ILogEntryMapper
    {
        public LogEntry Map(LOGGING input)
        {
            LogEntry output = new()
            {
                ContextId = input.CONTEXT_ID.GetValueOrDefault(),
                ExecutionId = input.EXECUTION_ID.GetValueOrDefault(),
                Created = input.CREATED.GetValueOrDefault(),
                Level = GetLogLevel(input),
                Message = input.LOG_MESSAGE ?? ""
            };
            return output;
        }

        public List<LogEntry> Map(IEnumerable<LOGGING> input)
        {
            return input.Select(Map).ToList();
        }

        private LogLevel GetLogLevel(LOGGING input)
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
