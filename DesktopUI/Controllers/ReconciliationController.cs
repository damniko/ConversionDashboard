using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<List<ReconciliationDto>> GetSince(DateTime fromDate)
        {
            var entries = await _reconciliationData.GetAsync(fromDate, "Default");
            var result = _mapper.Map<List<ReconciliationDto>>(entries);
            return result;
        }

        public async Task<Dictionary<string, List<ReconciliationDto>>> GetManagerReconciliationDict(DateTime fromDate)
        {
            var result = new Dictionary<string, List<ReconciliationDto>>();
            var entries = await GetSince(fromDate);
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
