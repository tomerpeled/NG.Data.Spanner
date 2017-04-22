using System;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerMaxLengthMapping: RelationalTypeMapping
    {
        public SpannerMaxLengthMapping(string storeType, Type clrType) : base(storeType, clrType)
        {
        }

        public SpannerMaxLengthMapping(string storeType, Type clrType, DbType? dbType) : base(storeType, clrType, dbType)
        {
        }

        public SpannerMaxLengthMapping(string storeType, Type clrType, DbType? dbType, bool unicode, int? size, bool hasNonDefaultUnicode = false, bool hasNonDefaultSize = false) : base(storeType, clrType, dbType, unicode, size, hasNonDefaultUnicode, hasNonDefaultSize)
        {
        }
    }
}
