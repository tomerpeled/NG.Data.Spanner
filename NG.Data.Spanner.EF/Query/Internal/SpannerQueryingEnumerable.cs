using System.Collections;
using System.Collections.Generic;
using System.Data;
using NG.Data.Spanner.EF.Storage.Internal;

namespace NG.Data.Spanner.EF.Query.Internal
{
    /// Wraps an QueryingEnumerable in an enumerable that opens a RepeatableRead transaction when querying with Includes
	/// This way every statement within the Includes gets a consistent snapshot of the database
    public class SpannerQueryingEnumerable<T>: IEnumerable<T>
    {
        private readonly SpannerQueryContext _queryContext;
        private readonly IEnumerable<T> _source;

        public SpannerQueryingEnumerable(SpannerQueryContext queryContext, IEnumerable<T> source)
        {
            _queryContext = queryContext;
            _source = source;
        }

        public IEnumerator<T> GetEnumerator()
            => new SpannerEnumerator(_queryContext, _source.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class SpannerEnumerator : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly SpannerQueryContext _queryContext;
            private SpannerRelationalTransaction _transaction;
            private bool _tryInitTransaction;

            public SpannerEnumerator(SpannerQueryContext queryContext, IEnumerator<T> enumerator)
            {
                _queryContext = queryContext;
                _enumerator = enumerator;
            }

            public bool MoveNext()
            {
                if (!_tryInitTransaction)
                {
                    if (_queryContext.HasInclude && _queryContext.Connection.CurrentTransaction == null)
                    {
                        _transaction = _queryContext.Connection.BeginTransaction(IsolationLevel.RepeatableRead) as SpannerRelationalTransaction;
                    }
                    _tryInitTransaction = true;
                }
                if (!_enumerator.MoveNext())
                {
                    _transaction?.Commit();
                    return false;
                }
                return true;
            }

            public void Reset() => _enumerator.Reset();

            public T Current => _enumerator.Current;

            object IEnumerator.Current => _enumerator.Current;

            public void Dispose()
            {
                _enumerator.Dispose();
                _transaction?.Dispose();
            }
        }
    }
}
