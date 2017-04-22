using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Migrations.Operations
{
    public class SpannerDropDatabaseOperation: MigrationOperation
    {
        public virtual string Name { get; [param: NotNull] set; }
    }
}
