using Microsoft.EntityFrameworkCore.Storage;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerSqlGenerationHelper: RelationalSqlGenerationHelper
    {
        public override string DelimitIdentifier(string identifier)
        {
            var r = base.DelimitIdentifier(identifier);
            return r;
        }

    }
}
