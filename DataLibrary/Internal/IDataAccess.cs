using DataLibrary.Internal.EFModels;

namespace DataLibrary.Internal;

/// <summary>
/// Base interface for implementing Data Access objects.
/// </summary>
public interface IDataAccess
{
    /// <summary>
    /// Retrieves all entries from the AFSTEMNING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<AFSTEMNING> GetAfstemningTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_FK_LOGERROR table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<AUDIT_FK_ERROR> GetAuditFkErrorTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_LOGERROR table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<AUDIT_LOGERROR> GetAuditLogErrorTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_LOGINFO table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<AUDIT_LOGINFO> GetAuditLogInfoTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_LOGINFO_TYPES table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<AUDIT_LOGINFO_TYPE> GetAuditLogInfoTypesTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the COLUMN_VALUE table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<COLUMN_VALUE> GetColumnValueTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the DEST_TABLE table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<DEST_TABLE> GetDestTableTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the ENGINE_PROPERTIES table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<ENGINE_PROPERTY> GetEnginePropertiesTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the EXECUTIONS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<EXECUTION> GetExecutionTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the HEALTH_REPORT table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<HEALTH_REPORT> GetHealthReportTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the LOGGING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<LOGGING> GetLoggingTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the LOGGING_CONTEXT table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<LOGGING_CONTEXT> GetLoggingContextTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the MANAGERS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<MANAGER> GetManagersTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the MANAGER_TRACKING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<MANAGER_TRACKING> GetManagerTrackingTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the MIGRATION_FILE table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<MIGRATION_FILE> GetMigrationFileTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the SEQUENCE_TRACKING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<SEQUENCE_TRACKING> GetSequenceTrackingTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the STATEMENT_COLUMNS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<STATEMENT_COLUMN> GetStatementColumnsTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the STATEMENT_JOINS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<STATEMENT_JOIN> GetStatementJoinsTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the STATEMENT_TABLES table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<STATEMENT_TABLE> GetStatementTablesTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the SYS_HOUSEKEEPING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<SYS_HOUSEKEEPING> GetSysHousekeepingTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the SYS_HOUSEKEEPING_UUID table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<SYS_HOUSEKEEPING_UUID> GetSysHousekeepingUuidTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the TABLE_LOG table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<TABLE_LOG> GetTableLogTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the V_ENGINE_PROPERTIES table.
    /// </summary>
    /// <remarks>This table is mysterious! It is auto-generated by Entity Framework.</remarks>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<V_ENGINE_PROPERTY> GetVEnginePropertiesTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the VOTE_COMBINATION table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<VOTE_COMBINATION> GetVoteCombinationTbl(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the VOTE_RESULT table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    IEnumerable<VOTE_RESULT> GetVoteResultTbl(string connStrKey);
}