namespace DataLibrary.Models.Database;

public partial class TABLE_LOG
{
    public string ID { get; set; } = null!;
    public string SCHEMA_NAME { get; set; } = null!;
    public string TABLE_NAME { get; set; } = null!;
    public string STATE { get; set; } = null!;
    public DateTime? CREATED { get; set; }
}