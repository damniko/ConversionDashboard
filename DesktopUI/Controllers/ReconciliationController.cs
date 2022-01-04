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

        public Dictionary<string, List<ReconciliationDto>> GetManagerReconciliationDict(DateTime fromDate)
        {
            var result = new Dictionary<string, List<ReconciliationDto>>();
            var entries = GetReconciliations(fromDate);
            foreach (var entry in entries)
            {
                if (result.ContainsKey(entry.Manager))
                {
                    result[entry.Manager].Add(entry);
                }
                else
                {
                    result.Add(entry.Manager, new() { entry });
                }
            }
            return result;
        }

    }
}
