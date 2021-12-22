using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class HEALTH_REPORT
    {
        public int? ROW_NO { get; set; }
        public int? MONITOR_NO { get; set; }
        public int? EXECUTION_ID { get; set; }
        public string? REPORT_TYPE { get; set; }
        public string? REPORT_KEY { get; set; }
        public string? REPORT_STRING_VALUE { get; set; }
        public long? REPORT_NUMERIC_VALUE { get; set; }
        public string? REPORT_VALUE_TYPE { get; set; }
        public string? REPORT_VALUE_HUMAN { get; set; }
        public DateTime? LOG_TIME { get; set; }
    }
}
