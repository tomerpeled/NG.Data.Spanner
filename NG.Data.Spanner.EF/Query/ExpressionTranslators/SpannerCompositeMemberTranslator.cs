using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace NG.Data.Spanner.EF.Query.ExpressionTranslators
{
    public class SpannerCompositeMemberTranslator: RelationalCompositeMemberTranslator
    {
        public SpannerCompositeMemberTranslator()
        {
            var spannerTranslators = new List<IMemberTranslator>
            {
               // new SpannerStringLengthTranslator(),
            //    new MySqlDateTimeNowTranslator(),
              //  new MySqlDatePartTranslator()
            };

            AddTranslators(spannerTranslators);
        }
    }
}
