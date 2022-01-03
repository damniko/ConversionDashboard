using System;
using System.Collections.Generic;
using AutoMapper;
using DataLibrary.DataAccess.Interfaces;
using DesktopUI.Models;

namespace DesktopUI.Controllers
{
    public class ReconciliationController
    {
        private readonly IReconciliationData _reconciliationData;
        private readonly IMapper _mapper;

        public ReconciliationController(IReconciliationData reconciliationData, IMapper mapper)
        {
            _reconciliationData = reconciliationData;
            _mapper = mapper;
        }

        public List<ReconciliationDto> GetReconciliations(DateTime fromDate)
        {
            var entries = _reconciliationData.GetSince(fromDate, "Default");

            var result = _mapper.Map<List<ReconciliationDto>>(entries);

            return result;
        }
    }
}
