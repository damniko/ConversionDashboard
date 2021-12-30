namespace DataLibrary.DataAccess.Interfaces
{
    public interface IHostSystemData
    {
        bool TryGetUpdatedHostName(DateTime fromDate, out string hostname, string connStrKey);
        bool TryGetUpdatedMonitorName(DateTime fromDate, out string monitorName, string connStrKey);
    }
}