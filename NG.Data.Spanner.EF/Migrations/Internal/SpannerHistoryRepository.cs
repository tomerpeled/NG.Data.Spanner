using System;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Metadata;
using NG.Data.Spanner.EF.Storage.Internal;

namespace NG.Data.Spanner.EF.Migrations.Internal
{
    public class SpannerHistoryRepository: HistoryRepository
    {
        private readonly SpannerRelationalConnection _connection;

        public SpannerHistoryRepository(IDatabaseCreator databaseCreator, IRawSqlCommandBuilder rawSqlCommandBuilder, 
            SpannerRelationalConnection connection, IDbContextOptions options, IMigrationsModelDiffer modelDiffer,
            SpannerMigrationsSqlGenerationHelper migrationsSqlGenerator, SpannerAnnotationProvider annotations, ISqlGenerationHelper sqlGenerationHelper) : base(databaseCreator, rawSqlCommandBuilder, connection, options, modelDiffer, migrationsSqlGenerator, annotations, sqlGenerationHelper)
        {
            _connection = connection;
        }

        protected override bool InterpretExistsResult(object value) => true;

        public override string GetCreateIfNotExistsScript()
        {
            return GetCreateScript();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by Spanner");
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by Spanner");
        }

        public override string GetEndIfScript()
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by Spanner");
        }

        protected override string ExistsSql
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE ");

                builder
                    .Append("TABLE_SCHEMA='")
                    .Append(SqlGenerationHelper.EscapeLiteral(TableSchema ?? _connection.DbConnection.Database))
                    .Append("' AND TABLE_NAME='")
                    .Append(SqlGenerationHelper.EscapeLiteral(TableName))
                    .Append("';");

                return builder.ToString();
            }
        }
    }
}
