using DataLibrary.Helpers;
using DataLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    public class DestTableDAO
    {
        private readonly IConfigHelper _configHelper;

        public DestTableDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public List<DestTable> Get(string connStrKey, string manager)
        {
            string connStr = _configHelper.GetConnectionString(connStrKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connStr)
                .Options;

            using var db = new DefaultDbContext(options);

            var entries = (from d in db.DEST_TABLE
                          where d.MGR == manager
                          select d)
                          .ToList();

            List<DestTable> output = new();
            foreach (var entry in entries)
            {
                if ((string.IsNullOrWhiteSpace(entry.MGR) && string.IsNullOrWhiteSpace(entry.TABLE_NAME)) == false)
                {
                    var destTable = new DestTable
                    {
                        Manager = manager,
                        Table = entry.TABLE_NAME!,
                    };
                    output.Add(destTable);
                }
            }
            return output;
        }
    }
}
