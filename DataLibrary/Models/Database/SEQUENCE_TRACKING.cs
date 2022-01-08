namespace DataLibrary.Models.Database;

public partial class SEQUENCE_TRACKING
{
    public string? MGR { get; set; }
    public int? START_SEQ_VAL { get; set; }
    public int? END_SEQ_VAL { get; set; }
    public string? TABLE_NAME { get; set; }
    public string? COLUMN_NAME { get; set; }
}