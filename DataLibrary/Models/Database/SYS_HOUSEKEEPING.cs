namespace DataLibrary.Models.Database;

public partial class SYS_HOUSEKEEPING
{
    public int ID { get; set; }
    public string? SRC_SCHEMA { get; set; }
    public string? SRC_TBL { get; set; }
    public string? MGR { get; set; }
    public string? SRC_PRIMARYKEY { get; set; }
    public int? KEYFROM { get; set; }
    public int? KEYTO { get; set; }
    public string? CLN_SCHEMA { get; set; }
    public string? CLN_TBL { get; set; }
    public string? CLN_PRIMARYKEY { get; set; }
}