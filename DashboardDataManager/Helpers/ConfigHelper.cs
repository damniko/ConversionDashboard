using System.Configuration;

namespace DataLibrary.Helpers
{
    public class ConfigHelper : IConfigHelper
    {
        public string GetConnectionString(string key)
        {
            string? output = ConfigurationManager.ConnectionStrings[key]?.ConnectionString;

            if (output is null)
            {
                throw new Exception($"The connection string with key { key } could not be found.");
            }

            return output;
        }
    }
}
