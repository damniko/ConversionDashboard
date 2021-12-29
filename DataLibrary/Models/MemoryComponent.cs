using DataLibrary.Internal.EFModels;

namespace DataLibrary.Models
{
    public class MemoryComponent
    {
        public long? Total { get; set; }
        public List<Reading> Readings { get; set; } = new();

        internal void CreateAndAddReading(HEALTH_REPORT input)
        {
            if (Total == null || Total <= 0)
            {
                throw new ArgumentException("Cannot create memory reading without a total");
            }
            if (input.REPORT_TYPE != "MEMORY" || input.REPORT_KEY != "AVAILABLE")
            {
                throw new ArgumentException("The given entry is not a valid memory reading");
            }

            var available = double.Parse(input.REPORT_NUMERIC_VALUE.GetValueOrDefault().ToString());
            Reading reading = new()
            {
                Date = input.LOG_TIME.GetValueOrDefault(),
                Value = 1 - (available / Total.Value)
            };
            Readings.Add(reading);
        }
    }
}
