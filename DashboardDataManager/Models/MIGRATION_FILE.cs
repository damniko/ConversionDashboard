using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class MIGRATION_FILE
    {
        public string ID { get; set; } = null!;
        public byte[]? CONTENT { get; set; }
        public string FILE_NAME { get; set; } = null!;
        public string? RELATIVE_PATH { get; set; }
        public string? JENKINS_JOB { get; set; }
        public string? TITLE { get; set; }
        public DateTime? CREATED { get; set; }
    }
}
