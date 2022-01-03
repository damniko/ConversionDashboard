using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class NetworkData : INetworkData
    {
        internal enum ValueType
        {
            String, Numeric
        }
        private readonly IDataAccess _db;

        // Init entries
        internal readonly string _initReportType = "NETWORK_INIT";
        internal readonly string _nameKey = "Interface 0: Name";
        internal readonly string _macAddressKey = "Interface 0: MAC address";
        internal readonly string _speedKey = "Interface 0: Speed";
        // Reading entries
        internal readonly string _readingsReportType = "NETWORK";
        internal readonly string _receivedKey = "Interface 0: Bytes Received";
        internal readonly string _receivedDeltaKey = "Interface 0: Bytes Received (Delta)";
        internal readonly string _receivedSpeedKey = "Interface 0: Bytes Received (Speed)";
        internal readonly string _sendKey = "Interface 0: Bytes Send";
        internal readonly string _sendDeltaKey = "Interface 0: Bytes Send (Delta)";
        internal readonly string _sendSpeedKey = "Interface 0: Bytes Send (Speed)";

        public NetworkData(IDataAccess db)
        {
            _db = db;
        }

        #region Bytes received readings
        public List<Reading> GetRcvReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, _receivedKey, connStrKey);
        }

        public List<Reading> GetRcvDeltaReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, _receivedDeltaKey, connStrKey);
        }

        public List<Reading> GetRcvSpeedReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, _receivedSpeedKey, connStrKey);
        }
        #endregion

        #region Bytes send readings
        public List<Reading> GetSendReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, _sendKey, connStrKey);
        }

        public List<Reading> GetSendDeltaReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, _sendDeltaKey, connStrKey);
        }

        public List<Reading> GetSendSpeedReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, _sendSpeedKey, connStrKey);
        }
        #endregion

        public bool TryGetUpdatedName(DateTime fromDate, out string name, string connStrKey)
        {
            name = "";

            var value = GetInitValue(fromDate, _nameKey, ValueType.String, connStrKey);
            if (value != null)
            {
                name = (string)value;
            }

            return value != null;
        }

        public bool TryGetUpdatedMacAddress(DateTime fromDate, out string macAddress, string connStrKey)
        {
            macAddress = "";

            var value = GetInitValue(fromDate, _macAddressKey, ValueType.String, connStrKey);
            if (value != null)
            {
                macAddress = (string)value;
            }

            return value != null;
        }

        public bool TryGetUpdatedSpeed(DateTime fromDate, out long speed, string connStrKey)
        {
            speed = 0;

            var value = GetInitValue(fromDate, _speedKey, ValueType.Numeric, connStrKey);
            if (value != null)
            {
                speed = Convert.ToInt64(value);
            }

            return value != null;
        }

        internal List<Reading> GetReadings(DateTime fromDate, string reportKey, string connStrKey)
        {
            var output = from e in _db.GetHealthReportTbl(connStrKey)
                         where e.LOG_TIME > fromDate
                         where e.REPORT_TYPE == _readingsReportType && e.REPORT_KEY == reportKey
                         where e.LOG_TIME.HasValue && e.REPORT_NUMERIC_VALUE.HasValue
                         orderby e.LOG_TIME
                         select new Reading
                         {
                             Date = e.LOG_TIME!.Value,
                             Value = e.REPORT_NUMERIC_VALUE!.Value
                         };

            return output.ToList();
        }

        internal object? GetInitValue(DateTime fromDate, string reportKey, ValueType type, string connStrKey)
        {
            var result = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault(x => x.REPORT_TYPE == _initReportType && x.REPORT_KEY == reportKey);

            switch (type)
            {
                case ValueType.String:
                    return result?.REPORT_STRING_VALUE;
                case ValueType.Numeric:
                    return result?.REPORT_NUMERIC_VALUE!.Value.ToString();
                default:
                    throw new Exception("The specified ValueType is invalid");
            }
        }

    }
}
