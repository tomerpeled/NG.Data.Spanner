using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NG.Data.Spanner.EF.Metadata.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Metadata
{
    public class SpannerModelAnnotations: RelationalModelAnnotations, ISpannerModelAnnotations
    {
        public SpannerModelAnnotations([NotNull] IModel model): base(model, SpannerFullAnnotationNames.Instance)
        {
        }
        public SpannerModelAnnotations(IModel model, RelationalFullAnnotationNames providerFullAnnotationNames) : base(model, providerFullAnnotationNames)
        {
        }

        public SpannerModelAnnotations(RelationalAnnotations annotations, RelationalFullAnnotationNames providerFullAnnotationNames) : base(annotations, providerFullAnnotationNames)
        {
        }
    }
}
