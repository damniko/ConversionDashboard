using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models;

namespace DataLibrary.DataAccess
{
    public class DestTableData : IDestTableData
    {
        private readonly IDataAccess _db;

        public DestTableData(IDataAccess db)
        {
            _db = db;
        }

        public List<DestTable> GetDestTablesForManager(string manager, string connStrKey)
        {
            var output = (from d in _db.GetDestTableTbl(connStrKey)
                          where d.MGR == manager
                          select new DestTable
                          {
                              Manager = d.MGR!,
                              Table = d.TABLE_NAME!
                          }).ToList();

            return output;
        }
    }
}
