using System.Text.RegularExpressions;
using DataLibrary.Helpers;
using DataLibrary.Internal.EntityModels;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class ManagerDAO
    {
        private readonly IConfigHelper _configHelper;

        public ManagerDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<Manager> Get(string connStrKey, DateTime fromDate)
        {
            string connStr = _configHelper.GetConnectionString(connStrKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connStr)
                .Options;

            using var db = new DefaultDbContext(options);

            var engineProperties = (from e in db.ENGINE_PROPERTIES
                                    where e.TIMESTAMP.HasValue && e.TIMESTAMP.Value > fromDate
                                    orderby e.TIMESTAMP
                                    select e)
                                    .ToList();

            List<string> managerNames = engineProperties
                .Where(e => e.KEY == "START_TIME")
                .Select(e => e.MANAGER!)
                .ToList();

            List<Manager> output = GetManagers(managerNames, engineProperties);

            return output;
        }

        private static List<Manager> GetManagers(List<string> names, List<ENGINE_PROPERTY> engineProperties)
        {
            List<Manager> output = new();

            foreach (var name in names)
            {
                Manager manager = new() { Name = name };

                var properties = engineProperties
                    .Where(e => e.MANAGER == name)
                    .ToList();

                foreach (var entry in properties)
                {
                    // Properties
                    if (entry.KEY == "START_TIME")
                    {
                        // If START_TIME is already set, the rest of the entries are for other executions
                        if (manager.StartTime.HasValue)
                        {
                            break;
                        }
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
                    engineProperties.Remove(entry);
                }
                output.Add(manager);
            }
            return output;
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
}
