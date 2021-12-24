using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Models
{
    public class Manager
    {
        private DateTime _startTime;
        private DateTime _endTime;

        public string Name { get; set; } = string.Empty;
        public int? RowId { get; set; }
        public Dictionary<string, int> RowsReadDict { get; set; } = new();
        public int RowsRead { get; set; }
        public Dictionary<string, int> RowsWrittenDict { get; set; } = new();
        public int RowsWritten { get; set; }
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                if (EndTime > value)
                {
                    Runtime = EndTime.Subtract(value);
                }
            }
        }
        public DateTime EndTime
        {
            get => _endTime; set
            {
                _endTime = value;
                if (value > StartTime && StartTime > default(DateTime))
                {
                    Runtime = value.Subtract(StartTime);
                }
            }
        }
        public TimeSpan Runtime { get; set; }
        public Dictionary<string, int> SqlCostDict { get; set; } = new();
        public Dictionary<string, int> TimeDict { get; set; } = new();
    }
}
