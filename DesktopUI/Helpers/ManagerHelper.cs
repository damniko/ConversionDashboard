using System.Collections.Generic;
using System.Linq;
using DesktopUI.Models;

namespace DesktopUI.Helpers;

public class ManagerHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newData"></param>
    /// <param name="data"></param>
    public static void MergeProperties(IEnumerable<ManagerDto> newData, List<ManagerDto> data)
    {
        // Add change to ManagerData: find latest START_TIME for a manager,
        // such that they are always assigned a start_time.
        foreach (ManagerDto newMgr in newData)
        {
            var mgr = data.FirstOrDefault(x => IsMatch(newMgr, x));
            if (mgr is null)
            {
                data.Add(newMgr);
            }
            else
            {
                mgr.StartTime ??= newMgr.StartTime;
                mgr.EndTime ??= newMgr.EndTime;
                mgr.RowsRead ??= newMgr.RowsRead;
                mgr.RowsWritten ??= newMgr.RowsWritten;
                foreach ((string key, int value) in newMgr.TimeDict)
                {
                    mgr.TimeDict.TryAdd(key, value);
                }
                foreach ((string key, int value) in newMgr.SqlCostDict)
                {
                    mgr.SqlCostDict.TryAdd(key, value);
                }
                foreach ((string key, int value) in newMgr.RowsReadDict)
                {
                    mgr.RowsReadDict.TryAdd(key, value);
                }                
                foreach ((string key, int value) in newMgr.RowsWrittenDict)
                {
                    mgr.RowsWrittenDict.TryAdd(key, value);
                }
            }
        }
    }

    private static bool IsMatch(ManagerDto newMgr, ManagerDto mgr)
    {
        return newMgr.Name == mgr.Name
            && !(mgr.RowsRead.HasValue && 
                 mgr.RowsWritten.HasValue && 
                 mgr.EndTime.HasValue && 
                 mgr.StartTime.HasValue &&
                 mgr.TimeDict.Count > 0 &&
                 mgr.SqlCostDict.Count > 0 &&
                 mgr.RowsReadDict.Count > 0 &&
                 mgr.RowsWrittenDict.Count > 0);
    }
}