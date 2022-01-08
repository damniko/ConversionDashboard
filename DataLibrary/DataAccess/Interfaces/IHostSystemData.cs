namespace DataLibrary.DataAccess.Interfaces;

public interface IHostSystemData
{
    Task<string?> GetHostNameAsync(DateTime fromDate, string connStrKey);
    Task<string?> GetMonitorNameAsync(DateTime fromDate, string connStrKey);
}