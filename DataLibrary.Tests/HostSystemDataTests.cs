using System;
using System.Collections.Generic;
using DataLibrary.DataAccess;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models.Database;
using FluentAssertions;
using Moq;
using Xunit;

namespace DataLibrary.Tests;

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
    public async void GetHostNameAsync_NoHostNameEntryExists_ReturnsNull()
    {
        // Arrange
        var irrelevantEntry = new HEALTH_REPORT
        {
            LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
            REPORT_TYPE = _sut._targetReportType,
            REPORT_KEY = _sut._monitorNameKey,
            REPORT_STRING_VALUE = "Test Monitor Name"
        };
        _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<HEALTH_REPORT> { irrelevantEntry });

        // Act
        var result = await _sut.GetHostNameAsync(DateTime.MinValue, "");

        // Assert
        result.Should().BeNull("because there is no entry with report key {0}", nameof(_sut._hostNameKey));
    }

    [Fact]
    public async void GetHostNameAsync_HostNameEntryExists_ReturnsString()
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
        _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<HEALTH_REPORT> { entry });

        // Act
        var result = await _sut.GetHostNameAsync(DateTime.MinValue, "");

        // Assert
        result.Should().NotBeNull("because there is an entry with report key {0}", nameof(_sut._hostNameKey))
            .And.Be(entryValue);
    }

    [Fact]
    public async void GetMonitorNameAsync_NoMonitorNameEntryExists_ReturnsNull()
    {
        // Arrange
        var irrelevantEntry = new HEALTH_REPORT
        {
            LOG_TIME = DateTime.Parse("2010/01/01 10:00:00"),
            REPORT_TYPE = _sut._targetReportType,
            REPORT_KEY = _sut._hostNameKey,
            REPORT_STRING_VALUE = "Test Host Name"
        };
        _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<HEALTH_REPORT> { irrelevantEntry });

        // Act
        var result = await _sut.GetMonitorNameAsync(DateTime.MinValue, "");

        // Assert
        result.Should().BeNull("because there is no entry with a report key {0}", nameof(_sut._monitorNameKey));
    }

    [Fact]
    public async void GetMonitorNameAsync_MonitorNameEntryExists_ReturnsString()
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
        _dbMock.Setup(x => x.GetHealthReportAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<HEALTH_REPORT> { entry });

        // Act
        var result = await _sut.GetMonitorNameAsync(DateTime.MinValue, "");

        // Assert
        result.Should().NotBeNull("because there is an entry with report key {0}", nameof(_sut._monitorNameKey))
            .And.Be(entryValue);
    }
}