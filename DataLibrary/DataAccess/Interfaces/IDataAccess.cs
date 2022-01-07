using DataLibrary.Models.Database;

namespace DataLibrary.DataAccess.Interfaces;

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
    Task<IEnumerable<AFSTEMNING>> GetAfstemningAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_FK_LOGERROR table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<AUDIT_FK_ERROR>> GetAuditFkErrorAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_LOGERROR table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<AUDIT_LOGERROR>> GetAuditLogErrorAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_LOGINFO table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<AUDIT_LOGINFO>> GetAuditLogInfoAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the AUDIT_LOGINFO_TYPES table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<AUDIT_LOGINFO_TYPE>> GetAuditLogInfoTypesAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the COLUMN_VALUE table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<COLUMN_VALUE>> GetColumnValueAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the DEST_TABLE table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<DEST_TABLE>> GetDestTableAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the ENGINE_PROPERTIES table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<ENGINE_PROPERTY>> GetEnginePropertiesAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the EXECUTIONS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<EXECUTION>> GetExecutionAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the HEALTH_REPORT table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<HEALTH_REPORT>> GetHealthReportAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the LOGGING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<LOGGING>> GetLoggingAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the LOGGING_CONTEXT table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<LOGGING_CONTEXT>> GetLoggingContextAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the MANAGERS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<MANAGER>> GetManagersAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the MANAGER_TRACKING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<MANAGER_TRACKING>> GetManagerTrackingAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the MIGRATION_FILE table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<MIGRATION_FILE>> GetMigrationFileAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the SEQUENCE_TRACKING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<SEQUENCE_TRACKING>> GetSequenceTrackingAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the STATEMENT_COLUMNS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<STATEMENT_COLUMN>> GetStatementColumnsAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the STATEMENT_JOINS table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<STATEMENT_JOIN>> GetStatementJoinsAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the STATEMENT_TABLES table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<STATEMENT_TABLE>> GetStatementTablesAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the SYS_HOUSEKEEPING table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<SYS_HOUSEKEEPING>> GetSysHousekeepingAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the SYS_HOUSEKEEPING_UUID table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<SYS_HOUSEKEEPING_UUID>> GetSysHousekeepingUuidAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the TABLE_LOG table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<TABLE_LOG>> GetTableLogAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the V_ENGINE_PROPERTIES table.
    /// </summary>
    /// <remarks>This table is mysterious! It is auto-generated by Entity Framework.</remarks>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<V_ENGINE_PROPERTY>> GetVEnginePropertiesAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the VOTE_COMBINATION table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<VOTE_COMBINATION>> GetVoteCombinationAsync(string connStrKey);
    /// <summary>
    /// Retrieves all entries from the VOTE_RESULT table.
    /// </summary>
    /// <param name="connStrKey">Connection string for the database.</param>
    /// <returns>A queryable list of all entries.</returns>
    Task<IEnumerable<VOTE_RESULT>> GetVoteResultAsync(string connStrKey);
}