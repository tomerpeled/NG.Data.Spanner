using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerScopedTypeMapper: IRelationalTypeMapper
    {
        private readonly SpannerTypeMapper _typeMapper;
        private readonly IDbContextOptions _options;


        public SpannerScopedTypeMapper(
            [NotNull] SpannerTypeMapper typeMapper,
            [CanBeNull] IDbContextOptions options)
        {
            Check.NotNull(typeMapper, nameof(typeMapper));

            _typeMapper = typeMapper;
            _options = options;
        }

        public RelationalTypeMapping FindMapping(IProperty property)
        {
            var mapping = _typeMapper.FindMapping(property);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public RelationalTypeMapping FindMapping(Type clrType)
        {
            var mapping = _typeMapper.FindMapping(clrType);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public RelationalTypeMapping FindMapping(string storeType)
        {
            var mapping = _typeMapper.FindMapping(storeType);
            return mapping == null ? null : MaybeConvertMapping(mapping);
        }

        public virtual void ValidateTypeName(string storeType) => _typeMapper.ValidateTypeName(storeType);

        public virtual IByteArrayRelationalTypeMapper ByteArrayMapper => _typeMapper.ByteArrayMapper;
        public virtual IStringRelationalTypeMapper StringMapper => _typeMapper.StringMapper;

        protected virtual RelationalTypeMapping MaybeConvertMapping(RelationalTypeMapping mapping)
        {
            return mapping;
        }
    }
}
