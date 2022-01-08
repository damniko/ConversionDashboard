namespace DataLibrary.Models;

public class MemoryComponent
{
    public long? Total { get; set; }
    public List<MemoryReading> Readings { get; set; } = new();
}