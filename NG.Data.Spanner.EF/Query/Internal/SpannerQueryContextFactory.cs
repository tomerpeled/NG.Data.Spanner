using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace NG.Data.Spanner.EF.Query.Internal
{
    public class SpannerQueryContextFactory: RelationalQueryContextFactory
    {
        private readonly IRelationalConnection _connection;

        public SpannerQueryContextFactory(ICurrentDbContext currentContext, IConcurrencyDetector concurrencyDetector, IRelationalConnection connection, IExecutionStrategyFactory executionStrategyFactory) : base(currentContext, concurrencyDetector, connection, executionStrategyFactory)
        {
            _connection = connection;
        }

        public override QueryContext Create()
            => new SpannerQueryContext(CreateQueryBuffer, _connection, StateManager, ConcurrencyDetector, ExecutionStrategyFactory);
    }
}
