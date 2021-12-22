﻿using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class STATEMENT_TABLE
    {
        public string? MGR { get; set; }
        public string? IDENTIFIER { get; set; }
        public string? IDENTIFIER_SHORT { get; set; }
        public string? SCHEMA_NAME { get; set; }
        public string? SCHEMA_NAME_FULL { get; set; }
        public string? TABLE_NAME { get; set; }
        public DateTime? CREATED { get; set; }
    }
}
