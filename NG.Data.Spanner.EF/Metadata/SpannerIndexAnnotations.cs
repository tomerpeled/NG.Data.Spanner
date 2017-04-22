using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NG.Data.Spanner.EF.Metadata.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Metadata
{
    public class SpannerIndexAnnotations: RelationalIndexAnnotations, ISpannerIndexAnnotations
    {
        public SpannerIndexAnnotations([NotNull] IIndex index)
            : base(index, RelationalFullAnnotationNames.Instance)
        {
        }

        protected SpannerIndexAnnotations([NotNull] RelationalAnnotations annotations)
            : base(annotations, RelationalFullAnnotationNames.Instance)
        {
        }

        public string Method
        {
            get { return (string)Annotations.GetAnnotation(SpannerFullAnnotationNames.Instance.IndexMethod, null); }
            set { Annotations.SetAnnotation(SpannerFullAnnotationNames.Instance.IndexMethod, null, value); }
        }
    }
}
