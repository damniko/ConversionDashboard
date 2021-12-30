namespace DataLibrary.Internal.EFModels
{
    public partial class LOGGING
    {
        public DateTime? CREATED { get; set; }
        public string? LOG_MESSAGE { get; set; }
        public string? LOG_LEVEL { get; set; }
        public long? EXECUTION_ID { get; set; }
        public long? CONTEXT_ID { get; set; }
    }
}
