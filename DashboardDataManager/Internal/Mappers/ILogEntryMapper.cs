using DataAccessLibrary.Internal.EntityModels;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Mappers
{
    internal interface ILogEntryMapper
    {
        List<LogEntry> Map(IEnumerable<LOGGING> input);
        LogEntry Map(LOGGING input);
    }
}