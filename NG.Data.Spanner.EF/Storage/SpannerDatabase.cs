using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Spanner.V1;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using NG.Data.Spanner.EF.Storage.Internal;
using NG.Data.Spanner.EF.Utils;

namespace NG.Data.Spanner.EF.Storage
{
    public class SpannerDatabase: Database
    {
        private readonly SpannerClient _spannerClient;
        private readonly DatabaseName _databaseName;
        private readonly SpannerRelationalConnection _spannerRelationalConnection;
        public SpannerDatabase(IQueryCompilationContextFactory queryCompilationContextFactory, SpannerRelationalConnection relationalConnection) : base(queryCompilationContextFactory)
        {
            _spannerClient = SpannerClient.Create();
            var csb = new SpannerConnectionStringBuilder(relationalConnection.ConnectionString);
            _databaseName = new DatabaseName(csb.ProjectId, csb.InstanceId, csb.DBName);
            _spannerRelationalConnection = relationalConnection;
        }

        public override int SaveChanges(IReadOnlyList<IUpdateEntry> entries)
        {
            throw new NotImplementedException();
        }

        public override async Task<int> SaveChangesAsync(IReadOnlyList<IUpdateEntry> entries, CancellationToken cancellationToken = new CancellationToken())
        {
            var mutations = new List<Mutation>();
            foreach (var entry in entries)
            {
                var tableName = DBUtilsHelper.Pluralize(entry.EntityType.ClrType.Name);
                var properties = entry.EntityType.GetProperties();
                switch (entry.EntityState)
                {
                    case EntityState.Added:
                    {
                        var mutation = CreateAddMutation(entry, tableName, properties);
                        mutations.Add(mutation);
                        break;
                    }
                    case EntityState.Modified:
                    {
                        var mutation = CreateUpdateMutation(entry, properties, tableName);
                        mutations.Add(mutation);
                        break;
                    }
                    case EntityState.Deleted:
                    {
                        var mutation = CreateDeleteMutation(entry, tableName);
                        mutations.Add(mutation);
                        break;
                    }
                    default:
                        break;
                }
            }

            if (!mutations.Any()) return 0;

            await RunTranasactionAsync(mutations).ConfigureAwait(false);
            return mutations.Count;
        }

        private Mutation CreateAddMutation(IUpdateEntry entry, string tableName, IEnumerable<IProperty> properties)
        {
            var columns = new List<string>();
            var values = new List<Value>();
         
            foreach (var property in properties)
            {
                var propertyVal = property.GetPropertyAccessors().CurrentValueGetter.DynamicInvoke(entry);
                columns.Add(property.Name);
                var value = GetValueFromProperty(property, propertyVal);
                values.Add(value);
            }

            //var entityEntry = entry.ToEntityEntry();
            //var entityEntryNavigations = entityEntry.Navigations;
            //var navigations = entry.EntityType.GetNavigations();
            //foreach (var navigation in navigations)
            //{
            //    var clrType = navigation.ClrType;
            //    var navigationProperties = navigations.SelectMany(n => n.ForeignKey.DeclaringEntityType.FindPrimaryKey().Properties);
            //    foreach (var property in navigationProperties)
            //    {
            //        if (columns.Contains(property.Name))
            //        {
            //            continue;
            //        }
            //        columns.Add(property.Name);
            //        var obj = entityEntryNavigations.Where(n => n.Metadata.Name == clrType.Name).Select(n => n.CurrentValue).FirstOrDefault();
            //        var propertyVal = clrType.GetProperty(property.Name).GetValue(obj);
            //        var value = GetValueFromProperty(property, propertyVal);
            //        values.Add(value);
            //    }
            //}

            var mutation = new Mutation
            {
                Insert = new Mutation.Types.Write
                {
                    Table = tableName,
                    Columns = { columns },
                    Values = { new ListValue { Values = { values } } }
                }
            };
            return mutation;
        }
        private Mutation CreateUpdateMutation(IUpdateEntry entry, IEnumerable<IProperty> properties, string tableName)
        {
            var columns = new List<string>();
            var values = new List<Value>();
            var primaryKey = entry.EntityType.FindPrimaryKey();
            var primaryKeyProperties = primaryKey.Properties;

            // Assumption: The primary keys cannot be changed
            foreach (var pk in primaryKeyProperties)
            {
                columns.Add(pk.Name);
                var propertyVal = pk.GetPropertyAccessors().CurrentValueGetter.DynamicInvoke(entry);
                var value = GetValueFromProperty(pk, propertyVal);
                values.Add(value);
            }
            foreach (var property in properties)
            {
                if (entry.IsModified(property))
                {
                    columns.Add(property.Name);
                    var currentVal = property.GetPropertyAccessors().CurrentValueGetter.DynamicInvoke(entry);
                    var value = GetValueFromProperty(property, currentVal);
                    values.Add(value);
                }
            }

            var mutation = new Mutation
            {
                Update = new Mutation.Types.Write
                {
                    Table = tableName,
                    Columns = {columns},
                    Values = {new ListValue {Values = {values}}}
                }
            };
            return mutation;
        }
        private Mutation CreateDeleteMutation(IUpdateEntry entry, string tableName)
        {
            var columns = new List<string>();
            var values = new List<Value>();
            var primaryKey = entry.EntityType.FindPrimaryKey();
            var primaryKeyProperties = primaryKey.Properties;
            foreach (var pk in primaryKeyProperties)
            {
                columns.Add(pk.Name);
                var propertyVal = pk.GetPropertyAccessors().CurrentValueGetter.DynamicInvoke(entry);
                var value = GetValueFromProperty(pk, propertyVal);
                values.Add(value);
            }

            var mutation = new Mutation
            {
                Delete = new Mutation.Types.Delete
                {
                    Table = tableName,
                    KeySet = new KeySet
                    {
                        Keys = { new ListValue
                        {
                            Values = { values }
                        }}
                    }
                }
            };
            return mutation;
        }

