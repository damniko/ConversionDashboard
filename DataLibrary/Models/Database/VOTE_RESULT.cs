namespace DataLibrary.Models.Database
{
    public partial class VOTE_RESULT
    {
        public string ID { get; set; } = null!;
        public string VOTE_NAME { get; set; } = null!;
        public string PASSED { get; set; } = null!;
        public string? VOTE_MESSAGE { get; set; }
        public DateTime CREATED { get; set; }
    }
}
