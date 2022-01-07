namespace DataLibrary.Models.Database
{
    public partial class MANAGER_TRACKING
    {
        public string? MGR { get; set; }
        public string? STATUS { get; set; }
        public int? RUNTIME { get; set; }
        public int? PERFORMANCECOUNTROWSREAD { get; set; }
        public int? PERFORMANCECOUNTROWSWRITTEN { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public int? WEEK { get; set; }
    }
}
