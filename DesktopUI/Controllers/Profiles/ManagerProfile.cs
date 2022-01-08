using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DataLibrary.Models;
using DesktopUI.Models;

namespace DesktopUI.Controllers.Profiles;

public class ManagerProfile : Profile
{
    public ManagerProfile()
    {
        CreateMap<Manager, ManagerDto>();
    }
}