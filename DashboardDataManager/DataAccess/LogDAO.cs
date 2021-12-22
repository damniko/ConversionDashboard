using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.Helpers;
using DataAccessLibrary.Models;
using DataAccessLibrary.Mappers;

namespace DataAccessLibrary.DataAccess
{
    public class LogDAO
    {
        private readonly IConfigHelper _configHelper;
        private ILogEntryMapper _mapper;

        public LogDAO(IConfigHelper configHelper)
        {
            _configHelper = configHelper;
            // TODO - This should be redone to work with dependency injection (it is problematic since it is internal)
            _mapper = new LogEntryMapper();
        }

        public List<LogEntry> GetAll(string connectionStringKey)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var dbModels = db.LOGGING.ToList();
            var output = _mapper.Map(dbModels);

            return output;
        }

        public List<LogEntry> Get(string connectionStringKey, DateTime fromDate)
        {
            var connectionString = _configHelper.GetConnectionString(connectionStringKey);
            var options = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new DefaultDbContext(options);

            var dbModels = db.LOGGING.Where(x => x.CREATED > fromDate).ToList();
            var output = _mapper.Map(dbModels);

            return output;
        }
    }
}
