using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Spanner.V1;

namespace NG.Data.Spanner
{
    public class SpannerTransaction: DbTransaction
    {
        private readonly SpannerConnection _connection;
        bool _isFinished;

        private Session _transactionSession;
        private Transaction _openedTransaction;

        public SpannerTransaction(SpannerConnection connection)
        {
            _connection = connection;

            _transactionSession = _connection.SpannerClient.CreateSession(_connection.DatabaseName);

            _openedTransaction = _connection.SpannerClient.BeginTransaction(new BeginTransactionRequest
            {
                SessionAsSessionName = _transactionSession.SessionName,
                Options = new TransactionOptions
                {
                    ReadWrite = new TransactionOptions.Types.ReadWrite() // TODO should support read only mode
                }
            });
            _connection.CurrentTransaction = this;
        }

        public override void Commit()
        {
            //_connection.SpannerClient.Commit();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken)) => CommitAsync(_connection.AsyncIOBehavior, cancellationToken);

        internal async Task CommitAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
        {
            VerifyNotDisposed();
            if (_isFinished)
                throw new InvalidOperationException("Already committed or rolled back.");

            if (_connection.CurrentTransaction == this)
            {
                using (var cmd = new SpannerCommand()) // "commit", _connection, this
                {
                    await cmd.ExecuteNonQueryAsync(ioBehavior, cancellationToken).ConfigureAwait(false);
                }
                _connection.CurrentTransaction = null;
                _isFinished = true;
            }
            else if (_connection.CurrentTransaction != null)
            {
                throw new InvalidOperationException("This is not the active transaction.");
            }
            else if (_connection.CurrentTransaction == null)
            {
                throw new InvalidOperationException("There is no active transaction.");
            }
        }

        public override void Rollback()
        {
            if (_openedTransaction != null)
            {
                _connection.SpannerClient.Rollback(_transactionSession.SessionName, _openedTransaction.Id);
            }
        }

        protected override DbConnection DbConnection => _connection;
        public override IsolationLevel IsolationLevel { get; }

        private void VerifyNotDisposed()
        {
            if (_connection == null)
                throw new ObjectDisposedException(nameof(SpannerTransaction));
        }

        public Transaction Transaction => _openedTransaction;
        public Session ActiveSession => _transactionSession;
    }
}
