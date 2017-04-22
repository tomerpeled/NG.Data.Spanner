using System;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace NG.Data.Spanner.EF.Query.Internal
{
    public class SpannerQueryContext: RelationalQueryContext
    {
        public bool HasInclude = false;

        public SpannerQueryContext(Func<IQueryBuffer> queryBufferFactory, IRelationalConnection connection, LazyRef<IStateManager> stateManager, IConcurrencyDetector concurrencyDetector, IExecutionStrategyFactory executionStrategyFactory) : base(queryBufferFactory, connection, stateManager, concurrencyDetector, executionStrategyFactory)
        {
        }
    }
}
