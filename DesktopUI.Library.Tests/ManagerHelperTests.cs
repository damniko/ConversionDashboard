using System;
using System.Collections.Generic;
using DesktopUI.Helpers;
using DesktopUI.Models;
using FluentAssertions;
using Xunit;

namespace DesktopUI.Tests;

public class ManagerHelperTests
{
    private readonly ManagerHelper _sut;
    
    public ManagerHelperTests()
    {
        _sut = new ManagerHelper();
    }
    
    [Fact]
    public void MergeProperties_OnlyNewData_AddsItToExistingData()
    {
        // Arrange
        List<ManagerDto> existingData = new();
        string expectedName = "manager1";
        List<ManagerDto> newData = new()
        {
            new ManagerDto
            {
                Name = expectedName
            }
        };
        
        // Act
        ManagerHelper.MergeProperties(newData, existingData);
        
        // Assert
        existingData.Should().ContainSingle("because there is one new entry")
            .Which.Name.Should().Be(expectedName);
    }
    
    [Fact]
    public void MergeProperties_NewDataContainsPropertiesForExistingManager_UpdatesExistingManager()
    {
        // Arrange
        string name = "manager1";
        DateTime startTime = DateTime.Parse("2010/01/01 10:00:00");
        ManagerDto manager = new ManagerDto()
        {
            Name = name,
            StartTime = startTime
        };
        List<ManagerDto> existingData = new() { manager };
        
        int rowsRead = 10;
        int rowsWritten = 20;
        DateTime endTime = DateTime.Parse("2010/01/01 10:00:10");
        List<ManagerDto> newData = new()
        {
            new ManagerDto
            {
                Name = name,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten,
                EndTime = endTime
            }
        };
        
        // Act
        ManagerHelper.MergeProperties(newData, existingData);
        
        // Assert
        existingData.Should().ContainSingle("because there is one new entry for a manager which already exists")
            .Which.Should().BeEquivalentTo(new ManagerDto()
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten
            });
    }
    
    [Fact]
    public void MergeProperties_NewDataHasTwoMatchingManagers_UpdatesCorrectManager()
    {
        // Arrange
        string name = "manager1";
        int rowsRead = 30;
        int rowsWritten = 40;
        DateTime startTime = DateTime.Parse("2010/01/01 10:10:00");
        DateTime endTime = DateTime.Parse("2010/01/01 10:10:10");
        ManagerDto firstMgr = new ManagerDto()
        {
            Name = name,
            StartTime = DateTime.Parse("2010/01/01 10:00:00"),
            EndTime = DateTime.Parse("2010/01/01 10:00:10"),
            RowsRead = 10,
            RowsWritten = 20
        };
        ManagerDto secondMgr = new ManagerDto()
        {
            Name = name,
            StartTime = startTime
        };
        List<ManagerDto> existingData = new() { firstMgr, secondMgr };
        List<ManagerDto> newData = new()
        {
            new ManagerDto
            {
                Name = name,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten,
                EndTime = endTime
            }
        };
        
        // Act
        ManagerHelper.MergeProperties(newData, existingData);
        
        // Assert
        existingData.Should().HaveCount(2, "because there are two existing managers, and new data for one of these")
            .And.SatisfyRespectively(
                mgr => mgr.Should().BeEquivalentTo(new ManagerDto()
                {
                    Name = name,
                    StartTime = DateTime.Parse("2010/01/01 10:00:00"),
                    EndTime = DateTime.Parse("2010/01/01 10:00:10"),
                    RowsRead = 10,
                    RowsWritten = 20
                }),
                mgr => mgr.Should().BeEquivalentTo(new ManagerDto()
                {
                    Name = name,
                    StartTime = startTime,
                    RowsRead = rowsRead,
                    RowsWritten = rowsWritten,
                    EndTime = endTime
                }));
    }
    
    [Fact]
    public void MergeProperties_NewDataHasDataForFirstOfTwoMatchingManagers_UpdatesFirstManager()
    {
        // Arrange
        string name = "manager1";
        DateTime startTime = DateTime.Parse("2010/01/01 10:00:00");
        int rowsRead = 10;
        int rowsWritten = 20;
        DateTime endTime = DateTime.Parse("2010/01/01 10:00:10");
        ManagerDto firstMgr = new ManagerDto()
        {
            Name = name,
            StartTime = startTime,
        };
        ManagerDto secondMgr = new ManagerDto()
        {
            Name = name,
            StartTime = DateTime.Parse("2010/01/01 10:10:00"),
            EndTime = DateTime.Parse("2010/01/01 10:10:10"),
            RowsRead = 30,
            RowsWritten = 40
        };
        List<ManagerDto> existingData = new() { firstMgr, secondMgr };

        List<ManagerDto> newData = new()
        {
            new ManagerDto
            {
                Name = name,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten,
                EndTime = endTime
            }
        };
        
        // Act
        ManagerHelper.MergeProperties(newData, existingData);
        
        // Assert
        existingData.Should().HaveCount(2, "because there are two existing managers, and new data for one of these")
            .And.SatisfyRespectively(
                mgr => mgr.Should().BeEquivalentTo(new ManagerDto()
                {
                    Name = name,
                    StartTime = startTime,
                    EndTime = endTime,
                    RowsRead = rowsRead,
                    RowsWritten = rowsWritten
                }),
                mgr => mgr.Should().BeEquivalentTo(new ManagerDto()
                {
                    Name = name,
                    StartTime = DateTime.Parse("2010/01/01 10:10:00"),
                    EndTime = DateTime.Parse("2010/01/01 10:10:10"),
                    RowsRead = 30,
                    RowsWritten = 40
                }));
    }
    
    [Fact]
    public void MergeProperties_NewDataDoesNotHaveEndTimeAndMatchesOneManager_UpdatesManager()
    {
        // Arrange
        string name = "manager1";
        DateTime startTime = DateTime.Parse("2010/01/01 10:00:00");
        int rowsRead = 10;
        int rowsWritten = 20;
        ManagerDto manager = new()
        {
            Name = name,
            StartTime = startTime
        };
        List<ManagerDto> existingData = new() { manager };
        List<ManagerDto> newData = new()
        {
            new()
            {
                Name = name,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten
            }
        };
        
        // Act
        ManagerHelper.MergeProperties(newData, existingData);
        
        // Assert
        existingData.Should().ContainSingle("because there is data for exactly one manager")
            .Which.Should().BeEquivalentTo(new ManagerDto()
            {
                Name = name,
                StartTime = startTime,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten
            });
    }
    
    [Fact]
    public void MergeProperties_NewDataDoesNotHaveEndTimeAndMatchesTwoManagers_UpdatesCorrectManager()
    {
        // Arrange
        string name = "manager1";
        ManagerDto firstMgr = new()
        {
            Name = name,
            StartTime = DateTime.Parse("2010/01/01 10:00:00"),
            EndTime = DateTime.Parse("2010/01/01 10:00:10"),
            RowsRead = 10,
            RowsWritten = 20
        };
        DateTime startTime = DateTime.Parse("2010/01/01 10:10:00");
        int rowsRead = 30;
        int rowsWritten = 40;
        ManagerDto secondMgr = new()
        {
            Name = name,
            StartTime = startTime
        };
        List<ManagerDto> existingData = new() { firstMgr, secondMgr };
        List<ManagerDto> newData = new()
        {
            new()
            {
                Name = name,
                RowsRead = rowsRead,
                RowsWritten = rowsWritten
            }
        };
        
        // Act
        ManagerHelper.MergeProperties(newData, existingData);
        
        // Assert
        existingData.Should().HaveCount(2, "because there are two existing managers, and new data for one of these")
            .And.SatisfyRespectively(
                mgr => mgr.Should().BeEquivalentTo(new ManagerDto()
                {
                    Name = name,
                    StartTime = DateTime.Parse("2010/01/01 10:00:00"),
                    EndTime = DateTime.Parse("2010/01/01 10:00:10"),
                    RowsRead = 10,
                    RowsWritten = 20
                }),
                mgr => mgr.Should().BeEquivalentTo(new ManagerDto()
                {
                    Name = name,
                    StartTime = startTime,
                    RowsRead = rowsRead,
                    RowsWritten = rowsWritten
                }));
    }
}