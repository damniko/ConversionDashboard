using System.Diagnostics;
using System.Text.RegularExpressions;
using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;
using DataLibrary.Models.Database;

namespace DataLibrary.DataAccess;

public class ManagerData : IManagerData
{
    private readonly IDataAccess _db;
    internal List<Manager> Managers { get; } = new(); 

    public ManagerData(IDataAccess db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Manager>> GetSinceAsync(DateTime fromDate, string connStrKey)
    {
        Trace.WriteLine($"{DateTime.Now}: Getting engine properties with fromDate=[{fromDate}]");
        var engineProperties = (from e in await _db.GetEnginePropertiesAsync(connStrKey)
                                where e.TIMESTAMP > fromDate
                                orderby e.TIMESTAMP
                                select e).ToList();

        var modifiedManagers = GetModifiedManagers(engineProperties, Managers);

        return modifiedManagers;
    }

    private List<Manager> GetModifiedManagers(IEnumerable<ENGINE_PROPERTY> engineProps, List<Manager> managers)
    {
        var modifiedManagers = new List<Manager>();
        foreach (var entry in engineProps)
        {
            Manager? manager;
            if (entry.KEY == "START_TIME") // This is the first entry written by a manager
            {
                if (managers.Any(x => x.Name == entry.MANAGER! && x.StartTime == DateTime.Parse(entry.VALUE!)))
                {
                    Trace.WriteLine($"{DateTime.Now}: START_TIME entry for manager {entry.MANAGER} has already been parsed, skipping");
                    continue;
                }
                manager = new() { Name = entry.MANAGER! };
                modifiedManagers.Add(manager);
                managers.Add(manager);
                Trace.WriteLine($"{DateTime.Now}: Created {manager.Name}");
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
                    Trace.WriteLine($"{DateTime.Now}: No matching manager for [{entry.MANAGER}] and key [{entry.KEY}]");
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

    private static DateTime? TryGetDateTime(ENGINE_PROPERTY entry)
    {
        if (DateTime.TryParse(entry?.VALUE, out DateTime result))
        {
            return result;
        }
        return null;
    }

    private static int? TryGetInt(ENGINE_PROPERTY entry)
    {
        if (int.TryParse(entry?.VALUE, out int result))
        {
            return result;
        }
        return null;
    }
}