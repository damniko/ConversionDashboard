﻿using DataLibrary.Helpers;
using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class HealthReportDAO
    {
        private readonly IConfigHelper _configHelper;

        public HealthReportDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        // get all entries ordered by log time
        // make subportion of these entries (log time > fromdate)
        // for memory - for each mofo, find the last MEMORY_INIT entry that is older than the logtime, and calculate its value

        public HealthReport Get(string connectionStringKey, DateTime fromDate)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            HealthReport output = new();

            List<HEALTH_REPORT> allEntries;
            using (var db = new DefaultDbContext(options))
            {
                allEntries = db.HEALTH_REPORT.OrderBy(x => x.LOG_TIME).ToList();
            }

            if (allEntries.Any() == false)
            {
                return new();
            }

            var entries = allEntries.Where(e => e.LOG_TIME > fromDate).ToList();

            string hostName = allEntries.FindLast(e => e.REPORT_TYPE == "INIT" && e.REPORT_KEY == "Hostname")?
                .REPORT_STRING_VALUE ?? string.Empty;
            string monitorName = allEntries.FindLast(e => e.REPORT_TYPE == "INIT" && e.REPORT_KEY == "Monitor Name")?
                .REPORT_STRING_VALUE ?? string.Empty;

            string cpuName = allEntries.FindLast(e => e.REPORT_TYPE == "CPU_INIT" && e.REPORT_KEY == "CPU Name")?
                .REPORT_STRING_VALUE ?? string.Empty;
            long cpuMaxFreq = allEntries.FindLast(e => e.REPORT_TYPE == "CPU_INIT" && e.REPORT_KEY == "CPU Max frequency")?
                .REPORT_NUMERIC_VALUE.GetValueOrDefault() ?? 0;
            long cpuLogicalCores = allEntries.FindLast(e => e.REPORT_TYPE == "CPU_INIT" && e.REPORT_KEY == "LogicalCores")?
                .REPORT_NUMERIC_VALUE.GetValueOrDefault() ?? 0;
            long cpuPhysicalCores = allEntries.FindLast(e => e.REPORT_TYPE == "CPU_INIT" && e.REPORT_KEY == "PhysicalCores")?
                .REPORT_NUMERIC_VALUE.GetValueOrDefault() ?? 0;
            var cpuReadings = entries
                .Where(e => e.REPORT_TYPE == "CPU" && e.REPORT_KEY == "LOAD")
                .Select(e => new Reading
                {
                    Date = e.LOG_TIME.GetValueOrDefault(),
                    Value = e.REPORT_NUMERIC_VALUE.GetValueOrDefault() / 100.0d,
                });

            long memoryTotal = allEntries.FindLast(e => e.REPORT_KEY == "TOTAL").REPORT_NUMERIC_VALUE.Value;
            var memoryEntries = entries
                .Where(e => e.REPORT_TYPE == "MEMORY" && e.REPORT_KEY == "AVAILABLE");
            List<Reading> memoryReadings = new();
            foreach (var entry in memoryEntries)
            {
                long total = allEntries.FindLast(x => x.LOG_TIME < entry.LOG_TIME && x.REPORT_KEY == "TOTAL")!.REPORT_NUMERIC_VALUE!.Value;
                Reading reading = new()
                {
                    Date = entry.LOG_TIME!.Value,
                    Value = 1 - (entry.REPORT_NUMERIC_VALUE!.Value / Convert.ToDouble(total)),
                };
                memoryReadings.Add(reading);
            }

            return new HealthReport
            {
                HostName = hostName,
                MonitorName = monitorName,
                Cpu = new()
                {
                    Name = cpuName,
                    MaxFrequency = cpuMaxFreq,
                    LogicalCores = cpuLogicalCores,
                    PhysicalCores = cpuPhysicalCores,
                    Readings = cpuReadings.ToList()
                },
                Memory = new()
                {
                    Total = memoryTotal,
                    Readings = memoryReadings
                }
            };
        }
    }
}
