using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;

namespace DataLibrary.Mappers
{
    internal interface ILogEntryMapper
    {
        List<LogEntry> Map(IEnumerable<LOGGING> input);
        LogEntry Map(LOGGING input);
    }
}