        private Value GetValueFromProperty(IProperty property, object propertyVal)
        {
            return GetValueFromProperty(property.ClrType, propertyVal);
        }
        private Value GetValueFromProperty(System.Type propertyType, object propertyVal)
        {
            var value = new Value();
            if (propertyVal == null)
            {
                value.NullValue = NullValue.NullValue;
                return value;
            }

            if (propertyType == typeof(int) || propertyType == typeof(int?))
            {
                value.StringValue = propertyVal.ToString();
            }
            else if (propertyType.IsEnum)
            {
                value.StringValue = ((int)propertyVal).ToString();
            }
            else if (propertyType == typeof(long) || propertyType == typeof(long?))
            {
                value.StringValue = propertyVal.ToString();
            }
            else if (propertyType == typeof(string))
            {
                value.StringValue = propertyVal.ToString();
            }
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                value.BoolValue = Convert.ToBoolean(propertyVal);
            }
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                value.StringValue = Convert.ToDateTime(propertyVal).ToSpannerFormat();
            }
            else if (propertyType == typeof(double) || propertyType == typeof(double?))
            {
                value.NumberValue = Convert.ToDouble(propertyVal);
            }
            else if (propertyType.IsClass)
            {
                var properties = propertyType.GetProperties(); // TODO: Improve this bad performance - need to cache the accessors of the complex objects - See maybe EF already did this for us
                var fields = new MapField<string, Value>();
                foreach (var property in properties)
                {
                    var val = property.GetValue(propertyVal);
                    fields.Add(property.Name, GetValueFromProperty(property.PropertyType, val));
                }
                value.StructValue = new Struct
                {
                    Fields = { fields }
                };
            }
            else if (propertyType.IsArray)
            {
                // Not supported yert
                var array = propertyVal as object[];
                value.ListValue = new ListValue
                {
                    //Values = { array.Select(a => GetValueFromProperty( ,a)) }
                };
            }
            return value;
        }

        private async Task RunTranasactionAsync(List<Mutation> mutations)
        {
            Transaction transaction = null;
            Session session = null;
            var spannerTransaction = _spannerRelationalConnection.CurrentTransaction as SpannerRelationalTransaction;
            if (spannerTransaction != null)
            {
                transaction = spannerTransaction.SpannerTransaction.Transaction;
                session = spannerTransaction.SpannerTransaction.ActiveSession;
            }
            else
            {
                // TODO: Add retry mechanism
                session = await _spannerClient.CreateSessionAsync(_databaseName).ConfigureAwait(false);

                transaction = _spannerClient.BeginTransaction(new BeginTransactionRequest
                {
                    SessionAsSessionName = session.SessionName,
                    Options = new TransactionOptions
                    {
                        ReadWrite = new TransactionOptions.Types.ReadWrite()
                    }
                });
            }

            var commitResponse = await _spannerClient.CommitAsync(new CommitRequest
            {
                Mutations = { mutations },
                TransactionId = transaction.Id,
                SessionAsSessionName = session.SessionName
            }).ConfigureAwait(false);

            await _spannerClient.DeleteSessionAsync(session.SessionName).ConfigureAwait(false);
        }


    }
}
