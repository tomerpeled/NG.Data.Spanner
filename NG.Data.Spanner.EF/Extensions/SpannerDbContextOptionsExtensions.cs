using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NG.Data.Spanner.EF.Infrastructure;
using NG.Data.Spanner.EF.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Extensions
{
    public static class SpannerDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseSpanner(
            [NotNull] this DbContextOptionsBuilder optionsBuilder,
            [NotNull] string connectionString,
            [CanBeNull] Action<SpannerDbContextOptionsBuilder> spannerOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotEmpty(connectionString, nameof(connectionString));

            var extension = GetOrCreateExtension(optionsBuilder);
            extension.ConnectionString = connectionString;
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            spannerOptionsAction?.Invoke(new SpannerDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        private static SpannerOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        {
            var existing = optionsBuilder.Options.FindExtension<SpannerOptionsExtension>();
            return existing != null
                ? new SpannerOptionsExtension(existing)
                : new SpannerOptionsExtension();
        }
    }
}
