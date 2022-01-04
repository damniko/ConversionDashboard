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
    public class ExecutionController
    {
        private readonly IExecutionData _executionData;
        private readonly IMapper _mapper;

        public ExecutionController(IExecutionData executionData, IMapper mapper)
        {
            _executionData = executionData;
            _mapper = mapper;
        }

        public List<ExecutionDto> GetExecutions(DateTime fromDate)
        {
            var entries = _executionData.GetSince(fromDate, "Default");
            var result = _mapper.Map<List<ExecutionDto>>(entries);
            return result;
        }

        public List<ExecutionDto> GetAllExecutions()
        {
            var entries = _executionData.GetAll("Default");
            var result = _mapper.Map<List<ExecutionDto>>(entries);
            return result;
        }
    }
}
