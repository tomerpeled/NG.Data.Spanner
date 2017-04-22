using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace NG.Data.Spanner.EF.Query.Internal
{
    public class SpannerQueryCompilationContext: RelationalQueryCompilationContext
    {
        public SpannerQueryCompilationContext(IModel model, ISensitiveDataLogger logger, IEntityQueryModelVisitorFactory entityQueryModelVisitorFactory, IRequiresMaterializationExpressionVisitorFactory requiresMaterializationExpressionVisitorFactory, ILinqOperatorProvider linqOperatorProvider, IQueryMethodProvider queryMethodProvider, Type contextType, bool trackQueryResults) : base(model, logger, entityQueryModelVisitorFactory, requiresMaterializationExpressionVisitorFactory, linqOperatorProvider, queryMethodProvider, contextType, trackQueryResults)
        {
        }

        public override string CreateUniqueTableAlias(string currentAlias)
        {
            currentAlias = currentAlias.Replace(".", "_");
            return base.CreateUniqueTableAlias(currentAlias);
        }
    }
}
