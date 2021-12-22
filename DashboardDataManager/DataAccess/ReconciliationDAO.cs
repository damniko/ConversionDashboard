using DataAccessLibrary.Helpers;
using DataAccessLibrary.Internal.Mappers;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLibrary.DataAccess
{
    public class ReconciliationDAO
    {
        private readonly IConfigHelper _configHelper;
        private IReconciliationMapper _mapper;

        public ReconciliationDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
            // TODO - This should be redone to work with dependency injection (it is problematic since it is internal)
            _mapper = new ReconciliationMapper();
        }

        public List<Reconciliation> GetAll(string connectionStringKey)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var dbModels = db.AFSTEMNING.OrderBy(x => x.AFSTEMTDATO).ToList();
            var output = _mapper.Map(dbModels);

            return output;
        }

        public List<Reconciliation> Get(string connectionStringKey, DateTime fromDate)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var dbModels = db.AFSTEMNING.Where(x => x.AFSTEMTDATO > fromDate).OrderBy(x => x.AFSTEMTDATO).ToList();
            var output = _mapper.Map(dbModels);

            return output;
        }
    }
}
