using DataLibrary.Models;

namespace DataLibrary.DataAccess.Interfaces
{
    public interface INetworkData
    {
        List<Reading> GetRcvDeltaReadingsSince(DateTime fromDate, string connStrKey);
        List<Reading> GetRcvReadingsSince(DateTime fromDate, string connStrKey);
        List<Reading> GetRcvSpeedReadingsSince(DateTime fromDate, string connStrKey);
        List<Reading> GetSendDeltaReadingsSince(DateTime fromDate, string connStrKey);
        List<Reading> GetSendReadingsSince(DateTime fromDate, string connStrKey);
        List<Reading> GetSendSpeedReadingsSince(DateTime fromDate, string connStrKey);
        bool TryGetUpdatedMacAddress(DateTime fromDate, out string macAddress, string connStrKey);
        bool TryGetUpdatedName(DateTime fromDate, out string name, string connStrKey);
        bool TryGetUpdatedSpeed(DateTime fromDate, out long speed, string connStrKey);
    }
}