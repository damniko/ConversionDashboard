using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class VOTE_COMBINATION
    {
        public string ID { get; set; } = null!;
        public string VOTE_NAME { get; set; } = null!;
        public string SCHEMA_NAME { get; set; } = null!;
        public string TABLE_NAME { get; set; } = null!;
        public DateTime CREATED { get; set; }
    }
}
