using DataLibrary.Internal;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class NetworkData
    {
        private enum ValueType
        {
            String, Numeric
        }
        private readonly IDataAccess _db;

        public NetworkData(IDataAccess db)
        {
            _db = db;
        }

        #region Bytes received readings
        public List<Reading> GetRcvReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, "Interface 0: Bytes Received", connStrKey);
        }

        public List<Reading> GetRcvDeltaReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, "Interface 0: Bytes Received (Delta)", connStrKey);
        }

        public List<Reading> GetRcvSpeedReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, "Interface 0: Bytes Received (Speed)", connStrKey);
        }
        #endregion

        #region Bytes send readings
        public List<Reading> GetSendReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, "Interface 0: Bytes Send", connStrKey);
        }

        public List<Reading> GetSendDeltaReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, "Interface 0: Bytes Send (Delta)", connStrKey);
        }

        public List<Reading> GetSendSpeedReadingsSince(DateTime fromDate, string connStrKey)
        {
            return GetReadings(fromDate, "Interface 0: Bytes Send (Speed)", connStrKey);
        }
        #endregion

        public bool TryGetUpdatedName(DateTime fromDate, out string name, string connStrKey)
        {
            name = "";

            var value = GetValue(fromDate, "Interface 0: Name", ValueType.String, connStrKey);
            if (value != null)
            {
                name = (string)value;
            }

            return value != null;
        }

        public bool TryGetUpdatedMacAddress(DateTime fromDate, out string macAddress, string connStrKey)
        {
            macAddress = "";

            var value = GetValue(fromDate, "Interface 0: MAC address", ValueType.String, connStrKey);
            if (value != null)
            {
                macAddress = (string)value;
            }

            return value != null;
        }

        public bool TryGetUpdatedSpeed(DateTime fromDate, out long speed, string connStrKey)
        {
            speed = 0;

            var value = GetValue(fromDate, "Interface 0: Speed", ValueType.Numeric, connStrKey);
            if(value != null)
            {
                speed = Convert.ToInt64(value);
            }

            return value != null;
        }

        private List<Reading> GetReadings(DateTime fromDate, string reportKey, string connStrKey)
        {
            var output = from e in _db.GetHealthReportTbl(connStrKey)
                         where e.LOG_TIME > fromDate
                         where e.REPORT_TYPE == "NETWORK" && e.REPORT_KEY == reportKey
                         where e.LOG_TIME.HasValue && e.REPORT_NUMERIC_VALUE.HasValue
                         orderby e.LOG_TIME
                         select new Reading
                         {
                             Date = e.LOG_TIME!.Value,
                             Value = e.REPORT_NUMERIC_VALUE!.Value
                         };

            return output.ToList();
        }

        private object? GetValue(DateTime fromDate, string reportKey, ValueType type, string connStrKey)
        {
            var result = _db.GetHealthReportTbl(connStrKey)
                .Where(x => x.LOG_TIME > fromDate)
                .OrderBy(x => x.LOG_TIME)
                .LastOrDefault(x => x.REPORT_TYPE == "NETWORK_INIT" && x.REPORT_KEY == reportKey);

            switch (type)
            {
                case ValueType.String:
                    return result?.REPORT_STRING_VALUE;
                case ValueType.Numeric:
                    return result?.REPORT_NUMERIC_VALUE!.Value.ToString();
                default:
                    throw new Exception("The specified ValueType is not valid");
            }
        }

    }
}
