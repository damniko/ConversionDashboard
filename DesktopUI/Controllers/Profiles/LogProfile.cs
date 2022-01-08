using AutoMapper;
using DataLibrary.Models;
using DesktopUI.Models;

namespace DesktopUI.Controllers.Profiles;

internal class LogProfile : Profile
{
    public LogProfile()
    {
        CreateMap<LogEntry, LogEntryDto>();
    }
}