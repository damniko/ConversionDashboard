using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Moq;
using DataLibrary.Helpers;
using DataLibrary.Models.Database;
using DataLibrary.Models;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DataLibrary.Tests;

public class ManagerDataHelperTests
{
    private readonly ManagerDataHelper _sut;
    private readonly Mock<ILogger<ManagerDataHelper>> _loggerMock;

    public ManagerDataHelperTests()
    {
        _loggerMock = new Mock<ILogger<ManagerDataHelper>>();
        _sut = new ManagerDataHelper(_loggerMock.Object);
    }

    [Fact]
    public void NoManagers_AllEntriesAtOnce_CreatesManager()
    {
        // Arrange
        const string name = "manager", nameAlt = "manager,rnd_2032045042";
        DateTime startTime = DateTime.Parse("2021-10-28 15:07:23.277"), endTime = DateTime.Parse("2021-10-28 15:07:32.747");
        const int read = 1, written = 4;
        const int dictCount = 2;
        var engineProperties = new List<ENGINE_PROPERTY>
        {
            new() { MANAGER = nameAlt, KEY = "START_TIME", VALUE = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                TIMESTAMP = DateTime.Parse("2021-10-28 15:07:23.347") },
            new() { MANAGER = name, KEY = "TIME_HOUSEKEEPING", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:25.617") },
            new() { MANAGER = name, KEY = "sql_0_398402984", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.043") },
            new() { MANAGER = name, KEY = "sql_1_123456789", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.055") },
            new() { MANAGER = name, KEY = "TIME_SELECT_TO_TOTAL_COUNT", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.073") },
            new() { MANAGER = nameAlt, KEY = "runtimeConversion", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.467") },
            new() { MANAGER = name, KEY = "READ[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.493") },
            new() { MANAGER = name, KEY = "Læste rækker", VALUE = read.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.533") },
            new() { MANAGER = name, KEY = "Skrevne rækker", VALUE = written.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.567") },
            new() { MANAGER = name, KEY = "FinishedExecution", VALUE = "yes", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.750") },
            new() { MANAGER = name, KEY = "END_TIME", VALUE = endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.810") },
            new() { MANAGER = name, KEY = "READ[TOTAL]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.897") },
            new() { MANAGER = name, KEY = "WRITE[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.923") },
            new() { MANAGER = name, KEY = "WRITE[Y]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.953") },
            new() { MANAGER = nameAlt, KEY = "runtimeOverall", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:34.180") },
        };

        // Act
        var managers = _sut.GetModifiedManagers(engineProperties);

        // Assert
        using (new AssertionScope())
        {
            managers.Should().ContainSingle("because there are only entries for one manager")
                .Which.Should().Match<Manager>((m) => 
                    m.Name == nameAlt
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
    public void OneManager_OneEntryWithoutStartTime_FindsAndUpdatesExistingManager()
    {
        // Arrange
        const string name = "manager", nameAlt = "manager,rnd_2032045042";
        DateTime startTime = DateTime.Parse("2021-10-28 15:07:23.277"), endTime = DateTime.Parse("2021-10-28 15:07:32.747");
        const int read = 1, written = 4;
        const int dictCount = 2;
        var engineProperties = new List<ENGINE_PROPERTY>
        {
            new() { MANAGER = name, KEY = "TIME_HOUSEKEEPING", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:25.617") },
            new() { MANAGER = name, KEY = "sql_0_398402984", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.043") },
            new() { MANAGER = name, KEY = "sql_1_123456789", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.055") },
            new() { MANAGER = name, KEY = "TIME_SELECT_TO_TOTAL_COUNT", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:27.073") },
            new() { MANAGER = nameAlt, KEY = "runtimeConversion", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.467") },
            new() { MANAGER = name, KEY = "READ[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.493") },
            new() { MANAGER = name, KEY = "Læste rækker", VALUE = read.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.533") },
            new() { MANAGER = name, KEY = "Skrevne rækker", VALUE = written.ToString(), TIMESTAMP = DateTime.Parse("2021-10-28 15:07:30.567") },
            new() { MANAGER = name, KEY = "FinishedExecution", VALUE = "yes", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.750") },
            new() { MANAGER = name, KEY = "END_TIME", VALUE = endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                TIMESTAMP = DateTime.Parse("2021-10-28 15:07:32.810") },
            new() { MANAGER = name, KEY = "READ[TOTAL]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.897") },
            new() { MANAGER = name, KEY = "WRITE[X]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.923") },
            new() { MANAGER = name, KEY = "WRITE[Y]", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:33.953") },
            new() { MANAGER = nameAlt, KEY = "runtimeOverall", VALUE = "1", TIMESTAMP = DateTime.Parse("2021-10-28 15:07:34.180") },
        };
        _sut.Managers.Add(new Manager { Name = nameAlt, StartTime = startTime });
            
        // Act
        var managers = _sut.GetModifiedManagers(engineProperties);

        // Assert
        using (new AssertionScope())
        {
            managers.Should().ContainSingle("because there are only entries for one manager")
                .Which.Should().Match<Manager>((m) => 
                    m.Name == nameAlt
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
    public void OneManagerFromPreviousExecution_AllEntriesAtOnce_CreatesSecondManager()
    {
        // Arrange
        const string name = "manager", nameAlt = "manager,rnd_2032045042";
        DateTime startTime = DateTime.Parse("2021-10-28 16:13:45.801"),
            endTime = DateTime.Parse("2021-10-28 16:13:56.318");
        const int read = 1, written = 4;
        _sut.Managers.Add(new Manager
        {
            Name = nameAlt,
            StartTime = DateTime.Parse("2021-10-28 15:07:23.277"),
            EndTime = DateTime.Parse("2021-10-28 15:07:32.747")
        });
        var engineProperties = new List<ENGINE_PROPERTY>
        {
            new() { MANAGER = nameAlt, KEY = "START_TIME", VALUE = startTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                TIMESTAMP = DateTime.Parse("2021-10-28 16:13:45.853") },
            new() { MANAGER = name, KEY = "Læste rækker", VALUE = read.ToString(), 
                TIMESTAMP = DateTime.Parse("2021-10-28 16:13:53.060") },
            new() { MANAGER = name, KEY = "Skrevne rækker", VALUE = written.ToString(), 
                TIMESTAMP = DateTime.Parse("2021-10-28 16:13:53.090") },
            new() { MANAGER = name, KEY = "END_TIME", VALUE = endTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), 
                TIMESTAMP = DateTime.Parse("2021-10-28 16:13:56.353") },
        };

        // Act
        var managers = _sut.GetModifiedManagers(engineProperties);

        // Assert
        _sut.Managers.Should().HaveCount(2, "because the existing manager is from a previous execution");
        managers.Should().ContainSingle("because there are only engine properties for one manager")
            .Which.Should().BeEquivalentTo(new Manager
            {
                Name = nameAlt,
                StartTime = startTime,
                EndTime = endTime,
                RowsRead = read,
                RowsWritten = written
            });
    }

    [Fact]
    public void TwoManagers_OneEntryForSecondManager_FindsAndUpdatesSecondManager()
    {
        // Arrange
        const string name = "manager", nameAlt = "manager,rnd_2032045042";
        DateTime startTime = DateTime.Parse("2021-10-28 16:13:45.801");
        const int read = 1, written = 4;
        var mgr1 = new Manager
        {
            Name = nameAlt,
            StartTime = DateTime.Parse("2021-10-28 15:07:23.277"),
            EndTime = DateTime.Parse("2021-10-28 15:07:32.747")
        };
        var mgr2 = new Manager
        {
            Name = nameAlt,
            StartTime = startTime
        };
        _sut.Managers.AddRange(new List<Manager> { mgr1, mgr2 });
        var engineProperties = new List<ENGINE_PROPERTY>
        {
            new() { MANAGER = name, KEY = "Læste rækker", VALUE = read.ToString(),
                TIMESTAMP = DateTime.Parse("2021-10-28 16:13:53.060") },
            new() { MANAGER = name, KEY = "Skrevne rækker", VALUE = written.ToString(),
                TIMESTAMP = DateTime.Parse("2021-10-28 16:13:53.090") },
        };

        // Act
        var managers = _sut.GetModifiedManagers(engineProperties);

        // Assert
        _sut.Managers.Should().HaveCount(2, "because the entries only contain data for existing managers");
        managers.Should().ContainSingle("because there are only engine properties for one manager")
            .Which.Should().BeEquivalentTo(new Manager
            {
                Name = nameAlt,
                StartTime = startTime,
                RowsRead = read,
                RowsWritten = written
            });
        _loggerMock.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void NoManagers_OneEntryWithNoStartTime_LogsWarning()
    {
        // Arrange
        var engineProperties = new List<ENGINE_PROPERTY>
        {
            new() { MANAGER = "Scripts", KEY = "script.sql_1", VALUE = "true", TIMESTAMP = DateTime.Parse("2021-10-28 15:09:32.083") }
        };
        
        // Act
        var managers = _sut.GetModifiedManagers(engineProperties);
        
        // Assert
        managers.Should().BeEmpty("because the provided entry is not associated with any manager");
        _loggerMock.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once, "Expected a warning to be logged.");
    }
    
    [Fact]
    public void NoManagers_DuplicateStartTimeEntry_LogsWarning()
    {
        // Arrange
        var engineProperties = new List<ENGINE_PROPERTY>
        {
            new() { MANAGER = "manager", KEY = "START_TIME", VALUE = "2021-10-28 15:07:23.277", TIMESTAMP = DateTime.Parse("2021-10-28 15:09:32.083") }
        };
        
        // Act
        var managers1 = _sut.GetModifiedManagers(engineProperties);
        var managers2 = _sut.GetModifiedManagers(engineProperties);
        
        // Assert
        managers1.Should().ContainSingle("because there is one start time entry");
        managers2.Should().BeEmpty("because the provided entry is a duplicate of one that has already been parsed");
        _loggerMock.Verify(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.Once, "Expected a warning to be logged.");
    }
}