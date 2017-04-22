using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NG.Data.Spanner
{
    public class SpannerDataParameterCollection: DbParameterCollection
    {
        private readonly List<SpannerParameter> _parameters;
        private Dictionary<string, int> _nameToIndex;

        public SpannerDataParameterCollection()
        {
            _parameters = new List<SpannerParameter>();
            _nameToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public SpannerParameter Add(string parameterName, DbType dbType)
        {
            var parameter = new SpannerParameter
            {
                ParameterName = parameterName,
                DbType = dbType
            };
            _parameters.Add(parameter);
            return parameter;
        }
        public override int Add(object value)
        {
            _parameters.Add((SpannerParameter)value);
            return _parameters.Count - 1;
        }

        public override bool Contains(object value)
        {
            var parameter = value as SpannerParameter;
            if (null == parameter)
                throw new ArgumentException("Argument must be of type DbParameter", "value");
            return _parameters.Contains(parameter);
        }

        public override void Clear()
        {
            _parameters.Clear();
        }

        public override int IndexOf(object value)
        {
            var parameter = value as SpannerParameter;
            if (null == parameter)
                throw new ArgumentException("Argument must be of type DbParameter", "value");
            return _parameters.IndexOf(parameter);
        }

        public override void Insert(int index, object value)
        {
            var parameter = value as SpannerParameter;
            if (parameter == null)
                throw new ArgumentException("Only SpannerParameter objects may be stored");

            _parameters.Insert(index, parameter);
        }

        public override void Remove(object value)
        {
            var parameter = value as SpannerParameter;
            _parameters.Remove(parameter);
        }

        public override void RemoveAt(int index)
        {
            _parameters.RemoveAt(index);
        }

        public override void RemoveAt(string parameterName)
        {
            var parameter = GetParameter(parameterName);
            Remove(parameter);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            InternalSetParameter(index, value as SpannerParameter);
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            InternalSetParameter(parameterName, value as SpannerParameter);
        }
        private void InternalSetParameter(string parameterName, SpannerParameter value)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
                throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
            InternalSetParameter(index, value);
        }

        private void InternalSetParameter(int index, SpannerParameter value)
        {
            SpannerParameter newParameter = value as SpannerParameter;
            if (newParameter == null)
                throw new ArgumentException("The new value must be a SpannerParameter object");

            CheckIndex(index);
            SpannerParameter p = (SpannerParameter)_parameters[index];

            // then we add in the new parameter
            _parameters[index] = newParameter;
        }
        void CheckIndex(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Parameter index is out of range.");
        }

        public override int Count => _parameters.Count;
        public override object SyncRoot => (_parameters as IList).SyncRoot;

        public override int IndexOf(string parameterName)
        {
            int index = 0;
            foreach (SpannerParameter p in this)
            {
                if (parameterName == p.ParameterName)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public override IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        protected override DbParameter GetParameter(int index)
        {
            return (DbParameter)InternalGetParameter(index);
        }
        private SpannerParameter InternalGetParameter(int index)
        {
            CheckIndex(index);
            return _parameters[index];
        }
        protected override DbParameter GetParameter(string parameterName)
        {
            return InternalGetParameter(parameterName);
        }

        private SpannerParameter InternalGetParameter(string parameterName)
        {
            int index = IndexOf(parameterName);
            if (index < 0)
            {
                // check to see if the user has added the parameter without a
                // parameter marker.  If so, kindly tell them what they did.
                if (parameterName.StartsWith("@", StringComparison.Ordinal) ||
                            parameterName.StartsWith("?", StringComparison.Ordinal))
                {
                    string newParameterName = parameterName.Substring(1);
                    index = IndexOf(newParameterName);
                    if (index != -1)
                        return _parameters[index];
                }
                throw new ArgumentException("Parameter '" + parameterName + "' not found in the collection.");
            }
            return _parameters[index];
        }

        public override bool Contains(string parameterName)
        {
            return IndexOf(parameterName) != -1;
        }

        public override void CopyTo(Array array, int index)
        {
            _parameters.ToArray().CopyTo(array, index);
        }

        public override void AddRange(Array values)
        {
            foreach (DbParameter p in values)
            {
                Add(p);
            }
        }
    }
}
