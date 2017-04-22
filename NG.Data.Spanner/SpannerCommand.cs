using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NG.Data.Spanner
{
    public sealed class SpannerCommand: DbCommand
    {
        public SpannerCommand()
        {
            UpdatedRowSource = UpdateRowSource.Both;
            CommandType = CommandType.Text;
            DbParameterCollection = new SpannerDataParameterCollection();
            CommandText = string.Empty;
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection { get; }
        protected override DbTransaction DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }

        internal new SpannerConnection Connection { get; set; }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            return new SpannerParameter();
        }


        internal Task<DbDataReader> ExecuteReaderAsync(CommandBehavior behavior, IOBehavior ioBehavior,
        CancellationToken cancellationToken)
        {
            //VerifyValid();
            //return await m_commandExecutor.ExecuteReaderAsync(CommandText, m_parameterCollection, behavior, ioBehavior, cancellationToken).ConfigureAwait(false);
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var command = PrepareCommand();
            var resultSet = Connection.RunQuery(command).Result;
            var reader = new SpannerDataReader(resultSet);
            return reader;
        }
        protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            var command = PrepareCommand();
            var resultSet = await Connection.RunQuery(command).ConfigureAwait(false);
            var reader = new SpannerDataReader(resultSet);
            return reader;
        }
        private string PrepareCommand()
        {
            var command = CommandText.Trim();
            command = command.Replace("\"", "");
            foreach (SpannerParameter parameter in Parameters)
            {
                var isApostrophesRequired = parameter.DbType != DbType.Double && parameter.DbType != DbType.Int64;
                command = command.Replace(parameter.ParameterName, isApostrophesRequired ? $"'{parameter.Value}'" : $"{parameter.Value}");
            }
            return command;
        }

        public override int ExecuteNonQuery()
        {
            //   throw new NotImplementedException();
        //    SpannerInstanceCreator.DropDatabaseAsync().GetAwaiter().GetResult();
            return 1;
        }

        internal Task<int> ExecuteNonQueryAsync(IOBehavior ioBehavior, CancellationToken cancellationToken)
        {
            //VerifyValid();
           // return await m_commandExecutor.ExecuteNonQueryAsync(CommandText, m_parameterCollection, ioBehavior, cancellationToken).ConfigureAwait(false);
           throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        private void VerifyValid()
        {
            VerifyNotDisposed();
            if (DbConnection == null)
                throw new InvalidOperationException("Connection property must be non-null.");
            if (DbConnection.State != ConnectionState.Open && DbConnection.State != ConnectionState.Connecting)
                throw new InvalidOperationException("Connection must be Open; current state is {0}".FormatInvariant(DbConnection.State));
            if (DbTransaction != Connection.CurrentTransaction)
                throw new InvalidOperationException("The transaction associated with this command is not the connection's active transaction.");
            if (string.IsNullOrWhiteSpace(CommandText))
                throw new InvalidOperationException("CommandText must be specified");
            if (Connection.ActiveReader != null)
                throw new Exception("There is already an open DataReader associated with this Connection which must be closed first.");
        }

        private void VerifyNotDisposed()
        {
            if (DbParameterCollection == null)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}
