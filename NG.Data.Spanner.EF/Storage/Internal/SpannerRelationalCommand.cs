﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Storage.Internal
{
    public class SpannerRelationalCommand: RelationalCommand
    {
        public SpannerRelationalCommand(ISensitiveDataLogger logger, DiagnosticSource diagnosticSource, string commandText, IReadOnlyList<IRelationalParameter> parameters) : base(logger, diagnosticSource, commandText, parameters)
        {
        }

        protected override object Execute(
            IRelationalConnection connection,
            string executeMethod,
            IReadOnlyDictionary<string, object> parameterValues,
            bool closeConnection = true)
        {
            return ExecuteAsync(IOBehavior.Synchronous, connection, executeMethod, parameterValues, closeConnection)
                .GetAwaiter()
                .GetResult();
        }

        protected override async Task<object> ExecuteAsync(
            IRelationalConnection connection,
            string executeMethod,
            IReadOnlyDictionary<string, object> parameterValues,
            bool closeConnection = true,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteAsync(IOBehavior.Asynchronous, connection, executeMethod, parameterValues, closeConnection, cancellationToken).ConfigureAwait(false);
        }

        private async Task<object> ExecuteAsync(
            IOBehavior ioBehavior,
            [NotNull] IRelationalConnection connection,
            [NotNull] string executeMethod,
            [CanBeNull] IReadOnlyDictionary<string, object> parameterValues,
            bool closeConnection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(connection, nameof(connection));
            Check.NotEmpty(executeMethod, nameof(executeMethod));

            using (var dbCommand = CreateCommand(connection, parameterValues))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var spannerConnection = connection as SpannerRelationalConnection;
                object result;
                var locked = false;
                var startTimestamp = Stopwatch.GetTimestamp();

                try
                {
                    //if (ioBehavior == IOBehavior.Asynchronous)
                    //    // ReSharper disable once PossibleNullReferenceException
                    //    await spannerConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    //else
                    //    // ReSharper disable once PossibleNullReferenceException
                    //    spannerConnection.Open();

                    if (ioBehavior == IOBehavior.Asynchronous)
                        await spannerConnection.CommandLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                    else
                        spannerConnection.CommandLock.Wait(cancellationToken);
                    locked = true;

                    switch (executeMethod)
                    {
                        case nameof(ExecuteNonQuery):
                            {
                                result = ioBehavior == IOBehavior.Asynchronous ?
                                    await dbCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false) :
                                    dbCommand.ExecuteNonQuery();
                                break;
                            }
                        case nameof(ExecuteScalar):
                            {
                                result = ioBehavior == IOBehavior.Asynchronous ?
                                    await dbCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false) :
                                    dbCommand.ExecuteScalar();
                                break;
                            }
                        case nameof(ExecuteReader):
                            {
                                var dataReader = ioBehavior == IOBehavior.Asynchronous ?
                                    await dbCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false) :
                                    dbCommand.ExecuteReader();
                                result = new RelationalDataReader(connection, dbCommand, new WrappedSpannerDataReader(dataReader));
                                break;
                            }
                        default:
                            {
                                throw new NotSupportedException();
                            }
                    }
                    var currentTimestamp = Stopwatch.GetTimestamp();
                    Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);
                    //if (closeConnection)
                    //    connection.Close();
                    return result;
                }
                catch (Exception)
                {
                    var currentTimestamp = Stopwatch.GetTimestamp();
                    Logger.LogCommandExecuted(dbCommand, startTimestamp, currentTimestamp);
                    //if (opened)
                    //    connection.Close();
                    throw;
                }
                finally
                {
                    if (locked)
                        spannerConnection.CommandLock.Release();
                }
            }
        }

        private DbCommand CreateCommand(
            IRelationalConnection connection,
            IReadOnlyDictionary<string, object> parameterValues)
        {
            var command = connection.DbConnection.CreateCommand();

            command.CommandText = CommandText;

            if (connection.CurrentTransaction != null)
            {
                command.Transaction = connection.CurrentTransaction.GetDbTransaction();
            }

            if (connection.CommandTimeout != null)
            {
                command.CommandTimeout = (int)connection.CommandTimeout;
            }

            if (Parameters.Count > 0)
            {
                if (parameterValues == null)
                {
                    throw new InvalidOperationException(RelationalStrings.MissingParameterValue(Parameters[0].InvariantName));
                }

                foreach (var parameter in Parameters)
                {
                    object parameterValue;

                    if (parameterValues.TryGetValue(parameter.InvariantName, out parameterValue))
                    {
                        if (parameterValue != null)
                        {
                            if (parameterValue is char)
                                parameter.AddDbParameter(command, Convert.ToByte((char)parameterValue));
                            else if (parameterValue.GetType().FullName.StartsWith("System.JsonObject"))
                                parameter.AddDbParameter(command, parameterValue.ToString());
                            else if (parameterValue.GetType().GetTypeInfo().IsEnum)
                                parameter.AddDbParameter(command, Convert.ChangeType(parameterValue, Enum.GetUnderlyingType(parameterValue.GetType())));
                            else
                                parameter.AddDbParameter(command, parameterValue);
                        }
                        else
                        {
                            parameter.AddDbParameter(command, parameterValue);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            RelationalStrings.MissingParameterValue(parameter.InvariantName));
                    }
                }
            }

            return command;
        }

    }
}
