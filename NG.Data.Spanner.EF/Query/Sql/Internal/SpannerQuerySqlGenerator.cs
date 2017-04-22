using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Query.Sql.Internal
{
    public class SpannerQuerySqlGenerator: DefaultQuerySqlGenerator
    {
        public SpannerQuerySqlGenerator(IRelationalCommandBuilderFactory relationalCommandBuilderFactory, ISqlGenerationHelper sqlGenerationHelper, IParameterNameGeneratorFactory parameterNameGeneratorFactory, IRelationalTypeMapper relationalTypeMapper, SelectExpression selectExpression):
            base(relationalCommandBuilderFactory, sqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper, selectExpression)
        {
        }

        protected override void GenerateTop(SelectExpression selectExpression)
        {
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("LIMIT ");
                Visit(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                if (selectExpression.Limit == null)
                {
                    // if we want to use Skip() without Take() we have to define the upper limit of LIMIT 
                    Sql.AppendLine().Append("LIMIT ").Append(18446744073709551610);
                }
                Sql.Append(' ');
                Sql.Append("OFFSET ");
                Visit(selectExpression.Offset);
            }
        }
    }
}
