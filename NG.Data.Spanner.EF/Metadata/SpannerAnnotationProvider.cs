using Microsoft.EntityFrameworkCore.Metadata;
using NG.Data.Spanner.EF.Extensions;

namespace NG.Data.Spanner.EF.Metadata
{
    public class SpannerAnnotationProvider: IRelationalAnnotationProvider
    {
        public SpannerAnnotationProvider()
        {
            
        }

        public virtual IRelationalEntityTypeAnnotations For(IEntityType entityType) => entityType.Spanner();

        public virtual IRelationalForeignKeyAnnotations For(IForeignKey foreignKey) => foreignKey.Spanner();

        public IRelationalIndexAnnotations For(IIndex index) => index.Spanner();

        public virtual IRelationalKeyAnnotations For(IKey key) => key.Spanner();
        public virtual IRelationalModelAnnotations For(IModel model) => model.Spanner();
        public virtual IRelationalPropertyAnnotations For(IProperty property) => property.Spanner();
        
    }
}
