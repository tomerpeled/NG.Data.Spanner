using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace NG.Data.Spanner.EF.Infrastructure.Internal
{
    public class SpannerModelSource: RelationalModelSource
    {
        public SpannerModelSource(IDbSetFinder setFinder, 
            ICoreConventionSetBuilder coreConventionSetBuilder, 
            IModelCustomizer modelCustomizer, 
            IModelCacheKeyFactory modelCacheKeyFactory) : base(setFinder, coreConventionSetBuilder, modelCustomizer, modelCacheKeyFactory)
        {
        }
    }
}
