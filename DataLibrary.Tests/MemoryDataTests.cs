using System;
using System.Collections.Generic;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models.Database;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataLibrary.Tests
{
    public class MemoryDataTests
    {
        private readonly MemoryData _sut;
        private readonly Mock<IDataAccess> _dbMock;
        private readonly Mock<ILogger<MemoryData>> _loggerMock;

        public MemoryDataTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _loggerMock = new Mock<ILogger<MemoryData>>();
            _sut = new MemoryData(_dbMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData(20, 100, 0.80d)]
        [InlineData(0, 100, 1.00d)]
        [InlineData(100, 100, 0.00d)]
        public void GetReadingsSinceDate_HasOneTotal_CalculatesCorrectValue(long? available, long? total, double expected)
        {
            // Arrange
            var entries = new List<HEALTH_REPORT>
            {
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = total,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:00")
                },
                new()
                {
                    REPORT_TYPE = "MEMORY",
                    REPORT_KEY = "AVAILABLE",
                    REPORT_NUMERIC_VALUE = available,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:05")
                }
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(entries);

            // Act
            var result = _sut.GetReadingsSinceDate(DateTime.MinValue, "");

            // Assert
            result.Should().ContainSingle()
                .Which.Value.Should().Be(expected);
        }

        [Fact]
        public void GetReadingsSinceDate_HasMultipleTotals_CalculatesCorrectLoadPercentages()
        {
            // Arrange
            long firstTotal = 100;
            long firstAvailable = 10;
            double firstExpected = 0.90d;
            long secondTotal = 200;
            long secondAvailable = 10;
            double secondExpected = 0.95d;
            var entries = new List<HEALTH_REPORT>
            {
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = firstTotal,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:00")
                },
                new()
                {
                    REPORT_TYPE = "MEMORY",
                    REPORT_KEY = "AVAILABLE",
                    REPORT_NUMERIC_VALUE = firstAvailable,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:05")
                },
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = secondTotal,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:10")
                },
                new()
                {
                    REPORT_TYPE = "MEMORY",
                    REPORT_KEY = "AVAILABLE",
                    REPORT_NUMERIC_VALUE = secondAvailable,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:15")
                }
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(entries);

            // Act
            var result = _sut.GetReadingsSinceDate(DateTime.MinValue, "");

            // Assert
            result.Should().HaveCount(2, "because there are two readings")
                .And.SatisfyRespectively(
                    first => first.Value.Should().Be(firstExpected, "because available is {0} out of {1} total", firstAvailable, firstTotal),
                    second => second.Value.Should().Be(secondExpected, "because available is {0} out of {1} total", secondAvailable, secondTotal)
                );
        }

        [Fact]
        public void GetLatestTotal_HasOneEntry_ReturnsTotalValue()
        {
            // Arrange
            long expected = 100;
            var entries = new List<HEALTH_REPORT>
            {
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = expected,
                    LOG_TIME = DateTime.Parse("2010/01/01 10:00:00")
                }
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(entries);

            // Act
            var result = _sut.TryGetUpdatedTotal(DateTime.MinValue, out long actual, "");

            // Assert
            result.Should().BeTrue("because one entry exists");
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetLatestTotal_HasNoEntries_ReturnsNull()
        {
            // Arrange
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(Array.Empty<HEALTH_REPORT>);

            // Act
            var result = _sut.TryGetUpdatedTotal(DateTime.MinValue, out _, "");

            // Assert
            result.Should().BeFalse("because no entries exist");
        }

        [Fact]
        public void GetLatestTotal_HasMultipleEntries_ReturnsLatestTotalValue()
        {
            // Arrange
            long expected = 300;
            var entries = new List<HEALTH_REPORT>
            {
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = 100,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:00")
                },
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = 200,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:05")
                },
                new()
                {
                    REPORT_TYPE = "MEMORY_INIT",
                    REPORT_KEY = "TOTAL",
                    REPORT_NUMERIC_VALUE = expected,
                    LOG_TIME = DateTime.Parse("2000/01/01 10:00:10")
                }
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(entries);

            // Act
            var result = _sut.TryGetUpdatedTotal(DateTime.MinValue, out long actual, "");

            // Assert
            result.Should().BeTrue("because multiple valid entries exist");
            actual.Should().Be(expected, "because this is the value of the latest entry");
        }

        // TODO - test logger when it is implemented
    }
}
