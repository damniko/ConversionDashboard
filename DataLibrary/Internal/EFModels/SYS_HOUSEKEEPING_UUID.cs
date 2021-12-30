namespace DataLibrary.Internal.EFModels
{
    public partial class SYS_HOUSEKEEPING_UUID
    {
        public int ID { get; set; }
        public string? SRC_SCHEMA { get; set; }
        public string? SRC_TBL { get; set; }
        public string? MGR { get; set; }
        public string? SRC_PRIMARYKEY { get; set; }
        public string? ID_PREFIX { get; set; }
        public string? CLN_SCHEMA { get; set; }
        public string? CLN_TBL { get; set; }
        public string? CLN_PRIMARYKEY { get; set; }
    }
}
