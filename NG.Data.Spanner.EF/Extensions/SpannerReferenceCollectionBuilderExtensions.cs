using System;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NG.Data.Spanner.EF.Infrastructure.Internal;
using NG.Data.Spanner.EF.Internal;
using NG.Data.Spanner.EF.Metadata;
using NG.Data.Spanner.EF.Migrations;
using NG.Data.Spanner.EF.Migrations.Internal;
using NG.Data.Spanner.EF.Query.ExpressionTranslators;
using NG.Data.Spanner.EF.Query.Internal;
using NG.Data.Spanner.EF.Query.Sql.Internal;
using NG.Data.Spanner.EF.Storage;
using NG.Data.Spanner.EF.Storage.Internal;
using NG.Data.Spanner.EF.Utils;
using NG.Data.Spanner.EF.ValueGeneration.Internal;

namespace NG.Data.Spanner.EF.Extensions
{
    public static class SpannerReferenceCollectionBuilderExtensions
    {
        public static IServiceCollection AddEntityFrameworkSpanner([NotNull] this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            //GrpcEnvironment.SetCompletionQueueCount(1);

            services.AddRelational()
                .AddScoped<IRelationalCommandBuilderFactory, SpannerCommandBuilderFactory>()
                .AddScoped<RelationalQueryContextFactory, SpannerQueryContextFactory>()
                //.AddScoped<IRelationalTypeMapper, SpannerTypeMapper>()
                .AddScoped<IDatabase, SpannerDatabase>();


            services.TryAddEnumerable(ServiceDescriptor
                .Singleton<IDatabaseProvider, DatabaseProvider<SpannerDatabaseProviderServices, SpannerOptionsExtension>>());

            services.TryAdd(new ServiceCollection()
                .AddSingleton<SpannerValueGeneratorCache>()
                .AddSingleton<SpannerSqlGenerationHelper>()
                .AddSingleton<SpannerTypeMapper>()
                .AddScoped<SpannerScopedTypeMapper>()
                .AddSingleton<SpannerModelSource>()
            //    .AddSingleton<SpannerDatabase>()
                .AddSingleton<SpannerAnnotationProvider>()
                .AddSingleton<SpannerMigrationsAnnotationProvider>()
              //  .AddScoped<MySqlBatchExecutor>()
                .AddScoped(p => GetProviderServices(p).BatchExecutor)
              //  .AddScoped<MySqlConventionSetBuilder>()
                .AddScoped<TableNameFromDbSetConvention>()
               // .AddScoped<IMySqlUpdateSqlGenerator, MySqlUpdateSqlGenerator>()
              //  .AddScoped<MySqlModificationCommandBatchFactory>()
                .AddScoped<SpannerValueGeneratorSelector>()
                .AddScoped<SpannerDatabaseProviderServices>()
                .AddScoped<SpannerRelationalConnection>()
                .AddScoped<SpannerDatabaseCreator>()
                .AddScoped<SpannerHistoryRepository>()
                .AddScoped<SpannerMigrationsSqlGenerationHelper>()
              //  .AddScoped<MySqlModificationCommandBatchFactory>()
                .AddQuery());

            services
               // .AddScoped<IChangeDetector, MySqlChangeDetector>()
                .AddScoped<IPropertyListener, IChangeDetector>(p => p.GetService<IChangeDetector>())
                .AddScoped<SpannerMigrationsModelDiffer>()
                .AddScoped<IMigrationsModelDiffer, SpannerMigrationsModelDiffer>();

            return services;
        }

        private static IServiceCollection AddQuery(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<SpannerQueryCompilationContextFactory>()
                .AddScoped<SpannerCompositeMemberTranslator>()
                .AddScoped<SpannerCompositeMethodCallTranslator>()
                .AddScoped<SpannerQuerySqlGenerationHelperFactory>();
        }

        private static IRelationalDatabaseProviderServices GetProviderServices(IServiceProvider serviceProvider)
        {
            var providerServices = serviceProvider.GetRequiredService<IDbContextServices>().DatabaseProviderServices
                as IRelationalDatabaseProviderServices;

            if (providerServices == null)
            {
                throw new InvalidOperationException(RelationalStrings.RelationalNotInUse);
            }

            return providerServices;
        }
    }
}
