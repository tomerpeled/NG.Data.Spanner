using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NG.Data.Spanner.EF.Metadata.Internal
{
    public class SpannerFullAnnotationNames: RelationalFullAnnotationNames
    {
        public SpannerFullAnnotationNames(string prefix) : base(prefix)
        {
            Serial = prefix + SpannerAnnotationNames.Serial;
            IndexMethod = prefix + SpannerAnnotationNames.IndexMethod;
        }

        public new static SpannerFullAnnotationNames Instance { get; } = new SpannerFullAnnotationNames(SpannerAnnotationNames.Prefix);

        public readonly string Serial;
        public readonly string IndexMethod;

    }
}
