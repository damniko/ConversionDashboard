using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.DataAccess;
using DataLibrary.Internal;
using DataLibrary.Internal.EFModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace DataLibrary.Tests
{
    public class NetworkDataTests
    {
        private readonly NetworkData _sut;
        private readonly Mock<IDataAccess> _dbMock;

        public NetworkDataTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _sut = new NetworkData(_dbMock.Object);
        }

        [Fact]
        public void GetRcvReadingsSince_HasMultipleEntries_ReturnsMultipleReadings()
        {
            // Arrange
            long firstValue = 1000;
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "NETWORK",
                REPORT_KEY = "Interface 0: Bytes Received",
                REPORT_NUMERIC_VALUE = firstValue
            };
            long secondValue = 2000;
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:10"),
                REPORT_TYPE = "NETWORK",
                REPORT_KEY = "Interface 0: Bytes Received",
                REPORT_NUMERIC_VALUE = secondValue
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var readings = _sut.GetRcvReadingsSince(DateTime.MinValue, "");

            // Assert
            readings.Should().HaveCount(2, "because two entries exist")
                .And.SatisfyRespectively(
                    first => first.Value.Should().Be(firstValue),
                    second => second.Value.Should().Be(secondValue));
        }

        [Fact]
        public void TryGetUpdatedName_HasOneEntry_ReturnsTrueAndValue()
        {
            // Arrange
            string expected = "Test Interface";
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "NETWORK_INIT",
                REPORT_KEY = "Interface 0: Name",
                REPORT_STRING_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var result = _sut.TryGetUpdatedName(DateTime.MinValue, out string actual, "");

            // Assert
            result.Should().BeTrue("because one entry exists");
            actual.Should().Be(expected);
        }

        [Fact]
        public void TryGetUpdatedMacAddress_HasOneEntry_ReturnsTrueAndValue()
        {
            // Arrange
            string expected = "Test MAC Address";
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "NETWORK_INIT",
                REPORT_KEY = "Interface 0: MAC address",
                REPORT_STRING_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var result = _sut.TryGetUpdatedMacAddress(DateTime.MinValue, out string actual, "");

            // Assert
            result.Should().BeTrue("because one entry exists");
            actual.Should().Be(expected);
        }

        [Fact]
        public void TryGetUpdatedSpeed_HasOneEntry_ReturnsTrueAndValue()
        {
            // Arrange
            long expected = 1000;
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "NETWORK_INIT",
                REPORT_KEY = "Interface 0: Speed",
                REPORT_NUMERIC_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var result = _sut.TryGetUpdatedSpeed(DateTime.MinValue, out long actual, "");

            // Assert
            result.Should().BeTrue("because one entry exists");
            actual.Should().Be(expected);
        }
    }
}
