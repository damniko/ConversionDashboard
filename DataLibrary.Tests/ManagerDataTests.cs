using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.DataAccess;
using DataLibrary.Models.Database;
using DataLibrary.Models;
using FluentAssertions.Execution;

namespace DataLibrary.Tests
{
    public class ManagerDataTests
    {
        private readonly IManagerData _sut;
        private readonly Mock<IDataAccess> _dbMock;

        public ManagerDataTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _sut = new AltManagerData(_dbMock.Object);
        }

        [Fact]
        public async Task OneManager_AllEntriesAtOnce_CreatesManager()
        {
            // Arrange
            const string name = "manager", nameAlt = "manager,rnd_2032045042";
            DateTime startTime = DateTime.Parse("2021-10-28 15:07:23.277"), endTime = DateTime.Parse("2021-10-28 15:07:32.747");
            const int read = 1, written = 4;
            const int dictCount = 2;
            var engineProperties = new List<ENGINE_PROPERTY>
            {
                new() { MANAGER = name, KEY = "START_TIME", VALUE = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:23.347") },
                new() { MANAGER = name, KEY = "TIME_HOUSEKEEPING", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:25.617") },
                new() { MANAGER = name, KEY = "sql_0_398402984", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.043") },
                new() { MANAGER = name, KEY = "TIME_SELECT_TO_TOTAL_COUNT", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.073") },
                new() { MANAGER = nameAlt, KEY = "runtimeConversion", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.467") },
                new() { MANAGER = name, KEY = "READ[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.493") },
                new() { MANAGER = name, KEY = "Læste rækker", VALUE = read.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.533") },
                new() { MANAGER = name, KEY = "Skrevne rækker", VALUE = written.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.567") },
                new() { MANAGER = name, KEY = "FinishedExecution", VALUE = "yes", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.750") },
                new() { MANAGER = name, KEY = "END_TIME", VALUE = endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.810") },
                new() { MANAGER = name, KEY = "READ[TOTAL]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.897") },
                new() { MANAGER = name, KEY = "WRITE[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.923") },
                new() { MANAGER = name, KEY = "WRITE[Y]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.953") },
                new() { MANAGER = nameAlt, KEY = "runtimeOverall", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:34.180") },
            };
            _dbMock.Setup(x => x.GetEnginePropertiesAsync(It.IsAny<string>()))
                .ReturnsAsync(engineProperties);

            // Act
            var managers = await _sut.GetSinceAsync(DateTime.MinValue, "");

            // Assert
            using (new AssertionScope())
            {
                managers.Should().ContainSingle("because there are only entries for one manager")
                    .Which.Should().Match<Manager>((m) => 
                       m.Name == name
                    && m.StartTime == startTime
                    && m.EndTime == endTime
                    && m.RowsRead == read
                    && m.RowsWritten == written
                    && m.RowsReadDict.Count == dictCount
                    && m.RowsWrittenDict.Count == dictCount
                    && m.TimeDict.Count == dictCount
                    && m.SqlCostDict.Count == dictCount);
            }
        }

        [Fact]
        public async Task OneManager_OneEntryWithoutStartTime_()
        {
            // Arrange
            const string name = "manager", nameAlt = "manager,rnd_2032045042";
            DateTime startTime = DateTime.Parse("2021-10-28 15:07:23.277"), endTime = DateTime.Parse("2021-10-28 15:07:32.747");
            const int read = 1, written = 4;
            const int dictCount = 2;
            var engineProperties = new List<ENGINE_PROPERTY>
            {
                new() { MANAGER = name, KEY = "START_TIME", VALUE = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:23.347") },
                new() { MANAGER = name, KEY = "TIME_HOUSEKEEPING", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:25.617") },
                new() { MANAGER = name, KEY = "sql_0_398402984", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.043") },
                new() { MANAGER = name, KEY = "TIME_SELECT_TO_TOTAL_COUNT", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.073") },
                new() { MANAGER = nameAlt, KEY = "runtimeConversion", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.467") },
                new() { MANAGER = name, KEY = "READ[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.493") },
                new() { MANAGER = name, KEY = "Læste rækker", VALUE = read.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.533") },
                new() { MANAGER = name, KEY = "Skrevne rækker", VALUE = written.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.567") },
                new() { MANAGER = name, KEY = "FinishedExecution", VALUE = "yes", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.750") },
                new() { MANAGER = name, KEY = "END_TIME", VALUE = endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.810") },
                new() { MANAGER = name, KEY = "READ[TOTAL]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.897") },
                new() { MANAGER = name, KEY = "WRITE[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.923") },
                new() { MANAGER = name, KEY = "WRITE[Y]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.953") },
                new() { MANAGER = nameAlt, KEY = "runtimeOverall", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:34.180") },
            };
            _dbMock.Setup(x => x.GetEnginePropertiesAsync(It.IsAny<string>()))
                .ReturnsAsync(engineProperties);

            // Act

            // Assert
        }
    }
}
