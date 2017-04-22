using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NG.Data.Spanner.EF.Extensions;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Internal
{
    public sealed class SpannerOptionsExtension: RelationalOptionsExtension
    {
        public SpannerOptionsExtension()
        {
            MaxBatchSize = 1;
        }

        public SpannerOptionsExtension([NotNull] RelationalOptionsExtension copyFrom) : base(copyFrom)
        {
        }

        public override void ApplyServices(IServiceCollection services)
                  => Check.NotNull(services, nameof(services)).AddEntityFrameworkSpanner();
        
    }
}
