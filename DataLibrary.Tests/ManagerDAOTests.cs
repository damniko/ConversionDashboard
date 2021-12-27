using DataLibrary.DataAccess;
using Xunit;

namespace DataLibrary.Tests
{
    public class ManagerDAOTests
    {
        [Fact]
        public void Correctly_defines_managers_for_each_execution()
        {
            var dao = new ManagerDAO(new ConfigHelperMock());

            var result = dao.Get("", System.Data.SqlTypes.SqlDateTime.MinValue.Value);

            Assert.Equal(3, result.Count);
            Assert.Collection(result,
                m => Assert.Equal(10, m.RowsRead),
                m => Assert.Equal(30, m.RowsRead),
                m => Assert.Equal(50, m.RowsRead));
        }
        // Other tests worth implementing:
        // - Gets managers and correct values when the manager crashes unexpectedly
    }
}