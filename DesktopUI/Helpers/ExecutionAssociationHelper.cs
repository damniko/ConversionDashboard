using DesktopUI.Models;

namespace DesktopUI.Helpers
{
    public class ExecutionAssociationHelper
    {
        public bool IsInExecution(ReconciliationDto item, ExecutionDto? execution)
        {
            if (execution is null)
            {
                return true;
            }
            else
            {
                return execution.EndTime.HasValue
                    ? item.Date >= execution.StartTime && item.Date <= execution.EndTime.Value
                    : item.Date >= execution.StartTime;
            }
        }
    }
}
