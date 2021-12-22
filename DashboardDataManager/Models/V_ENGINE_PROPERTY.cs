using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class V_ENGINE_PROPERTY
    {
        public string? MANAGER { get; set; }
        public string? KEY { get; set; }
        public string? VALUE { get; set; }
        public int? RUN_NO { get; set; }
        public DateTime? TIMESTAMP { get; set; }
    }
}
