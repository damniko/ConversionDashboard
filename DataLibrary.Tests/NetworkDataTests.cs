using System;
using System.Collections.Generic;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;
using DataLibrary.Models.Database;
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
        public void GetReadings_ConvertsMultipleEntries_ToReadings()
        {
            // Arrange
            string key = "Interface 0: Bytes Received";
            long firstValue = 1000;
            DateTime firstDate = DateTime.Parse("2010/01/01 10:00:00");
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = firstDate,
                REPORT_TYPE = "NETWORK",
                REPORT_KEY = key,
                REPORT_NUMERIC_VALUE = firstValue
            };
            long secondValue = 2000;
            DateTime secondDate = DateTime.Parse("2010/01/01 10:00:05");
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = secondDate,
                REPORT_TYPE = "NETWORK",
                REPORT_KEY = key,
                REPORT_NUMERIC_VALUE = secondValue
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var readings = _sut.GetReadings(DateTime.MinValue, key, "");

            // Assert
            readings.Should().HaveCount(2, "because two entries exist")
                .And.SatisfyRespectively(
                    first => first.Should().BeEquivalentTo(new Reading { Date = firstDate, Value = firstValue }),
                    second => second.Should().BeEquivalentTo(new Reading { Date = secondDate, Value = secondValue }));
        }
        
        [Fact]
        public void GetReadings_ReportNumericValueIsNull_IgnoresInvalidEntry()
        {
            // Arrange
            string key = "Interface 0: Bytes Received";
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "NETWORK",
                REPORT_KEY = key,
                REPORT_NUMERIC_VALUE = null
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var readings = _sut.GetReadings(DateTime.MinValue, key, "");

            // Assert
            readings.Should().BeEmpty("because the one entry does not have a value for {0}", nameof(HEALTH_REPORT.REPORT_NUMERIC_VALUE));
        }

        [Fact]
        public void GetReadings_MultipleKeys_GetsCorrectKey()
        {
            // Arrange
            string rcvReportKey = "Interface 0: Bytes Received";
            DateTime rcvDate = DateTime.Parse("2010/01/01 10:00:00");
            long rcvVal = 1;
            var entries = new List<HEALTH_REPORT>
            {
                new()
                {
                    LOG_TIME = rcvDate,
                    REPORT_TYPE = "NETWORK",
                    REPORT_KEY = rcvReportKey,
                    REPORT_NUMERIC_VALUE = rcvVal
                },
                new()
                {
                    LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                    REPORT_TYPE = "NETWORK",
                    REPORT_KEY = "Interface 0: Bytes Received (Delta)",
                    REPORT_NUMERIC_VALUE = 2
                },
                new()
                {
                    LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                    REPORT_TYPE = "NETWORK",
                    REPORT_KEY = "Interface 0: Bytes Received (Speed)",
                    REPORT_NUMERIC_VALUE = 3
                },
                new()
                {
                    LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                    REPORT_TYPE = "NETWORK",
                    REPORT_KEY = "Interface 0: Bytes Send",
                    REPORT_NUMERIC_VALUE = 4
                },
                new()
                {
                    LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                    REPORT_TYPE = "NETWORK",
                    REPORT_KEY = "Interface 0: Bytes Send (Delta)",
                    REPORT_NUMERIC_VALUE = 5
                },
                new()
                {
                    LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                    REPORT_TYPE = "NETWORK",
                    REPORT_KEY = "Interface 0: Bytes Send (Speed)",
                    REPORT_NUMERIC_VALUE = 6
                }
            };
             _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(entries);
            
            var expected = new Reading
            {
                Date = rcvDate,
                Value = rcvVal
            };

            // Act
            var readings = _sut.GetReadings(DateTime.MinValue, rcvReportKey, "");

            // Assert
            readings.Should().ContainSingle("because there is one entry for each report key")
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetReadings_LogTimeIsNull_IgnoresInvalidEntry()
        {
            // Arrange
            string key = "Interface 0: Bytes Received";
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = null,
                REPORT_TYPE = "NETWORK",
                REPORT_KEY = key,
                REPORT_NUMERIC_VALUE = 100
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var readings = _sut.GetReadings(DateTime.MinValue, key, "");

            // Assert
            readings.Should().BeEmpty("because the one entry does not have a value for {0}", nameof(HEALTH_REPORT.LOG_TIME));
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
