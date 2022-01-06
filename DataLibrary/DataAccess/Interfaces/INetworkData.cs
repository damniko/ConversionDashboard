using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public interface INetworkData
    {
        Task<string?> GetMacAddressAsync(DateTime fromDate, string connStrKey);
        Task<string?> GetNameAsync(DateTime fromDate, string connStrKey);
        Task<List<Reading>> GetRcvDeltaReadingsAsync(DateTime fromDate, string connStrKey);
        Task<List<Reading>> GetRcvReadingsAsync(DateTime fromDate, string connStrKey);
        Task<List<Reading>> GetRcvSpeedReadingsAsync(DateTime fromDate, string connStrKey);
        Task<List<Reading>> GetSendDeltaReadingsAsync(DateTime fromDate, string connStrKey);
        Task<List<Reading>> GetSendReadingsAsync(DateTime fromDate, string connStrKey);
        Task<List<Reading>> GetSendSpeedReadingsAsync(DateTime fromDate, string connStrKey);
        Task<long?> GetSpeedAsync(DateTime fromDate, string connStrKey);
    }
}