using System;
using DesktopUI.Models;

namespace DesktopUI.Helpers;

public class ExecutionAssociationHelper
{
    public bool IsInExecution(ReconciliationDto item, ExecutionDto? e)
        => GetValue(item.Date, e);

    public bool IsInExecution(LogEntryDto item, ExecutionDto? e) 
        => e is null || e.Id == item.ExecutionId;

    private bool GetValue(DateTime date, ExecutionDto? e)
    {
        if (e is null)
        {
            return true;
        }
        else
        {
            return e.EndTime.HasValue
                ? date >= e.StartTime && date <= e.EndTime.Value
                : date >= e.StartTime;
        }
    }
}