using DataLibrary.Helpers;

namespace DataLibrary.Tests
{
    public class ConfigHelperMock : IConfigHelper
    {
        private readonly string _testDbConnStr = @"Data Source=NIKOLAJ-DAM-LEN\SQLEXPRESS;Initial Catalog=DASHBOARD_TEST;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public string GetConnectionString(string key)
        {
            return _testDbConnStr;
        }
    }
}