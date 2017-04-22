using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NG.Data.Spanner.EF.Infrastructure.Internal;
using NG.Data.Spanner.EF.Metadata;
using NG.Data.Spanner.EF.Migrations;
using NG.Data.Spanner.EF.Migrations.Internal;
using NG.Data.Spanner.EF.Query.ExpressionTranslators;
using NG.Data.Spanner.EF.Query.Internal;
using NG.Data.Spanner.EF.Query.Sql.Internal;
using NG.Data.Spanner.EF.ValueGeneration.Internal;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerDatabaseProviderServices: RelationalDatabaseProviderServices
    {
        public SpannerDatabaseProviderServices(IServiceProvider services) : base(services)
        {
        }

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;
        public override IModelSource ModelSource => GetService<SpannerModelSource>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<SpannerValueGeneratorCache>();
        public override IRelationalAnnotationProvider AnnotationProvider => GetService<SpannerAnnotationProvider>();
        public override IQuerySqlGeneratorFactory QuerySqlGeneratorFactory => GetService<SpannerQuerySqlGenerationHelperFactory>();

        //public override IRelationalTypeMapper TypeMapper => GetService<SpannerTypeMapper>();
        public override IRelationalTypeMapper TypeMapper => GetService<SpannerScopedTypeMapper>();

        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<SpannerCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<SpannerCompositeMemberTranslator>();
        public override IHistoryRepository HistoryRepository => GetService<SpannerHistoryRepository>();
        public override IRelationalConnection RelationalConnection => GetService<SpannerRelationalConnection>();
        public override ISqlGenerationHelper SqlGenerationHelper => GetService<SpannerSqlGenerationHelper>();
        public override IMigrationsAnnotationProvider MigrationsAnnotationProvider => GetService<SpannerMigrationsAnnotationProvider>();

        public override IUpdateSqlGenerator UpdateSqlGenerator { get; }
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory { get; }


        public override IValueGeneratorSelector ValueGeneratorSelector => GetService<SpannerValueGeneratorSelector>();

        public override IDatabaseCreator Creator => GetService<SpannerDatabaseCreator>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<SpannerDatabaseCreator>();
        public override IMigrationsSqlGenerator MigrationsSqlGenerator => GetService<SpannerMigrationsSqlGenerationHelper>();

        public override IDatabase Database => GetService<SpannerDatabase>();

        public override IQueryCompilationContextFactory QueryCompilationContextFactory => GetService<SpannerQueryCompilationContextFactory>();

    }
}
