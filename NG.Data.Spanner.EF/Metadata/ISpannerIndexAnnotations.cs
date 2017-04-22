using Microsoft.EntityFrameworkCore.Metadata;

namespace NG.Data.Spanner.EF.Metadata
{
    public interface ISpannerIndexAnnotations: IRelationalIndexAnnotations
    {
        string Method { get; }
    }
}
