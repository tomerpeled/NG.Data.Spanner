using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NG.Data.Spanner.EF.Extensions;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.ValueGeneration.Internal
{
    public class SpannerValueGeneratorSelector: RelationalValueGeneratorSelector
    {
        public SpannerValueGeneratorSelector(IValueGeneratorCache cache, IRelationalAnnotationProvider relationalExtensions) : base(cache, relationalExtensions)
        {
        }

        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Check.NotNull(property, nameof(property));
            Check.NotNull(entityType, nameof(entityType));

            var ret = property.ClrType.UnwrapNullableType() == typeof(Guid)
                ? property.ValueGenerated == ValueGenerated.Never
                  || property.Spanner().DefaultValueSql != null
                    ? (ValueGenerator)new TemporaryGuidValueGenerator()
                    : new SequentialGuidValueGenerator()
                : base.Create(property, entityType);
            return ret;
        }
    }
}
