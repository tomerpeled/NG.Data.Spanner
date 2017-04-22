using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NG.Data.Spanner.EF.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Infrastructure
{
    public class SpannerDbContextOptionsBuilder: RelationalDbContextOptionsBuilder<SpannerDbContextOptionsBuilder, SpannerOptionsExtension>
    {
        public SpannerDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder) : base(optionsBuilder)
        {
        }

        protected override SpannerOptionsExtension CloneExtension()
            => new SpannerOptionsExtension(OptionsBuilder.Options.GetExtension<SpannerOptionsExtension>());
    }
}
