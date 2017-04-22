using System;
using System.Data;
using System.Data.Common;

namespace NG.Data.Spanner
{
    public class SpannerParameter: DbParameter, IDataParameter
    {
        private string _name;
        public SpannerParameter()
        {
            
        }
        public override void ResetDbType()
        {
            throw new NotImplementedException();
        }

        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NormalizedParameterName = NormalizeParameterName(_name);
            }
        }
        internal string NormalizedParameterName { get; private set; }

        public override string SourceColumn { get; set; }
        public override object Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }
        public override int Size { get; set; }

        internal static string NormalizeParameterName(string name)
        {
            name = name.Trim();

            if ((name.StartsWith("@`", StringComparison.Ordinal) || name.StartsWith("?`", StringComparison.Ordinal)) && name.EndsWith("`", StringComparison.Ordinal))
                return name.Substring(2, name.Length - 3).Replace("``", "`");
            if ((name.StartsWith("@'", StringComparison.Ordinal) || name.StartsWith("?'", StringComparison.Ordinal)) && name.EndsWith("'", StringComparison.Ordinal))
                return name.Substring(2, name.Length - 3).Replace("''", "'");
            if ((name.StartsWith("@\"", StringComparison.Ordinal) || name.StartsWith("?\"", StringComparison.Ordinal)) && name.EndsWith("\"", StringComparison.Ordinal))
                return name.Substring(2, name.Length - 3).Replace("\"\"", "\"");

            return name.StartsWith("@", StringComparison.Ordinal) || name.StartsWith("?", StringComparison.Ordinal) ? name.Substring(1) : name;
        }
    }
}
