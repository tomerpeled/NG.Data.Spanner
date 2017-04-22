using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Extensions;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerTypeMapper: RelationalTypeMapper
    {
        private static readonly Regex TypeRe = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?\s*(unsigned)?", RegexOptions.IgnoreCase);

        // boolean
        private readonly RelationalTypeMapping _bit = new RelationalTypeMapping("BOOL", typeof(bool), DbType.Boolean);

        // integers
        private readonly RelationalTypeMapping _bigint = new RelationalTypeMapping("INT64", typeof(long), DbType.Int64);

        // decimals
        private readonly RelationalTypeMapping _float = new RelationalTypeMapping("FLOAT64", typeof(float));

        // bytes
        private readonly RelationalTypeMapping _bytes = new RelationalTypeMapping("BYTES", typeof(byte[]), DbType.Binary);

        // string
        private readonly RelationalTypeMapping _varcharmax = new RelationalTypeMapping("STRING", typeof(string), DbType.AnsiString); // TODO: should use something like MySqlMaxLengthMapping

        // DateTime
        private readonly RelationalTypeMapping _dateTime = new RelationalTypeMapping("DATE", typeof(DateTime), DbType.DateTime);

        // json
        //private readonly RelationalTypeMapping _json = new RelationalTypeMapping("json", typeof(JsonObject<>), DbType.String);

        // row version
        private readonly RelationalTypeMapping _rowversion = new RelationalTypeMapping("TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP", typeof(byte[]), DbType.Binary);

        // guid
        private readonly RelationalTypeMapping _uniqueidentifier = new RelationalTypeMapping("char(36)", typeof(Guid));

        readonly Dictionary<string, RelationalTypeMapping> _simpleNameMappings;
        readonly Dictionary<Type, RelationalTypeMapping> _simpleMappings;

        public SpannerTypeMapper()
        {
            _simpleNameMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    // boolean
                    { "bit", _bit },

                    // integers
                    { "_bigint", _bigint },

                    // decimals
                    { "float", _float },

                    // binary
                    { "binary", _bytes },

                    // string
                    { "varchar", _varcharmax },

                    // DateTime
                    { "datetime", _dateTime },

                    // json
                    //{ "json", _json },
                };

            _simpleMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
	                // boolean
	                { typeof(bool), _bit },

	                // integers
                    { typeof(int), _bigint },

	                // decimals
                    { typeof(float), _float },

	                // byte / char
                    { typeof(byte), _bytes },

	                // DateTime
	                { typeof(DateTime), _dateTime },

	                // json
	                //{ typeof(JsonObject<>), _json },

	                // guid
	                { typeof(Guid), _uniqueidentifier }
                };

            //ByteArrayMapper
            //    = new ByteArrayRelationalTypeMapper(
            //        8000,
            //        _varbinarymax,
            //        _varbinary767,
            //        _varbinary767,
            //        _rowversion, size => new MySqlMaxLengthMapping(
            //            "varbinary(" + size + ")",
            //            typeof(byte[]),
            //            DbType.Binary,
            //            unicode: false,
            //            size: size,
            //            hasNonDefaultUnicode: false,
            //            hasNonDefaultSize: true));

            StringMapper
                = new StringRelationalTypeMapper(
                    8000,
                    _varcharmax,
                    _varcharmax,
                    _varcharmax,
                    size => new SpannerMaxLengthMapping(
                        "varchar(" + size + ")",
                        typeof(string),
                        dbType: DbType.AnsiString,
                        unicode: false,
                        size: size,
                        hasNonDefaultUnicode: true,
                        hasNonDefaultSize: true),
                    8000,
                    _varcharmax,
                    _varcharmax,
                    _varcharmax,
                    size => new SpannerMaxLengthMapping(
                        "varchar(" + size + ")",
                        typeof(string),
                        dbType: null,
                        unicode: true,
                        size: size,
                        hasNonDefaultUnicode: false,
                        hasNonDefaultSize: true));
        }

        protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
        {
            return _simpleNameMappings;
        }

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
        {
            return _simpleMappings;
        }

        public override IByteArrayRelationalTypeMapper ByteArrayMapper { get; }

        public override IStringRelationalTypeMapper StringMapper { get; }

        protected override string GetColumnType(IProperty property) => property.Spanner().ColumnType;

        protected override RelationalTypeMapping CreateMappingFromStoreType([NotNull] string storeType)
        {
            Check.NotNull(storeType, nameof(storeType));
            storeType = storeType.Trim().ToLower();

            var matchType = storeType;
            var matchLen = 0;
            var matchUnsigned = false;
            var match = TypeRe.Match(storeType);
            if (match.Success)
            {
                matchType = match.Groups[1].Value.ToLower();
                if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
                    int.TryParse(match.Groups[2].Value, out matchLen);
                if (!string.IsNullOrWhiteSpace(match.Groups[3].Value))
                    matchUnsigned = true;
            }

            var exactMatch = matchType + (matchLen > 0 ? $"({matchLen})" : "") + (matchUnsigned ? " unsigned" : "");
            RelationalTypeMapping mapping;
            if (GetStoreTypeMappings().TryGetValue(exactMatch, out mapping))
                return mapping;

            var noLengthMatch = matchType + (matchUnsigned ? " unsigned" : "");
            if (!GetStoreTypeMappings().TryGetValue(noLengthMatch, out mapping))
                return null;

            if (mapping.ClrType == typeof(string) || mapping.ClrType == typeof(byte[]))
            {
                return mapping.CreateCopy(exactMatch, matchLen);
            }
            return mapping.CreateCopy(exactMatch, mapping.Size);
        }

        public override RelationalTypeMapping FindMapping(Type clrType)
        {
            Check.NotNull(clrType, nameof(clrType));

            //if (clrType.Name == typeof(JsonObject<>).Name)
            //    return _json;

            return clrType == typeof(string)
                ? _varcharmax
                : (clrType == typeof(byte[])
                    ? _bytes
                    : base.FindMapping(clrType));
        }

        protected override RelationalTypeMapping FindCustomMapping([NotNull] IProperty property)
        {
            Check.NotNull(property, nameof(property));

            var clrType = property.ClrType.UnwrapEnumType();

            return clrType == typeof(string)
                ? StringMapper.FindMapping(true, property.IsKey() || property.IsIndex(), property.GetMaxLength())
                : clrType == typeof(byte[])
                    ? ByteArrayMapper.FindMapping(false, property.IsKey() || property.IsIndex(), property.GetMaxLength())
                    : null;
        }

    }
}
