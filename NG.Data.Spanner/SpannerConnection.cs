using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Google.Cloud.Spanner.V1;

namespace NG.Data.Spanner
{
    public class SpannerConnection: DbConnection
    {
        private SpannerClient _spannerClient;
        private readonly DatabaseName _databaseName;
        private readonly SpannerConnectionStringBuilder _spannerConnectionStringBuilder;

        public SpannerConnection(string connectionString)
        {
            _spannerConnectionStringBuilder = new SpannerConnectionStringBuilder(connectionString);
            _databaseName = new DatabaseName(_spannerConnectionStringBuilder.ProjectId, _spannerConnectionStringBuilder.InstanceId, _spannerConnectionStringBuilder.DBName);
            State = ConnectionState.Closed;
        }

        internal SpannerTransaction CurrentTransaction { get; set; }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (isolationLevel == IsolationLevel.Unspecified)
                return BeginTransaction();
            return BeginTransaction(isolationLevel);
        }

        public new SpannerTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.RepeatableRead);
        }

        public new SpannerTransaction BeginTransaction(IsolationLevel iso)
        {
            SpannerTransaction t = new SpannerTransaction(this);//this, iso
            return t;
        }

        public override void Close()
        {
            // _spannerClient.DeleteSession(_activeSession.SessionName);
            OnStateChange(new StateChangeEventArgs(State, ConnectionState.Closed));
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            if (State == ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection is already opened");
            }

            OnStateChange(new StateChangeEventArgs(State, ConnectionState.Connecting));

            _spannerClient = SpannerClient.Create();

            OnStateChange(new StateChangeEventArgs(State, ConnectionState.Open));

        }

        public override string ConnectionString { get; set; }
        public override string Database => _spannerConnectionStringBuilder.DBName;
        public override ConnectionState State { get; }
        public override string DataSource { get; }
        public override string ServerVersion { get; }

        protected override DbCommand CreateDbCommand()
        {
            var c = new SpannerCommand {Connection = this};
            return c;
        }

        public async Task<ResultSet> RunQuery(string commandText)
        {

            Session activeSession;
            if (CurrentTransaction != null)
            {
                activeSession = CurrentTransaction.ActiveSession;
            }
            else
            {
                DatabaseName database = new DatabaseName(_spannerConnectionStringBuilder.ProjectId, _spannerConnectionStringBuilder.InstanceId, _spannerConnectionStringBuilder.DBName);
                activeSession = await _spannerClient.CreateSessionAsync(database).ConfigureAwait(false);
            }

            var request = new ExecuteSqlRequest
            {
                SessionAsSessionName = activeSession.SessionName,
                Sql = commandText
            };

            if (CurrentTransaction != null)
            {
                request.Transaction = new TransactionSelector
                {
                    Id = CurrentTransaction.Transaction.Id
                };
            }

            return await _spannerClient.ExecuteSqlAsync(request);
        }

        public static void ClearAllPools()
        {
        }

        public static void ClearPool(SpannerConnection connection)
        {
        }

        internal IOBehavior AsyncIOBehavior => IOBehavior.Asynchronous;
        internal SpannerDataReader ActiveReader { get; set; }

        public SpannerClient SpannerClient => _spannerClient;

        public DatabaseName DatabaseName => _databaseName;
    }
}
