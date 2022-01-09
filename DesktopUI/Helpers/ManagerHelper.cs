using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataLibrary.Models.Database;
using DesktopUI.Models;

namespace DesktopUI.Helpers;

public class ManagerHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="engineProps">New engine properties to parse.</param>
    /// <param name="managers">Existing managers.</param>
    public void MergeEngineProperties(IEnumerable<ENGINE_PROPERTY> engineProps, List<ManagerDto> managers)
    {
        foreach (var entry in engineProps)
        {
            ManagerDto? manager;
            if (entry.KEY == "START_TIME") // This is the first entry written by a manager
            {
                manager = new() { Name = entry.MANAGER! };
                managers.Add(manager);
            }
            else
            {
                // Try to find the last manager that already exists with the same name, which also matches the timestamp
                manager = managers.LastOrDefault(x => x.Name.Contains(entry.MANAGER!) && x.StartTime <= entry.TIMESTAMP);
                if (manager is null)
                {
                    // Hmm, then we have a property for a manager that has not yet written a START_TIME entry.
                    // For now, we will log it and move on (it may be a 'Scripts' entry which is not super useful)
                    // TODO - log this
                    continue;
                }
            }
            // add parsed value to mgr
            AddParsedValueToMgr(entry, ref manager);
        }
    }

    private void AddParsedValueToMgr(ENGINE_PROPERTY entry, ref ManagerDto manager)
    {
        // Properties
        if (entry.KEY == "START_TIME")
        {
            manager.StartTime = TryGetDateTime(entry);
        }
        else if (entry.KEY == "END_TIME")
        {
            manager.EndTime = TryGetDateTime(entry);
        }
        else if (Regex.IsMatch(entry.KEY!, "^L.ste r.kker$"))
        {
            manager.RowsRead = TryGetInt(entry);
        }
        else if (Regex.IsMatch(entry.KEY!, "^Skrevne r.kker$"))
        {
            manager.RowsWritten = TryGetInt(entry);
        }
        // Dictionaries
        else if (entry.KEY!.StartsWith("READ"))
        {
            manager.RowsReadDict.Add(entry.KEY!, int.Parse(entry.VALUE!));
        }
        else if (entry.KEY!.StartsWith("WRITE"))
        {
            manager.RowsWrittenDict.Add(entry.KEY!, int.Parse(entry.VALUE!));
        }
        else if (entry.KEY!.StartsWith("sql_"))
        {
            manager.SqlCostDict.Add(entry.KEY!, int.Parse(entry.VALUE!));
        }
        else if (entry.KEY!.StartsWith("TIME_"))
        {
            manager.TimeDict.Add(entry.KEY!, int.Parse(entry.VALUE!));
        }
    }

    private DateTime? TryGetDateTime(ENGINE_PROPERTY entry)
    {
        if (DateTime.TryParse(entry?.VALUE, out DateTime result))
        {
            return result;
        }
        return null;
    }

    private int? TryGetInt(ENGINE_PROPERTY entry)
    {
        if (int.TryParse(entry?.VALUE, out int result))
        {
            return result;
        }
        return null;
    }
}