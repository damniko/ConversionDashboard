using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLibrary.DataAccess.Interfaces;
using DesktopUI.Models;

namespace DesktopUI.Controllers;

public class ExecutionController
{
    private readonly IExecutionData _executionData;
    private readonly IMapper _mapper;

    public ExecutionController(IExecutionData executionData, IMapper mapper)
    {
        _executionData = executionData;
        _mapper = mapper;
    }

    public async Task<List<ExecutionDto>> GetSinceAsync(DateTime fromDate)
    {
        var entries = await _executionData.GetSinceAsync(fromDate, "Default");
        var result = _mapper.Map<List<ExecutionDto>>(entries);
        return result;
    }

    public async Task<List<ExecutionDto>> GetAllAsync()
    {
        var entries = await _executionData.GetAllAsync("Default");
        var result = _mapper.Map<List<ExecutionDto>>(entries);
        return result;
    }
}