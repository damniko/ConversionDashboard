using System;
using DesktopUI.Helpers;
using DesktopUI.Models;
using FluentAssertions;
using Xunit;

namespace DesktopUI.Tests
{
    public class ExecutionAssociationHelperTests
    {
        private readonly ExecutionAssociationHelper _sut;

        public ExecutionAssociationHelperTests()
        {
            _sut = new ExecutionAssociationHelper();
        }

        [Fact]
        public void IsInExecution_NoEndTimeAndReconciliationIsAfterStartTime_ReturnsTrue()
        {
            // Arrange
            var execution = new ExecutionDto
            {
                StartTime = DateTime.Parse("2010/01/01 10:00:00")
            };
            var reconciliation = new ReconciliationDto
            {
                Date = DateTime.Parse("2010/01/01 10:30:00")
            };

            // Act
            bool result = _sut.IsInExecution(reconciliation, execution);

            // Assert
            result.Should().BeTrue("because the execution only has a start time which is earlier than the date of the given reconciliation");
        }

        [Fact]
        public void IsInExecution_NoEndTimeAndReconciliationIsBeforeStartTime_ReturnsFalse()
        {
            // Arrange
            var execution = new ExecutionDto
            {
                StartTime = DateTime.Parse("2010/01/01 10:00:00")
            };
            var reconciliation = new ReconciliationDto
            {
                Date = DateTime.Parse("2010/01/01 09:00:00")
            };

            // Act
            bool result = _sut.IsInExecution(reconciliation, execution);

            // Assert
            result.Should().BeFalse("because the execution only has a start time which is later than the date of the given reconciliation");
        }

        [Fact]
        public void IsInExecution_ReconciliationIsBeforeExecution_ReturnsFalse()
        {
            // Arrange
            var execution = new ExecutionDto
            {
                StartTime = DateTime.Parse("2010/01/01 10:00:00"),
                EndTime = DateTime.Parse("2010/01/01 11:00:00")
            };
            var reconciliation = new ReconciliationDto
            {
                Date = DateTime.Parse("2010/01/01 09:00:00")
            };

            // Act
            bool result = _sut.IsInExecution(reconciliation, execution);

            // Assert
            result.Should().BeFalse("because the date of the given reconciliation is before the execution");
        }

        [Fact]
        public void IsInExecution_ReconciliationIsWithinExecution_ReturnsTrue()
        {
            // Arrange
            var execution = new ExecutionDto
            {
                StartTime = DateTime.Parse("2010/01/01 10:00:00"),
                EndTime = DateTime.Parse("2010/01/01 11:00:00")
            };
            var reconciliation = new ReconciliationDto
            {
                Date = DateTime.Parse("2010/01/01 10:30:00")
            };

            // Act
            bool result = _sut.IsInExecution(reconciliation, execution);

            // Assert
            result.Should().BeTrue("because the date of the given reconciliation is within the execution");
        }

        [Fact]
        public void IsInExecution_ReconciliationIsAfterExecution_ReturnsFalse()
        {
            // Arrange
            var execution = new ExecutionDto
            {
                StartTime = DateTime.Parse("2010/01/01 10:00:00"),
                EndTime = DateTime.Parse("2010/01/01 11:00:00")
            };
            var reconciliation = new ReconciliationDto
            {
                Date = DateTime.Parse("2010/01/01 12:00:00")
            };

            // Act
            bool result = _sut.IsInExecution(reconciliation, execution);

            // Assert
            result.Should().BeFalse("because the date of the given reconciliation is after the execution");
        }

        [Fact]
        public void IsInExecution_ExecutionIsNull_ReturnsTrue()
        {
            // Arrange
            var reconciliation = new ReconciliationDto
            {
                Date = DateTime.Parse("2010/01/01 10:00:00")
            };

            // Act
            bool result = _sut.IsInExecution(reconciliation, null);

            // Assert
            result.Should().BeTrue("because the given execution is null");
        }
    }
}