using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace NG.Data.Spanner.EF.Utils
{
    public static class DBUtilsHelper
    {
        public static string ToSpannerFormat(this DateTime dateTime)
        {
            return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
        }

        private static readonly Regex _singularRegex
            = new Regex(pattern: "(ey|.)(?<!s)$", options: RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static string Pluralize([NotNull] string value)
          => _singularRegex.Replace(
              Check.NotNull(value, nameof(value)),
              match => string.Equals(a: "y", b: match.Value, comparisonType: StringComparison.OrdinalIgnoreCase)
                  ? "ies"
                  : $"{match.Value}s");

    }
}
