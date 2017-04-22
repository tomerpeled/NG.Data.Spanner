using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerRelationalTransaction: IDbContextTransaction, IInfrastructure<DbTransaction>
    {
        private readonly IRelationalConnection _relationalConnection;
        private readonly SpannerTransaction _dbTransaction;
        private readonly bool _transactionOwned;

        private bool _disposed;

        public SpannerRelationalTransaction(
            [NotNull] IRelationalConnection connection,
            [NotNull] SpannerTransaction transaction,
            bool transactionOwned)
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(transaction, nameof(transaction));

            if (connection.DbConnection != transaction.Connection)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAssociatedWithDifferentConnection);
            }

            _relationalConnection = connection;
            _dbTransaction = transaction;
            _transactionOwned = transactionOwned;
        }

        public void Commit()
        {
            _dbTransaction.Commit();
            ClearTransaction();
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _dbTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            ClearTransaction();
        }

        public void Rollback()
        {
            _dbTransaction.Rollback();
            ClearTransaction();
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //await _dbTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            //ClearTransaction();
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (_transactionOwned)
                {
                    _dbTransaction.Dispose();
                }
                ClearTransaction();
            }
        }

        private void ClearTransaction()
        {
            Debug.Assert((_relationalConnection.CurrentTransaction == null) ||
                         (_relationalConnection.CurrentTransaction == this));
            _relationalConnection.UseTransaction(null);
        }

        DbTransaction IInfrastructure<DbTransaction>.Instance => _dbTransaction;

        public SpannerTransaction SpannerTransaction => _dbTransaction;
    }
}
