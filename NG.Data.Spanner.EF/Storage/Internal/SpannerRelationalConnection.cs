using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerRelationalConnection: IRelationalConnection
    {
        private SpannerConnection _connection;
        private readonly string _connectionString;

        public SpannerRelationalConnection([NotNull] IDbContextOptions options)
        {
            var relationalOptions = RelationalOptionsExtension.Extract(options);
            if (relationalOptions.Connection != null)
            {
                if (!string.IsNullOrWhiteSpace(relationalOptions.ConnectionString))
                {
                    throw new InvalidOperationException(RelationalStrings.ConnectionAndConnectionString);
                }
            }
            else if (!string.IsNullOrWhiteSpace(relationalOptions.ConnectionString))
            {
                _connectionString = relationalOptions.ConnectionString;
                ConnectionString = _connectionString;
            }
            else
            {
                throw new InvalidOperationException(RelationalStrings.NoConnectionOrConnectionString);
            }
            _connection = new SpannerConnection(_connectionString);

            //var relationalOptions = RelationalOptionsExtension.Extract(options);
            //_commandTimeout = relationalOptions.CommandTimeout;
        }


        internal readonly SemaphoreSlim CommandLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1);

        public IDbContextTransaction BeginTransaction() => BeginTransaction(IsolationLevel.Unspecified);

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbContextTransaction CurrentTransaction { get; [param: CanBeNull] protected set; }

        public void Open()
        {
            _connection.Open();
            //_connectionLock.Wait();
            //try
            //{
            //    if (_openedCount == 0 && DbConnection.State != ConnectionState.Open)
            //    {
            //        DbConnection.Open();
            //        _openedInternally = true;
            //    }
            //    _openedCount++;
            //}
            //finally
            //{
            //    _connectionLock.Release();
            //}
        }

        public Task OpenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _connection.OpenAsync(cancellationToken);
        }

        public void Close()
        {
            _connection.Close();
        }

        public string ConnectionString { get; }
        public DbConnection DbConnection => _connection ?? (_connection = new SpannerConnection(_connectionString));
        public int? CommandTimeout { get; set; }
        public bool IsMultipleActiveResultSetsEnabled => true;
        public IValueBufferCursor ActiveCursor { get; set; }

        IDbContextTransaction IDbContextTransactionManager.CurrentTransaction
        {
            get { throw new NotImplementedException(); }
        }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (CurrentTransaction != null)
            {
                throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
            }
            Open();
            return BeginTransactionWithNoPreconditions(isolationLevel);
        }

        private IDbContextTransaction BeginTransactionWithNoPreconditions(IsolationLevel isolationLevel)
        {

            // ReSharper disable once AssignNullToNotNullAttribute
            CurrentTransaction = new SpannerRelationalTransaction(this, _connection.BeginTransaction(isolationLevel) as SpannerTransaction, true);
            return CurrentTransaction;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public IDbContextTransaction UseTransaction(DbTransaction transaction)
        {
            if (transaction == null)
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction = null;
                    Close();
                }
            }
            else
            {
                var spannerTransaction = transaction as SpannerTransaction;
                if (spannerTransaction == null)
                {
                    throw new InvalidCastException("transaction must be of the type SpannerTransaction");
                }
                if (CurrentTransaction != null)
                {
                    throw new InvalidOperationException(RelationalStrings.TransactionAlreadyStarted);
                }
                Open();
                CurrentTransaction = new SpannerRelationalTransaction(this, spannerTransaction, false);
            }
            return CurrentTransaction;
        }

        public void Dispose()
        {
            CurrentTransaction?.Dispose();
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public SpannerRelationalConnection CreateMasterConnection()
        {
            //var csb = new SpannerConnectionStringBuilder(ConnectionString)
            //{
            //    Database = "mysql",
            //    Pooling = false
            //};
            //var optionsBuilder = new DbContextOptionsBuilder();
            //optionsBuilder.UseMySql(csb.ConnectionString);
            //return new SpannerRelationalConnection(optionsBuilder.Options, _logger);

            return new SpannerRelationalConnection(null);
        }
    }
}
