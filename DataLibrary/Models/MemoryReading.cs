namespace DataLibrary.Models
{
    public class MemoryReading : Reading
    {
        public long Available { get; set; }
        public long Total { get; set; }
        private double? _value;
        public override double Value 
        {
            get
            {
                if (_value.HasValue is false) 
                {
                    _value = 1 - (Available / Convert.ToDouble(Total));
                }
                return _value.Value;
            }
        }
}
}
