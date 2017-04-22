using System;

namespace NG.Data.Spanner.Types
{
    internal interface ISpannerValue
    {
        bool IsNull { get; }
        SpannerDbType SpannerType { get; }
        object Value { get; /*set;*/ }
        Type SystemType { get; }
        string SpannerTypeName { get; }
    }
}
