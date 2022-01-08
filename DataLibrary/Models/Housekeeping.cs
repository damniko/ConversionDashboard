namespace DataLibrary.Models;

public class Housekeeping
{
    public int Id { get; set; }
    public string Manager { get; set; } = string.Empty;
    public string SrcSchema { get; set; } = string.Empty;
    public string SrcTable { get; set; } = string.Empty;
    public string SrcPrimaryKey { get; set; } = string.Empty;
    public string CleanSchema { get; set; } = string.Empty;
    public string CleanTable { get; set; } = string.Empty;
    public string CleanPrimaryKey { get; set; } = string.Empty;
    public int KeyFrom { get; set; }
    public int KeyTo { get; set; }
}