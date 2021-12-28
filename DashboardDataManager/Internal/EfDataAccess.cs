using System.Configuration;
using DataLibrary.Internal.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.Internal;

/// <summary>
/// Entity Framework implementation of the <see cref="IDataAccess"/> interface.
/// </summary>
internal class EfDataAccess : IDataAccess
{
    private static string GetConnectionString(string connStrKey)
    {
        return ConfigurationManager.ConnectionStrings[connStrKey].ConnectionString;
    }

    private static DbContextOptions GetDbOptions(string connStrKey)
    {
        string connStr = GetConnectionString(connStrKey);
        return new DbContextOptionsBuilder()
            .UseSqlServer(connStr)
            .Options;
    }

    #region Implementation of IDataAccess

    /// <inheritdoc />
    public IEnumerable<AFSTEMNING> GetAfstemningTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.AFSTEMNING.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<AUDIT_FK_ERROR> GetAuditFkErrorTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.AUDIT_FK_ERROR.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<AUDIT_LOGERROR> GetAuditLogErrorTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.AUDIT_LOGERROR.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<AUDIT_LOGINFO> GetAuditLogInfoTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.AUDIT_LOGINFO.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<AUDIT_LOGINFO_TYPE> GetAuditLogInfoTypesTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.AUDIT_LOGINFO_TYPE.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<COLUMN_VALUE> GetColumnValueTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.COLUMN_VALUE.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<DEST_TABLE> GetDestTableTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.DEST_TABLE.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<ENGINE_PROPERTY> GetEnginePropertiesTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.ENGINE_PROPERTIES.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<EXECUTION> GetExecutionTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.EXECUTIONS.ToList();
    }

    /// <inheritdoc />
    public IEnumerable<HEALTH_REPORT> GetHealthReportTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.HEALTH_REPORT.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<LOGGING> GetLoggingTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.LOGGING.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<LOGGING_CONTEXT> GetLoggingContextTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.LOGGING_CONTEXT.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<MANAGER> GetManagersTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.MANAGERS.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<MANAGER_TRACKING> GetManagerTrackingTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.MANAGER_TRACKING.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<MIGRATION_FILE> GetMigrationFileTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.MIGRATION_FILE.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<SEQUENCE_TRACKING> GetSequenceTrackingTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.SEQUENCE_TRACKING.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<STATEMENT_COLUMN> GetStatementColumnsTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.STATEMENT_COLUMN.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<STATEMENT_JOIN> GetStatementJoinsTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.STATEMENT_JOINS.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<STATEMENT_TABLE> GetStatementTablesTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.STATEMENT_TABLES.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<SYS_HOUSEKEEPING> GetSysHousekeepingTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.SYS_HOUSEKEEPING.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<SYS_HOUSEKEEPING_UUID> GetSysHousekeepingUuidTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.SYS_HOUSEKEEPING_UUID.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<TABLE_LOG> GetTableLogTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.TABLE_LOG.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<V_ENGINE_PROPERTY> GetVEnginePropertiesTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.V_ENGINE_PROPERTIES.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<VOTE_COMBINATION> GetVoteCombinationTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.VOTE_COMBINATION.ToList();    
    }

    /// <inheritdoc />
    public IEnumerable<VOTE_RESULT> GetVoteResultTbl(string connStrKey)
    {
        using var db = new DefaultDbContext(GetDbOptions(connStrKey));
        return db.VOTE_RESULT.ToList();    
    }

    #endregion
}