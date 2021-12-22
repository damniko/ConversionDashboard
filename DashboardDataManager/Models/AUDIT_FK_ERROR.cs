using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class AUDIT_FK_ERROR
    {
        public int ID { get; set; }
        public string? TABLENAME { get; set; }
        public string? FOREIGN_KEY_VIOLATED { get; set; }
        public string? ROWDATA { get; set; }
    }
}
