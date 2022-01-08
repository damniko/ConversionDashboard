using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess;

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
    public async Task<List<Reading>> GetRcvReadingsAsync(DateTime fromDate, string connStrKey)
    {
        return await GetReadings(fromDate, _receivedKey, connStrKey);
    }

    public async Task<List<Reading>> GetRcvDeltaReadingsAsync(DateTime fromDate, string connStrKey)
    {
        return await GetReadings(fromDate, _receivedDeltaKey, connStrKey);
    }

    public async Task<List<Reading>> GetRcvSpeedReadingsAsync(DateTime fromDate, string connStrKey)
    {
        return await GetReadings(fromDate, _receivedSpeedKey, connStrKey);
    }
    #endregion

    #region Bytes send readings
    public async Task<List<Reading>> GetSendReadingsAsync(DateTime fromDate, string connStrKey)
    {
        return await GetReadings(fromDate, _sendKey, connStrKey);
    }

    public async Task<List<Reading>> GetSendDeltaReadingsAsync(DateTime fromDate, string connStrKey)
    {
        return await GetReadings(fromDate, _sendDeltaKey, connStrKey);
    }

    public async Task<List<Reading>> GetSendSpeedReadingsAsync(DateTime fromDate, string connStrKey)
    {
        return await GetReadings(fromDate, _sendSpeedKey, connStrKey);
    }
    #endregion

    public async Task<string?> GetNameAsync(DateTime fromDate, string connStrKey)
    {
        var value = await GetInitValue(fromDate, _nameKey, ValueType.String, connStrKey);
        return (string?)value;
    }

    public async Task<string?> GetMacAddressAsync(DateTime fromDate, string connStrKey)
    {
        var value = await GetInitValue(fromDate, _macAddressKey, ValueType.String, connStrKey);
        return (string?)value;
    }

    public async Task<long?> GetSpeedAsync(DateTime fromDate, string connStrKey)
    {
        var value = await GetInitValue(fromDate, _speedKey, ValueType.Numeric, connStrKey);
        return (long?)value;
    }

    internal async Task<List<Reading>> GetReadings(DateTime fromDate, string reportKey, string connStrKey)
    {
        var output = from e in await _db.GetHealthReportAsync(connStrKey)
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

    internal async Task<object?> GetInitValue(DateTime fromDate, string reportKey, ValueType type, string connStrKey)
    {
        var result = (from e in await _db.GetHealthReportAsync(connStrKey)
            where e.LOG_TIME > fromDate
            orderby e.LOG_TIME
            where e.REPORT_TYPE == _initReportType && e.REPORT_KEY == reportKey
            select e).LastOrDefault();

        switch (type)
        {
            case ValueType.String:
                return result?.REPORT_STRING_VALUE;
            case ValueType.Numeric:
                return result?.REPORT_NUMERIC_VALUE;
            default:
                throw new Exception("The specified ValueType is invalid");
        }
    }

}