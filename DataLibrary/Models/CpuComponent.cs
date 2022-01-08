namespace DataLibrary.Models;

public class CpuComponent
{
    public string? Name { get; set; }
    public long? LogicalCores { get; set; }
    public long? PhysicalCores { get; set; }
    public long? MaxFrequency { get; set; }
    public List<Reading> Readings { get; set; } = new();
}