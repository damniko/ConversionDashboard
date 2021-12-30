namespace DataLibrary.Internal.EFModels
{
    public partial class AUDIT_LOGINFO
    {
        public string ID { get; set; } = null!;
        public string MGRNAME { get; set; } = null!;
        public DateTime CREATED { get; set; }
        public int? BUSINESSID { get; set; }
        public string? CPRNR { get; set; }
        public string? MESSAGE { get; set; }
        public int? RECONCILIATIONVALUE { get; set; }
    }
}
