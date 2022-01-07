using DataLibrary.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.DataAccess
{
    /// <summary>
    /// Entity Framework scaffold of the ANS_CUSTOM_2 database.
    /// </summary>
    public partial class DefaultDbContext : DbContext
    {
        public DefaultDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<AFSTEMNING> AFSTEMNING { get; set; } = null!;
        public virtual DbSet<AUDIT_FK_ERROR> AUDIT_FK_ERROR { get; set; } = null!;
        public virtual DbSet<AUDIT_LOGERROR> AUDIT_LOGERROR { get; set; } = null!;
        public virtual DbSet<AUDIT_LOGINFO> AUDIT_LOGINFO { get; set; } = null!;
        public virtual DbSet<AUDIT_LOGINFO_TYPE> AUDIT_LOGINFO_TYPE { get; set; } = null!;
        public virtual DbSet<COLUMN_VALUE> COLUMN_VALUE { get; set; } = null!;
        public virtual DbSet<DEST_TABLE> DEST_TABLE { get; set; } = null!;
        public virtual DbSet<ENGINE_PROPERTY> ENGINE_PROPERTIES { get; set; } = null!;
        public virtual DbSet<EXECUTION> EXECUTIONS { get; set; } = null!;
        public virtual DbSet<HEALTH_REPORT> HEALTH_REPORT { get; set; } = null!;
        public virtual DbSet<LOGGING> LOGGING { get; set; } = null!;
        public virtual DbSet<LOGGING_CONTEXT> LOGGING_CONTEXT { get; set; } = null!;
        public virtual DbSet<MANAGER> MANAGERS { get; set; } = null!;
        public virtual DbSet<MANAGER_TRACKING> MANAGER_TRACKING { get; set; } = null!;
        public virtual DbSet<MIGRATION_FILE> MIGRATION_FILE { get; set; } = null!;
        public virtual DbSet<SEQUENCE_TRACKING> SEQUENCE_TRACKING { get; set; } = null!;
        public virtual DbSet<STATEMENT_COLUMN> STATEMENT_COLUMN { get; set; } = null!;
        public virtual DbSet<STATEMENT_JOIN> STATEMENT_JOINS { get; set; } = null!;
        public virtual DbSet<STATEMENT_TABLE> STATEMENT_TABLES { get; set; } = null!;
        public virtual DbSet<SYS_HOUSEKEEPING> SYS_HOUSEKEEPING { get; set; } = null!;
        public virtual DbSet<SYS_HOUSEKEEPING_UUID> SYS_HOUSEKEEPING_UUID { get; set; } = null!;
        public virtual DbSet<TABLE_LOG> TABLE_LOG { get; set; } = null!;
        public virtual DbSet<VOTE_COMBINATION> VOTE_COMBINATION { get; set; } = null!;
        public virtual DbSet<VOTE_RESULT> VOTE_RESULT { get; set; } = null!;
        public virtual DbSet<V_ENGINE_PROPERTY> V_ENGINE_PROPERTIES { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AFSTEMNING>(entity =>
            {
                entity.ToTable("AFSTEMNING");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.AFSTEMRESULTAT).HasMaxLength(4000);

                entity.Property(e => e.AFSTEMTDATO).HasColumnType("datetime");

                entity.Property(e => e.CONTEXT).HasMaxLength(400);

                entity.Property(e => e.CUSTOM_SQL).HasMaxLength(4000);

                entity.Property(e => e.DESCRIPTION).HasMaxLength(4000);

                entity.Property(e => e.DST_SQL).HasMaxLength(4000);

                entity.Property(e => e.END_TIME).HasColumnType("datetime");

                entity.Property(e => e.MANAGER).HasMaxLength(400);

                entity.Property(e => e.RUN_JOB).HasMaxLength(400);

                entity.Property(e => e.SRC_SQL).HasMaxLength(4000);

                entity.Property(e => e.START_TIME).HasColumnType("datetime");
            });

            modelBuilder.Entity<AUDIT_FK_ERROR>(entity =>
            {
                entity.ToTable("AUDIT_FK_ERRORS");

                entity.HasIndex(e => e.TABLENAME, "INDX_ERROR_ROWS_TNAME");

                entity.Property(e => e.FOREIGN_KEY_VIOLATED).HasMaxLength(400);

                entity.Property(e => e.ROWDATA).IsUnicode(false);

                entity.Property(e => e.TABLENAME).HasMaxLength(400);
            });

            modelBuilder.Entity<AUDIT_LOGERROR>(entity =>
            {
                entity.ToTable("AUDIT_LOGERROR");

                entity.HasIndex(e => new { e.ID, e.MGRNAME, e.CREATED }, "AUDIT_LOGERROR_ID_INDX");

                entity.HasIndex(e => e.MGRNAME, "AUDIT_LOGERROR_MGRNAME_INDX");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.MESSAGE).HasMaxLength(4000);

                entity.Property(e => e.MGRNAME).HasMaxLength(200);

                entity.Property(e => e.SOURCEROWS).HasMaxLength(4000);
            });

            modelBuilder.Entity<AUDIT_LOGINFO>(entity =>
            {
                entity.ToTable("AUDIT_LOGINFO");

                entity.HasIndex(e => e.BUSINESSID, "AUDIT_LOGINFO_BUSINESSID_INDX");

                entity.HasIndex(e => e.CPRNR, "AUDIT_LOGINFO_CPRNR_INDX");

                entity.HasIndex(e => new { e.ID, e.MGRNAME, e.CREATED }, "AUDIT_LOGINFO_ID_INDX");

                entity.HasIndex(e => e.MGRNAME, "AUDIT_LOGINFO_MGRNAME_INDX");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.CPRNR).HasMaxLength(400);

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.MESSAGE).HasMaxLength(1999);

                entity.Property(e => e.MGRNAME).HasMaxLength(200);
            });

            modelBuilder.Entity<AUDIT_LOGINFO_TYPE>(entity =>
            {
                entity.ToTable("AUDIT_LOGINFO_TYPES");

                entity.Property(e => e.ID).ValueGeneratedNever();

                entity.Property(e => e.DESCRIPTION).HasMaxLength(1999);

                entity.Property(e => e.ENUMNAME).HasMaxLength(500);

                entity.Property(e => e.TITLE).HasMaxLength(500);
            });

            modelBuilder.Entity<COLUMN_VALUE>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("COLUMN_VALUE");

                entity.Property(e => e.COLUMN_NAME).HasMaxLength(400);

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.VALUE).HasMaxLength(400);

                entity.Property(e => e.VOTE_COMBINATION_FK).HasMaxLength(400);
            });

            modelBuilder.Entity<DEST_TABLE>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("DEST_TABLE");

                entity.Property(e => e.ID_PREFIX).HasMaxLength(25);

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.TABLE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<ENGINE_PROPERTY>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ENGINE_PROPERTIES");

                entity.Property(e => e.KEY).HasMaxLength(400);

                entity.Property(e => e.MANAGER).HasMaxLength(200);

                entity.Property(e => e.TIMESTAMP).HasColumnType("datetime");

                entity.Property(e => e.VALUE).HasMaxLength(400);
            });

            modelBuilder.Entity<EXECUTION>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("EXECUTIONS");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.EXECUTION_UUID).HasMaxLength(400);
            });

            modelBuilder.Entity<HEALTH_REPORT>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("HEALTH_REPORT");

                entity.Property(e => e.LOG_TIME).HasColumnType("datetime");

                entity.Property(e => e.REPORT_TYPE).HasMaxLength(400);

                entity.Property(e => e.REPORT_VALUE_HUMAN)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.REPORT_VALUE_TYPE).HasMaxLength(400);
            });

            modelBuilder.Entity<LOGGING>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("LOGGING");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.LOG_LEVEL).HasMaxLength(80);
            });

            modelBuilder.Entity<LOGGING_CONTEXT>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("LOGGING_CONTEXT");
            });

            modelBuilder.Entity<MANAGER>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MANAGERS");

                entity.Property(e => e.MANAGER_NAME).HasMaxLength(500);
            });

            modelBuilder.Entity<MANAGER_TRACKING>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MANAGER_TRACKING");

                entity.Property(e => e.ENDTIME).HasColumnType("datetime");

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.STARTTIME).HasColumnType("datetime");

                entity.Property(e => e.STATUS).HasMaxLength(400);
            });

            modelBuilder.Entity<MIGRATION_FILE>(entity =>
            {
                entity.ToTable("MIGRATION_FILE");

                entity.Property(e => e.ID)
                    .HasMaxLength(400)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.FILE_NAME).HasMaxLength(400);

                entity.Property(e => e.JENKINS_JOB).HasMaxLength(400);

                entity.Property(e => e.RELATIVE_PATH).HasMaxLength(400);

                entity.Property(e => e.TITLE).HasMaxLength(400);
            });

            modelBuilder.Entity<SEQUENCE_TRACKING>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SEQUENCE_TRACKING");

                entity.Property(e => e.COLUMN_NAME).HasMaxLength(200);

                entity.Property(e => e.MGR).HasMaxLength(200);

                entity.Property(e => e.TABLE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<STATEMENT_COLUMN>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("STATEMENT_COLUMNS");

                entity.Property(e => e.COLUMN_NAME).HasMaxLength(40);

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.IDENTIFIER).HasMaxLength(400);

                entity.Property(e => e.IDENTIFIER_SHORT).HasMaxLength(60);

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.SCHEMA_NAME).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_FULL).HasMaxLength(60);

                entity.Property(e => e.TABLE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<STATEMENT_JOIN>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("STATEMENT_JOINS");

                entity.Property(e => e.COLUMN_NAME_1).HasMaxLength(40);

                entity.Property(e => e.COLUMN_NAME_2).HasMaxLength(40);

                entity.Property(e => e.COLUMN_NAME_3).HasMaxLength(40);

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.CRITERION).HasMaxLength(400);

                entity.Property(e => e.IDENTIFIER).HasMaxLength(400);

                entity.Property(e => e.IDENTIFIER_SHORT).HasMaxLength(60);

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.SCHEMA_NAME_1).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_2).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_3).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_FULL_1).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_FULL_2).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_FULL_3).HasMaxLength(60);

                entity.Property(e => e.TABLE_NAME_1).HasMaxLength(400);

                entity.Property(e => e.TABLE_NAME_2).HasMaxLength(400);

                entity.Property(e => e.TABLE_NAME_3).HasMaxLength(400);
            });

            modelBuilder.Entity<STATEMENT_TABLE>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("STATEMENT_TABLES");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.IDENTIFIER).HasMaxLength(400);

                entity.Property(e => e.IDENTIFIER_SHORT).HasMaxLength(60);

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.SCHEMA_NAME).HasMaxLength(60);

                entity.Property(e => e.SCHEMA_NAME_FULL).HasMaxLength(60);

                entity.Property(e => e.TABLE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<SYS_HOUSEKEEPING>(entity =>
            {
                entity.ToTable("SYS_HOUSEKEEPING");

                entity.HasIndex(e => new { e.SRC_SCHEMA, e.SRC_TBL, e.CLN_SCHEMA, e.CLN_TBL, e.MGR }, "SYS_HOUSEKEEPING_UK")
                    .IsUnique();

                entity.Property(e => e.CLN_PRIMARYKEY).HasMaxLength(400);

                entity.Property(e => e.CLN_SCHEMA).HasMaxLength(400);

                entity.Property(e => e.CLN_TBL).HasMaxLength(400);

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.SRC_PRIMARYKEY).HasMaxLength(400);

                entity.Property(e => e.SRC_SCHEMA).HasMaxLength(400);

                entity.Property(e => e.SRC_TBL).HasMaxLength(400);
            });

            modelBuilder.Entity<SYS_HOUSEKEEPING_UUID>(entity =>
            {
                entity.ToTable("SYS_HOUSEKEEPING_UUID");

                entity.HasIndex(e => new { e.SRC_SCHEMA, e.SRC_TBL, e.CLN_SCHEMA, e.CLN_TBL, e.MGR }, "SYS_HOUSEKEEPING_UUID_UK")
                    .IsUnique();

                entity.Property(e => e.CLN_PRIMARYKEY).HasMaxLength(400);

                entity.Property(e => e.CLN_SCHEMA).HasMaxLength(400);

                entity.Property(e => e.CLN_TBL).HasMaxLength(400);

                entity.Property(e => e.ID_PREFIX).HasMaxLength(400);

                entity.Property(e => e.MGR).HasMaxLength(400);

                entity.Property(e => e.SRC_PRIMARYKEY).HasMaxLength(400);

                entity.Property(e => e.SRC_SCHEMA).HasMaxLength(400);

                entity.Property(e => e.SRC_TBL).HasMaxLength(400);
            });

            modelBuilder.Entity<TABLE_LOG>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TABLE_LOG");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.SCHEMA_NAME).HasMaxLength(400);

                entity.Property(e => e.STATE).HasMaxLength(400);

                entity.Property(e => e.TABLE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<VOTE_COMBINATION>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("VOTE_COMBINATION");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.SCHEMA_NAME).HasMaxLength(400);

                entity.Property(e => e.TABLE_NAME).HasMaxLength(400);

                entity.Property(e => e.VOTE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<VOTE_RESULT>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("VOTE_RESULT");

                entity.Property(e => e.CREATED).HasColumnType("datetime");

                entity.Property(e => e.ID).HasMaxLength(400);

                entity.Property(e => e.PASSED).HasMaxLength(1);

                entity.Property(e => e.VOTE_MESSAGE).HasMaxLength(4000);

                entity.Property(e => e.VOTE_NAME).HasMaxLength(400);
            });

            modelBuilder.Entity<V_ENGINE_PROPERTY>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_ENGINE_PROPERTIES");

                entity.Property(e => e.KEY).HasMaxLength(400);

                entity.Property(e => e.MANAGER).HasMaxLength(200);

                entity.Property(e => e.TIMESTAMP).HasColumnType("datetime");

                entity.Property(e => e.VALUE).HasMaxLength(400);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
