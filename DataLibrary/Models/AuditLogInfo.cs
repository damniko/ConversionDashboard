namespace DataLibrary.Models;

public class AuditLogInfo
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Manager { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int? BusinessId { get; set; }
    public string? CprNumber { get; set; }
    public string? ErrorDescription { get; set; }
    public string? Message { get; set; }
    public int? ReconciliationValue { get; set; }
}