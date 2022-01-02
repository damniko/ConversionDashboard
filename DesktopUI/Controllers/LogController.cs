using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLibrary.DataAccess.Interfaces;
using DesktopUI.Models;

namespace DesktopUI.Controllers
{
    public class LogController
    {
        private readonly ILogData _logData;
        private readonly IMapper _mapper;

        public LogController(ILogData logData ,IMapper mapper)
        {
            _logData = logData;
            _mapper = mapper;
        }

        public List<LogEntryDto> GetLogEntries(DateTime fromDate)
        {
            // TODO - add conn string to settings
            var logEntries = _logData.GetLogEntries(fromDate, "Default");
            
            List<LogEntryDto> result = _mapper.Map<List<LogEntryDto>>(logEntries);

            return result;
        }
    }
}
