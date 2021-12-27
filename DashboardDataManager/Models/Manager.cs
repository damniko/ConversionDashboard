namespace DataLibrary.Models
{
    public class Manager
    {
        private DateTime? _startTime;
        private DateTime? _endTime;

        public string Name { get; set; } = string.Empty;
        public int? RowId { get; set; }
        public Dictionary<string, int> RowsReadDict { get; set; } = new();
        public int? RowsRead { get; set; }
        public Dictionary<string, int> RowsWrittenDict { get; set; } = new();
        public int? RowsWritten { get; set; }
        public DateTime? StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                if (value.HasValue && EndTime.HasValue)
                {
                    Runtime = EndTime?.Subtract(value.Value);
                }
            }
        }
        public DateTime? EndTime
        {
            get => _endTime; set
            {
                _endTime = value;
                if (value.HasValue && StartTime.HasValue)
                {
                    Runtime = value?.Subtract(StartTime.Value);
                }
            }
        }
        public TimeSpan? Runtime { get; set; }
        public Dictionary<string, int> SqlCostDict { get; set; } = new();
        public Dictionary<string, int> TimeDict { get; set; } = new();
    }
}
