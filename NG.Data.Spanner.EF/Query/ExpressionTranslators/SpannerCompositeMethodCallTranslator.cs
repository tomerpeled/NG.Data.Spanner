using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.Extensions.Logging;

namespace NG.Data.Spanner.EF.Query.ExpressionTranslators
{
    public class SpannerCompositeMethodCallTranslator: RelationalCompositeMethodCallTranslator
    {
        private static readonly IMethodCallTranslator[] _methodCallTranslators =
        {
            new SpannerStringSubstringTranslator(),
            new SpannerContainsOptimizedTranslator()
         //   new MySqlMathAbsTranslator(),
         //   new MySqlMathCeilingTranslator(),
         //   new MySqlMathFloorTranslator(),
         //   new MySqlMathPowerTranslator(),
        //    new MySqlMathRoundTranslator(),
        //    new MySqlMathTruncateTranslator(),
     //       new MySqlStringReplaceTranslator(),
      //      new MySqlStringToLowerTranslator(),
      //      new MySqlStringToUpperTranslator(),
     //       new MySqlRegexIsMatchTranslator(),
     //       new MySqlContainsOptimizedTranslator(),
    //        new MySqlStartsWithOptimizedTranslator(),
    //        new MySqlEndsWithOptimizedTranslator(),
    //        new MySqlDateAddTranslator()
        };

        public SpannerCompositeMethodCallTranslator(ILogger<SpannerCompositeMethodCallTranslator> logger) : base(logger)
        {
            AddTranslators(_methodCallTranslators);
        }
    }
}
