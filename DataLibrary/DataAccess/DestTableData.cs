using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Internal;
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
            var data = _db.GetDestTableTbl(connStrKey);

            var output = (from d in data
                          where d.MGR == manager
                          select new DestTable
                          {
                              Manager = d.MGR,
                              Table = d.TABLE_NAME
                          }).ToList();

            return output;
        }
    }
}
