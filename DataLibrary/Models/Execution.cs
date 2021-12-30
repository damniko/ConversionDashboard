namespace DataLibrary.Models
{
    public class Execution
    {
        public long Id { get; set; }
        public Guid Uuid { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Dictionary<long, string?> ContextDict { get; set; } = new();        
    }
}
