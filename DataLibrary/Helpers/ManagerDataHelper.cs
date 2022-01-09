using System.Text.RegularExpressions;
using DataLibrary.Models;
using DataLibrary.Models.Database;
using Microsoft.Extensions.Logging;

namespace DataLibrary.Helpers;

public class ManagerDataHelper
{
    private readonly ILogger<ManagerDataHelper> _logger;

    public ManagerDataHelper(ILogger<ManagerDataHelper> logger)
    {
        _logger = logger;
    }
    
    internal virtual List<Manager> Managers { get; } = new();
    
    internal List<Manager> GetModifiedManagers(IEnumerable<ENGINE_PROPERTY> engineProps)
    {
        var modifiedManagers = new List<Manager>();
        foreach (var entry in engineProps)
        {
            Manager? manager;
            if (entry.KEY == "START_TIME") // This is the first entry written by a manager
            {
                if (Managers.Any(x => x.Name == entry.MANAGER! && x.StartTime == DateTime.Parse(entry.VALUE!)))
                {
                    _logger.LogWarning("START_TIME entry for manager {Manager} has already been parsed, skipping",
                        entry.MANAGER);
                    continue;
                }
                manager = new Manager { Name = entry.MANAGER! };
                modifiedManagers.Add(manager);
                Managers.Add(manager);
                _logger.LogInformation("Created {Manager}", entry.MANAGER);
            }
            else
            {
                // Try to find the last manager that already exists with the same name, which also matches the timestamp
                manager = Managers.LastOrDefault(x => x.Name.Contains(entry.MANAGER!) && x.StartTime <= entry.TIMESTAMP);
                if (manager is null)
                {
                    _logger.LogWarning("There exists no manager associated with entry: (Key=[{Manager}], Value=[{Key}]) - skipping",
                        entry.MANAGER, entry.KEY);
                    continue;
                }
                if (!modifiedManagers.Any(x => x.Name == manager.Name && x.StartTime <= entry.TIMESTAMP))
                {
                    modifiedManagers.Add(manager);
                }
            }
            // add parsed value to mgr
            AddParsedValueToMgr(entry, ref manager);
        }
        return modifiedManagers;
    }

    private void AddParsedValueToMgr(ENGINE_PROPERTY entry, ref Manager manager)
    {
        // Properties
        if (entry.KEY == "START_TIME")
        {
            manager.StartTime ??= TryGetDateTime(entry);
        }
        else if (entry.KEY == "END_TIME")
        {
            manager.EndTime ??= TryGetDateTime(entry);
        }
        else if (Regex.IsMatch(entry.KEY!, "^L.ste r.kker$"))
        {
            manager.RowsRead ??= TryGetInt(entry);
        }
        else if (Regex.IsMatch(entry.KEY!, "^Skrevne r.kker$"))
        {
            manager.RowsWritten ??= TryGetInt(entry);
        }
        // Dictionaries
        else if (entry.KEY!.StartsWith("READ"))
        {
            manager.RowsReadDict.TryAdd(entry.KEY!, int.Parse(entry.VALUE!));
        }
        else if (entry.KEY!.StartsWith("WRITE"))
        {
            manager.RowsWrittenDict.TryAdd(entry.KEY!, int.Parse(entry.VALUE!));
        }
        else if (entry.KEY!.StartsWith("sql_"))
        {
            manager.SqlCostDict.TryAdd(entry.KEY!, int.Parse(entry.VALUE!));
        }
        else if (entry.KEY!.StartsWith("TIME_"))
        {
            manager.TimeDict.TryAdd(entry.KEY!, int.Parse(entry.VALUE!));
        }
    }

    private DateTime? TryGetDateTime(ENGINE_PROPERTY entry)
    {
        if (DateTime.TryParse(entry?.VALUE, out DateTime result))
        {
            return result;
        }
        _logger.LogError("Failed to parse value [{Value}] as DateTime ({Manager})", 
            entry?.VALUE, entry?.MANAGER);
        return null;
    }

    private int? TryGetInt(ENGINE_PROPERTY entry)
    {
        if (int.TryParse(entry?.VALUE, out int result))
        {
            return result;
        }
        _logger.LogError("Failed to parse value [{Value}] as Int ({Manager})", 
            entry?.VALUE, entry?.MANAGER);
        return null;
    }
}