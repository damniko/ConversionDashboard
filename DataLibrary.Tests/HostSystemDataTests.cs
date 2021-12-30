using System;
using System.Collections.Generic;
using DataLibrary.DataAccess;
using DataLibrary.Internal;
using DataLibrary.Internal.EFModels;
using FluentAssertions;
using Moq;
using Xunit;

namespace DataLibrary.Tests
{
    public class HostSystemDataTests
    {
        private readonly HostSystemData _sut;
        private readonly Mock<IDataAccess> _dbMock;

        public HostSystemDataTests()
        {
            _dbMock = new Mock<IDataAccess>();
            _sut = new HostSystemData(_dbMock.Object);
        }

        [Fact]
        public void TryGetUpdatedHostName_NoHostNameEntryExists_ReturnsFalse()
        {
            // Arrange
            var irrelevantEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = _sut._targetReportType,
                REPORT_KEY = _sut._monitorNameKey,
                REPORT_STRING_VALUE = "Test Monitor Name"
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { irrelevantEntry });

            // Act
            var result = _sut.TryGetUpdatedHostName(DateTime.MinValue, out _, "");

            // Assert
            result.Should().BeFalse("because there is no entry with a report key {0}", nameof(_sut._hostNameKey));
        }

        [Fact]
        public void TryGetUpdatedHostName_HostNameEntryExists_ReturnsTrueAndString()
        {
            // Arrange
            string entryValue = "Test Host Name";
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = _sut._targetReportType,
                REPORT_KEY = _sut._hostNameKey,
                REPORT_STRING_VALUE = entryValue
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var result = _sut.TryGetUpdatedHostName(DateTime.MinValue, out string hostName, "");

            // Assert
            result.Should().BeTrue("because there is an entry with a report key {0}", nameof(_sut._hostNameKey));
            hostName.Should().Be(entryValue);
        }

        [Fact]
        public void TryGetUpdatedMonitorName_NoMonitorNameEntryExists_ReturnsFalse()
        {
            // Arrange
            var irrelevantEntry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = _sut._targetReportType,
                REPORT_KEY = _sut._hostNameKey,
                REPORT_STRING_VALUE = "Test Host Name"
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { irrelevantEntry });

            // Act
            var result = _sut.TryGetUpdatedMonitorName(DateTime.MinValue, out _, "");

            // Assert
            result.Should().BeFalse("because there is no entry with a report key {0}", nameof(_sut._monitorNameKey));
        }

        [Fact]
        public void TryGetUpdatedMonitorName_MonitorNameEntryExists_ReturnsTrueAndString()
        {
            // Arrange
            string entryValue = "Test Monitor Name";
            var entry = new HEALTH_REPORT
            {
                LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
                REPORT_TYPE = _sut._targetReportType,
                REPORT_KEY = _sut._monitorNameKey,
                REPORT_STRING_VALUE = entryValue
            };
            _dbMock.Setup(x => x.GetHealthReportTbl(It.IsAny<string>()))
                .Returns(new List<HEALTH_REPORT> { entry });

            // Act
            var result = _sut.TryGetUpdatedMonitorName(DateTime.MinValue, out string monitorName, "");

            // Assert
            result.Should().BeTrue("because there is an entry with a report key {0}", nameof(_sut._monitorNameKey));
            monitorName.Should().Be(entryValue);
        }
    }
}
