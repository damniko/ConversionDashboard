using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Get entries from MANAGERS table (use ROW_ID instead of fromDate)
        // Use name to get entries from ENGINE_PROPERTIES ordered by TIMESTAMP
        // Get first entry with END_TIME and use this to delimit the entries 
        // (Remove entries where TIMESTAMP > END_TIME, possibly with some padding)

        public List<Manager> Get(string connectionStringKey, int lastRowId)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            List<Manager> output = new();

            List<MANAGER> managers;
            List<ENGINE_PROPERTY> allProps;
            using (var db = new DefaultDbContext(options))
            {
                managers = db.MANAGERS.Where(m => m.ROW_ID > lastRowId).ToList();
                allProps = db.ENGINE_PROPERTIES.OrderBy(p => p.TIMESTAMP).ToList();
            }

            foreach (var manager in managers)
            {
                var managerProps = allProps
                    .Where(p => p.MANAGER!.Contains(manager.MANAGER_NAME!));

                var endTimeEntry = managerProps.FirstOrDefault(p => p.KEY == "END_TIME");
                DateTime endTime = DateTime.Parse(endTimeEntry?.VALUE ?? string.Empty);

                var props = managerProps
                    .Where(p => p.TIMESTAMP.HasValue && p.TIMESTAMP.Value <= endTime.AddSeconds(10))
                    .OrderBy(p => p.TIMESTAMP)
                    .ToList();

                var startTimeEntry = props.FirstOrDefault(p => p.KEY == "START_TIME");
                DateTime startTime = DateTime.Parse(startTimeEntry?.VALUE ?? string.Empty);

                var rowsReadEntry = props.FirstOrDefault(p => p.KEY == "Læste rækker");
                int rowsRead = int.Parse(rowsReadEntry?.VALUE!);
                var rowsWrittenEntry = props.FirstOrDefault(p => p.KEY == "Skrevne rækker");
                int rowsWritten = int.Parse(rowsWrittenEntry?.VALUE!);

                var timeEntries = props
                    .Where(p => p.KEY!.StartsWith("TIME_"))
                    .DistinctBy(p => p.KEY)
                    .ToDictionary(p => p.KEY!, p => int.Parse(p.VALUE!));
                var sqlEntries = props
                    .Where(p => p.KEY!.StartsWith("sql_"))
                    .DistinctBy(p => p.KEY)
                    .ToDictionary(p => p.KEY!, p => int.Parse(p.VALUE!));
                var readEntries = props
                    .Where(p => p.KEY!.StartsWith("READ"))
                    .DistinctBy(p => p.KEY)
                    .ToDictionary(p => p.KEY!, p => int.Parse(p.VALUE!));
                var writtenEntries = props
                    .Where(p => p.KEY!.StartsWith("WRITE"))
                    .DistinctBy(p => p.KEY)
                    .ToDictionary(p => p.KEY!, p => int.Parse(p.VALUE!));

                output.Add(new Manager
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    Name = manager.MANAGER_NAME ?? "",
                    RowId = manager.ROW_ID.GetValueOrDefault(),
                    RowsRead = rowsRead,
                    RowsWritten = rowsWritten,
                    RowsReadDict = readEntries,
                    RowsWrittenDict = writtenEntries,
                    SqlCostDict = sqlEntries,
                    TimeDict = timeEntries
                });
            }

            return output;
        }

    }
}
