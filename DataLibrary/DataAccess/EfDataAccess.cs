using DataLibrary.DataAccess.Interfaces;
using DataLibrary.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataLibrary.DataAccess
{
    /// <summary>
    /// Entity Framework implementation of the <see cref="IDataAccess"/> interface.
    /// </summary>
    public class EfDataAccess : IDataAccess
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EfDataAccess> _logger;

        public EfDataAccess(IConfiguration config, ILogger<EfDataAccess> logger)
        {
            _config = config;
            _logger = logger;
        }

        private string GetConnectionString(string connStrKey)
        {
            return _config.GetConnectionString(connStrKey);
        }

        private DbContextOptions GetDbOptions(string connStrKey)
        {
            string connStr = GetConnectionString(connStrKey);
            return new DbContextOptionsBuilder()
                .UseSqlServer(connStr)
                .Options;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AFSTEMNING>> GetAfstemningAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.AFSTEMNING.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AUDIT_FK_ERROR>> GetAuditFkErrorAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.AUDIT_FK_ERROR.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AUDIT_LOGERROR>> GetAuditLogErrorAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.AUDIT_LOGERROR.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AUDIT_LOGINFO>> GetAuditLogInfoAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.AUDIT_LOGINFO.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AUDIT_LOGINFO_TYPE>> GetAuditLogInfoTypesAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.AUDIT_LOGINFO_TYPE.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<COLUMN_VALUE>> GetColumnValueAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.COLUMN_VALUE.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DEST_TABLE>> GetDestTableAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.DEST_TABLE.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ENGINE_PROPERTY>> GetEnginePropertiesAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.ENGINE_PROPERTIES.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EXECUTION>> GetExecutionAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.EXECUTIONS.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<HEALTH_REPORT>> GetHealthReportAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.HEALTH_REPORT.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LOGGING>> GetLoggingAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.LOGGING.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LOGGING_CONTEXT>> GetLoggingContextAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.LOGGING_CONTEXT.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MANAGER>> GetManagersAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            try
            {
                return await db.MANAGERS.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Tried to access the {MANAGERS} table, but it does not exist in the selected database.", nameof(db.MANAGERS));
                return new List<MANAGER>();
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MANAGER_TRACKING>> GetManagerTrackingAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.MANAGER_TRACKING.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<MIGRATION_FILE>> GetMigrationFileAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.MIGRATION_FILE.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SEQUENCE_TRACKING>> GetSequenceTrackingAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.SEQUENCE_TRACKING.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<STATEMENT_COLUMN>> GetStatementColumnsAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.STATEMENT_COLUMN.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<STATEMENT_JOIN>> GetStatementJoinsAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.STATEMENT_JOINS.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<STATEMENT_TABLE>> GetStatementTablesAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.STATEMENT_TABLES.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SYS_HOUSEKEEPING>> GetSysHousekeepingAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.SYS_HOUSEKEEPING.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SYS_HOUSEKEEPING_UUID>> GetSysHousekeepingUuidAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.SYS_HOUSEKEEPING_UUID.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TABLE_LOG>> GetTableLogAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.TABLE_LOG.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<V_ENGINE_PROPERTY>> GetVEnginePropertiesAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.V_ENGINE_PROPERTIES.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VOTE_COMBINATION>> GetVoteCombinationAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.VOTE_COMBINATION.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VOTE_RESULT>> GetVoteResultAsync(string connStrKey)
        {
            using var db = new DefaultDbContext(GetDbOptions(connStrKey));
            return await db.VOTE_RESULT.ToListAsync();
        }
    }
}
