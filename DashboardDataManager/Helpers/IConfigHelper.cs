namespace DataAccessLibrary.Helpers
{
    public interface IConfigHelper
    {
        string GetConnectionString(string key);
    }
}