namespace DataLibrary.Models;

public class StatementTable
{
    public string? Manager { get; set; }
    public string? Identifier { get; set; }
    public string? IdentifierShort { get; set; }
    public string? Schema { get; set; }
    public string? SchemaShort { get; set; }
    public string? Table { get; set; }
    public DateTime? Date { get; set; }
}