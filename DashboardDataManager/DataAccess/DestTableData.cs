using DataLibrary.Internal;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class DestTableData
    {
        internal IDataAccess Db { get; } = new EfDataAccess();
        
        public List<DestTable> GetDestTablesForManager(string manager, string connStrKey)
        {
            List<DestTable> output = new();

            var data = Db.GetDestTableTbl(connStrKey);
            var entries = (from d in data
                           where d.MGR == manager
                           select d)
                           .ToList();

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
