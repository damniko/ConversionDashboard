namespace DataLibrary.Models;

public class AuditLogError
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Manager { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string SourceRows { get; set; } = string.Empty;
    public string? Message { get; set; }
}