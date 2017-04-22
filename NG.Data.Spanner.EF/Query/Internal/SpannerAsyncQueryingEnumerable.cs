using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using NG.Data.Spanner.EF.Storage.Internal;

namespace NG.Data.Spanner.EF.Query.Internal
{
    public class SpannerAsyncQueryingEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly SpannerQueryContext _queryContext;
        private readonly IAsyncEnumerable<T> _source;

        public SpannerAsyncQueryingEnumerable(SpannerQueryContext queryContext, IAsyncEnumerable<T> source)
        {
            _queryContext = queryContext;
            _source = source;
        }

        public IAsyncEnumerator<T> GetEnumerator()
            => new SpannerAsyncEnumerator(_queryContext, _source.GetEnumerator());

        private sealed class SpannerAsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly IAsyncEnumerator<T> _enumerator;
            private readonly SpannerQueryContext _queryContext;
            //private SpannerRelationalTransaction _transaction;
            private bool _tryInitTransaction;

            public SpannerAsyncEnumerator(SpannerQueryContext queryContext, IAsyncEnumerator<T> enumerator)
            {
                _queryContext = queryContext;
                _enumerator = enumerator;
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken)
            {
                if (!_tryInitTransaction)
                {
                    if (_queryContext.HasInclude && _queryContext.Connection.CurrentTransaction == null)
                    {
                       // _transaction = await _queryContext.Connection.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellationToken).ConfigureAwait(false) as SpannerRelationalTransaction;
                    }
                    _tryInitTransaction = true;
                }
                if (!await _enumerator.MoveNext(cancellationToken).ConfigureAwait(false))
                {
                    //if (_transaction != null)
                    //{
                    //    await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                    //}
                    return false;
                }
                return true;
            }

            public T Current => _enumerator.Current;

            public void Dispose()
            {
                _enumerator.Dispose();
               // _transaction?.Dispose();
            }
        }
    }
}
