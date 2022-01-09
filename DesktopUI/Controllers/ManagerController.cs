using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataLibrary.DataAccess.Interfaces;
using DesktopUI.Models;

namespace DesktopUI.Controllers;

public class ManagerController
{
    private readonly IManagerData _managerData;
    private readonly IMapper _mapper;

    public ManagerController(IManagerData managerData, IMapper mapper)
    {
        _managerData = managerData;
        _mapper = mapper;
    }

    public async Task<List<ManagerDto>> GetSince(DateTime fromDate)
    {
        var entries = await _managerData.GetSinceAsync(fromDate, "Default");
        var result = _mapper.Map<List<ManagerDto>>(entries);
        return result;
    }
}