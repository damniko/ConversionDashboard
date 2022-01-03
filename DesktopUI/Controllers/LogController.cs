using System;
using System.Collections.Generic;
using AutoMapper;
using DataLibrary.DataAccess.Interfaces;
using DesktopUI.Models;

namespace DesktopUI.Controllers
{
    public class LogController
    {
        private readonly ILogData _logData;
        private readonly IMapper _mapper;

        public LogController(ILogData logData, IMapper mapper)
        {
            _logData = logData;
            _mapper = mapper;
        }

        public List<LogEntryDto> GetLogEntries(DateTime fromDate)
        {
            // TODO - make the connection string changeable through settings
            var logEntries = _logData.GetSince(fromDate, "Default");
            
            List<LogEntryDto> result = _mapper.Map<List<LogEntryDto>>(logEntries);

            return result;
        }
    }
}
