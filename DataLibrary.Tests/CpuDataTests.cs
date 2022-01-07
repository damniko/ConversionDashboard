using System;
using System.Collections.Generic;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models.Database;
using FluentAssertions;
using Moq;
using Xunit;

namespace DataLibrary.Tests
{
    public class CpuDataTests
    {
        private readonly CpuData _sut;
        private readonly Mock<IDataAccess> _dbMock;

        public CpuDataTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _sut = new CpuData(_dbMock.Object);
        }

        [Fact]
        public async void GetReadingsAsync_HasNoEntries_ReturnsEmptyList()
        {
            // Arrange
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT>());

            // Act
            var readings = await _sut.GetReadingsAsync(DateTime.MinValue, "");

            // Assert
            readings.Should().BeEmpty("because no entries exist");
        }

        [Fact]
        public async void GetReadingsAsync_EntryHasNoValue_SkipsEntry()
        {
            // Arrange
            var entry = new HEALTH_REPORT
            {
                REPORT_TYPE = "CPU",
                REPORT_KEY = "LOAD",
                REPORT_NUMERIC_VALUE = null
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { entry });

            // Act
            var readings = await _sut.GetReadingsAsync(DateTime.MinValue, "");

            // Assert
            readings.Should().BeEmpty("because the single entry does not have a valid value");
        }

        //[Fact]
        //public void GetReadingsAsync_EntryHasNoValue_LogsWarning()
        //{
        //    // Arrange
        //    var entry = new HEALTH_REPORT
        //    {
        //        REPORT_TYPE = "CPU",
        //        REPORT_KEY = "LOAD",
        //        REPORT_NUMERIC_VALUE = null
        //    };
        //    _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
        //        .Returns(new List<HEALTH_REPORT> { entry });

        //    // Act
        //    var readings = _sut.GetReadingsAsync(DateTime.MinValue, "");

        //    // Assert
        //    // TODO - add this when the logger is implemented
        //}

        [Fact]
        public async void GetReadingsAsync_HasOneEntry_ReturnsSingleReading()
        {
            // Arrange
            double expected = 0.25d;
            long value = 25;
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "CPU",
                REPORT_KEY = "LOAD",
                REPORT_NUMERIC_VALUE = value
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { entry });

            // Act
            var readings = await _sut.GetReadingsAsync(DateTime.MinValue, "");

            // Assert
            readings.Should().ContainSingle("because one entry exists")
                .Which.Value.Should().Be(expected, "because the value (in percent) is {0}", value);
        }

        [Fact]
        public async void GetReadingsAsync_HasMultipleEntries_ReturnsReadings()
        {
            // Arrange
            double firstExpected = 0.25d;
            long firstValue = 25;
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "CPU",
                REPORT_KEY = "LOAD",
                REPORT_NUMERIC_VALUE = firstValue
            };
            double secondExpected = 0.50d;
            long secondValue = 50;
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:05"),
                REPORT_TYPE = "CPU",
                REPORT_KEY = "LOAD",
                REPORT_NUMERIC_VALUE = secondValue
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var readings = await _sut.GetReadingsAsync(DateTime.MinValue, "");

            // Assert
            readings.Should().HaveCount(2, "because two entries exist")
                .And.SatisfyRespectively(
                    first => first.Value.Should().Be(firstExpected, "because the value (in percent) is {0}", firstValue),
                    second => second.Value.Should().Be(secondExpected, "because the value (in percent) is {0}", secondValue));
        }
    
        [Fact]
        public async void GetNameAsync_HasNoEntries_ReturnsNull()
        {
            // Arrange
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT>());

            // Act
            var result = await _sut.GetNameAsync(DateTime.MinValue, "");

            // Assert
            result.Should().BeNull("because no entry was found");
        }

        [Fact]
        public async void GetNameAsync_HasMultipleEntries_ReturnsLatest()
        {
            // Arrange
            string expected = "Test CPU";
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "CPU Name",
            };
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:05"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "CPU Name",
                REPORT_STRING_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var result = await _sut.GetNameAsync(DateTime.MinValue, "");

            // Assert
            result.Should().NotBeNull("because two entries exist")
                .And.Be(expected);
        }

        [Fact]
        public async void GetLogicalCoresAsync_HasNoEntries_ReturnsNull()
        {
            // Arrange
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT>());

            // Act
            var result = await _sut.GetLogicalCoresAsync(DateTime.MinValue, "");

            // Assert
            result.Should().BeNull("because no entry exists");
        }

        [Fact]
        public async void GetLogicalCoresAsync_HasMultipleEntries_ReturnsLatest()
        {
            // Arrange
            long expected = 8;
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "LogicalCores",
                REPORT_NUMERIC_VALUE = 4
            };
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:05"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "LogicalCores",
                REPORT_NUMERIC_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var result = await _sut.GetLogicalCoresAsync(DateTime.MinValue, "");

            // Assert
            result.Should().NotBeNull("because two entries exist")
                .And.Be(expected);
        }

        [Fact]
        public async void GetPhysicalCoresAsync_HasNoEntries_ReturnsNull()
        {
            // Arrange
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT>());

            // Act
            var result = await _sut.GetPhysicalCoresAsync(DateTime.MinValue, "");

            // Assert
            result.Should().BeNull("because no entry exists");
        }

        [Fact]
        public async void GetPhysicalCoresAsync_HasMultipleEntries_ReturnsLatest()
        {
            // Arrange
            long expected = 8;
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "PhysicalCores",
                REPORT_NUMERIC_VALUE = 4
            };
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:05"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "PhysicalCores",
                REPORT_NUMERIC_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var result = await _sut.GetPhysicalCoresAsync(DateTime.MinValue, "");

            // Assert
            result.Should().NotBeNull("because two entries exist")
                .And.Be(expected);
        }

        [Fact]
        public async void GetMaxFrequencyAsync_HasNoEntries_ReturnsNull()
        {
            // Arrange
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT>());

            // Act
            var result = await _sut.GetMaxFrequencyAsync(DateTime.MinValue, "");

            // Assert
            result.Should().BeNull("because no entry exists");
        }

        [Fact]
        public async void GetMaxFrequencyAsync_HasMultipleEntries_ReturnsLatest()
        {
            // Arrange
            long expected = 20000;
            var firstEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "CPU Max frequency",
                REPORT_NUMERIC_VALUE = 10000
            };
            var secondEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:05"),
                REPORT_TYPE = "CPU_INIT",
                REPORT_KEY = "CPU Max frequency",
                REPORT_NUMERIC_VALUE = expected
            };
            _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<HEALTH_REPORT> { firstEntry, secondEntry });

            // Act
            var result = await _sut.GetMaxFrequencyAsync(DateTime.MinValue, "");

            // Assert
            result.Should().NotBeNull("because two entries exist")
                .And.Be(expected);
        }
    }
}
