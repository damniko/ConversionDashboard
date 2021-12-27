namespace DataLibrary.Models
{
    public class HealthReport
    {
        public string? HostName { get; set; }
        public string? MonitorName { get; set; }

        public CpuComponent? Cpu { get; set; }
        public MemoryComponent? Memory { get; set; }
    }
}
