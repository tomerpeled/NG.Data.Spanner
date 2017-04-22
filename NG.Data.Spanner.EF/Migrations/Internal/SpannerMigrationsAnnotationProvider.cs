using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using NG.Data.Spanner.EF.Extensions;
using NG.Data.Spanner.EF.Metadata.Internal;

namespace NG.Data.Spanner.EF.Migrations.Internal
{
    public class SpannerMigrationsAnnotationProvider: MigrationsAnnotationProvider
    {
        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            // The migrations SQL generator gets the property's DefaultValue and DefaultValueSql.
            // However, there's no way there to detect properties that have ValueGenerated.OnAdd
            // *without* defining a default value; these should translate to SERIAL columns.
            // So we add a custom annotation here to pass the information.
            if (property.ValueGenerated == ValueGenerated.OnAdd)
                yield return new Annotation(SpannerAnnotationNames.Prefix + SpannerAnnotationNames.ValueGeneratedOnAdd, true);
            else if (property.ValueGenerated == ValueGenerated.OnAddOrUpdate)
                yield return new Annotation(SpannerAnnotationNames.Prefix + SpannerAnnotationNames.ValueGeneratedOnAddOrUpdate, true);
        }

        public override IEnumerable<IAnnotation> For(IIndex index)
        {
            if (index.Spanner().Method != null)
            {
                yield return new Annotation(
                     SpannerAnnotationNames.Prefix + SpannerAnnotationNames.IndexMethod,
                     index.Spanner().Method);
            }
        }
    }
}
