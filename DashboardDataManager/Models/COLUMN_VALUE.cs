using System;
using System.Collections.Generic;

namespace DashboardDataManager.Models
{
    public partial class COLUMN_VALUE
    {
        public string ID { get; set; } = null!;
        public string COLUMN_NAME { get; set; } = null!;
        public string VALUE { get; set; } = null!;
        public DateTime CREATED { get; set; }
        public string VOTE_COMBINATION_FK { get; set; } = null!;
    }
}
