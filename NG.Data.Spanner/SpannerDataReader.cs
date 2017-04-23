using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Spanner.V1;
using Google.Protobuf.WellKnownTypes;
using Type = System.Type;
using TypeCode = Google.Cloud.Spanner.V1.TypeCode;

namespace NG.Data.Spanner
{
    public class SpannerDataReader: DbDataReader, IDataReader
    {
        // The DataReader should always be open when returned to the user.
        private bool _isOpen = true;
        private readonly ResultSet _resultSet;
        private int _currentRowIndex;

        public SpannerDataReader(ResultSet resultSet)
        {
            _resultSet = resultSet;
            _currentRowIndex = -1;
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            return DBNull.Value == GetValue(ordinal);
        }

        public override int FieldCount => _resultSet.Metadata.RowType.Fields.Count;

        public override object this[int ordinal]
        {
            get { throw new NotImplementedException(); }
        }

        public override object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public override bool HasRows { get; }
        public override bool IsClosed { get; }
        public override int RecordsAffected { get; }

        public override bool NextResult() => NextResultAsync(IOBehavior.Synchronous, CancellationToken.None).GetAwaiter().GetResult();

        internal Task<bool> NextResultAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
        {
           // VerifyNotDisposed();
            throw new NotImplementedException();
        }

        private void VerifyNotDisposed()
        {
            if (Command == null)
                throw new ObjectDisposedException(GetType().Name);
        }

        internal SpannerCommand Command { get; private set; }


        public override Task<bool> ReadAsync(CancellationToken cancellationToken) => Task.FromResult(Read());
        internal Task<bool> ReadAsync(IOBehavior ioBehavior, CancellationToken cancellationToken) => Task.FromResult(Read());

        public override bool Read()
        {
            
            _currentRowIndex++;
            if (_currentRowIndex >= _resultSet.Rows.Count)
            {
                return false;
            }
            return true;
        }

        public override int Depth { get; }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override bool GetBoolean(int ordinal)
        {
            return Convert.ToBoolean(GetValue(ordinal));
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            string s = GetString(ordinal);
            return s[0];
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            return Convert.ToInt32(GetValue(ordinal));
        }

        public override long GetInt64(int ordinal)
        {
            return Convert.ToInt64(GetValue(ordinal));
        }

        public override DateTime GetDateTime(int ordinal)
        {
            var dateTimeString = GetValue(ordinal) as string;
            return string.IsNullOrEmpty(dateTimeString) ? Convert.ToDateTime(dateTimeString) :
                DateTimeOffset.Parse(dateTimeString).UtcDateTime;
        }

        public override string GetString(int ordinal)
        {
            return GetValue(ordinal).ToString();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            return Convert.ToDouble(GetValue(ordinal));
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            return GetColumnType(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            if (!_isOpen)
                throw new Exception("No current query in data reader");

            if (ordinal >= FieldCount)
                throw new IndexOutOfRangeException();

            var columnValue = GetColumnValue(ordinal);
            var val = GetColumnFromValue(columnValue, ordinal);
            if (val == null)
                return DBNull.Value;

            return val;

        }

        private object GetColumnFromValue(Value val, int columnIndex)
        {
            var field = _resultSet.Metadata.RowType.Fields[columnIndex];
            if (field.Type.Code == TypeCode.String)
            {
                return string.IsNullOrEmpty(val.StringValue) ? null : val.StringValue;
            }
            else if (field.Type.Code == TypeCode.Int64)
            {
                return string.IsNullOrEmpty(val.StringValue) ? null : val.StringValue;
            }
            else if (field.Type.Code == TypeCode.Bool)
            {
	            return val.ToString() == "null" ? null : (bool?)val.BoolValue;
            }
			else if (field.Type.Code == TypeCode.Float64)
            {
                return val.NumberValue;
            }
            else if (field.Type.Code == TypeCode.Timestamp)
            {
                return string.IsNullOrEmpty(val.StringValue) ? null : val.StringValue;
            }
            return null;
        }
        private Type GetColumnType(int columnIndex)
        {
            var field = _resultSet.Metadata.RowType.Fields[columnIndex];
            if (field.Type.Code == TypeCode.String)
            {
                return typeof(string);
            }
            else if (field.Type.Code == TypeCode.Int64)
            {
                return typeof(long);
            }
            else if (field.Type.Code == TypeCode.Bool)
            {
                return typeof(bool);
            }
            else if (field.Type.Code == TypeCode.Float64)
            {
                return typeof(double);
            }
            else if (field.Type.Code == TypeCode.Timestamp)
            {
                return typeof(DateTime);
            }
            return null;
        }

        private Value GetColumnValue(int index)
        {
            if (index < 0 || index >= FieldCount)
                throw new ArgumentException("Invalid column ordinal");

            var values = _resultSet.Rows[_currentRowIndex];
            return values.Values[index];
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            if (!_isOpen) return;

            _isOpen = false;

            base.Close();
        }
    }
}
