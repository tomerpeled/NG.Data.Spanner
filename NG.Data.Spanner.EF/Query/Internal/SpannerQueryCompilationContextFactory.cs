using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;

namespace NG.Data.Spanner.EF.Query.Internal
{
    public class SpannerQueryCompilationContextFactory: RelationalQueryCompilationContextFactory 
    {
        public SpannerQueryCompilationContextFactory(IModel model, ISensitiveDataLogger<RelationalQueryCompilationContextFactory> logger, IEntityQueryModelVisitorFactory entityQueryModelVisitorFactory, IRequiresMaterializationExpressionVisitorFactory requiresMaterializationExpressionVisitorFactory, MethodInfoBasedNodeTypeRegistry methodInfoBasedNodeTypeRegistry, ICurrentDbContext currentContext) : base(model, logger, entityQueryModelVisitorFactory, requiresMaterializationExpressionVisitorFactory, methodInfoBasedNodeTypeRegistry, currentContext)
        {
        }

        public override QueryCompilationContext Create(bool async)
            => async
                ? new SpannerQueryCompilationContext(
                    Model,
                    (ISensitiveDataLogger)Logger,
                    EntityQueryModelVisitorFactory,
                    RequiresMaterializationExpressionVisitorFactory,
                    new AsyncLinqOperatorProvider(),
                    new SpannerAsyncQueryMethodProvider(),
                    ContextType,
                    TrackQueryResults)
                : new SpannerQueryCompilationContext(
                    Model,
                    (ISensitiveDataLogger)Logger,
                    EntityQueryModelVisitorFactory,
                    RequiresMaterializationExpressionVisitorFactory,
                    new LinqOperatorProvider(),
                    new SpannerQueryMethodProvider(),
                    ContextType,
                    TrackQueryResults);
    }
}
