using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class LOGGING_CONTEXT
    {
        public long CONTEXT_ID { get; set; }
        public long? EXECUTION_ID { get; set; }
        public string? CONTEXT { get; set; }
    }
}
