using System.Globalization;

namespace NG.Data.Spanner
{
    public static class Utils
    {
        public static string FormatInvariant(this string format, params object[] args) => string.Format(CultureInfo.InvariantCulture, format, args);
    }
}
