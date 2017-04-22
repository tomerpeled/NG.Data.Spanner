using Microsoft.EntityFrameworkCore.Metadata;
using NG.Data.Spanner.EF.Metadata;
using NG.Data.Spanner.EF.Metadata.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Extensions
{
    public static class SpannerMetadataExtensions
    {
        public static IRelationalEntityTypeAnnotations Spanner([NotNull] this IEntityType entityType)
           => new RelationalEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)), SpannerFullAnnotationNames.Instance);

        public static RelationalEntityTypeAnnotations Spanner([NotNull] this IMutableEntityType entityType)
           => (RelationalEntityTypeAnnotations)Spanner((IEntityType)entityType);

        public static IRelationalForeignKeyAnnotations Spanner([NotNull] this IForeignKey foreignKey)
            => new RelationalForeignKeyAnnotations(Check.NotNull(foreignKey, nameof(foreignKey)), SpannerFullAnnotationNames.Instance);

        public static RelationalForeignKeyAnnotations Spanner([NotNull] this IMutableForeignKey foreignKey)
            => (RelationalForeignKeyAnnotations)Spanner((IForeignKey)foreignKey);

        public static ISpannerIndexAnnotations Spanner([NotNull] this IIndex index)
            => new SpannerIndexAnnotations(Check.NotNull(index, nameof(index)));

        public static RelationalIndexAnnotations Spanner([NotNull] this IMutableIndex index)
            => (SpannerIndexAnnotations)Spanner((IIndex)index);

        public static IRelationalKeyAnnotations Spanner([NotNull] this IKey key)
            => new RelationalKeyAnnotations(Check.NotNull(key, nameof(key)), SpannerFullAnnotationNames.Instance);

        public static RelationalKeyAnnotations Spanner([NotNull] this IMutableKey key)
            => (RelationalKeyAnnotations)Spanner((IKey)key);

        public static ISpannerModelAnnotations Spanner([NotNull] this IModel model)
            => new SpannerModelAnnotations(Check.NotNull(model, nameof(model)));

        public static SpannerModelAnnotations Spanner([NotNull] this IMutableModel model)
            => (SpannerModelAnnotations)Spanner((IModel)model);

        public static IRelationalPropertyAnnotations Spanner([NotNull] this IProperty property)
            => new RelationalPropertyAnnotations(Check.NotNull(property, nameof(property)), SpannerFullAnnotationNames.Instance);

        public static RelationalPropertyAnnotations Spanner([NotNull] this IMutableProperty property)
            => (RelationalPropertyAnnotations)Spanner((IProperty)property);
    }
}
