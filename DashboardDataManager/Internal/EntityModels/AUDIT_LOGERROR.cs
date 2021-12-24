namespace DataLibrary.Internal.EntityModels
{
    internal partial class AUDIT_LOGERROR
    {
        public string ID { get; set; } = null!;
        public string MGRNAME { get; set; } = null!;
        public DateTime CREATED { get; set; }
        public string SOURCEROWS { get; set; } = null!;
        public string? MESSAGE { get; set; }
    }
}
