using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using SQLServerless.SQLCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SQLServerless.SQLCore.Implementations
{
    public class SQLChangeTracker : IChangeTracker
    {
        private string connectionString;
        private long lastTrackingVersion = 0;
        private string tableName;
        private string keyName;

        #region [ Private Methods ]

        private async Task<long> GetCurrentTrackingVersionAsync(CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = QueryFactory.GetChangeTrackingCurrentVersionQuery();

                connection.Open();
                using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    reader.Read();
                    var value = reader.GetInt64(0);
                    return value;
                }
            }
        }

        private async Task<TableData> GetChangesAsync(string tableName, string keyName, long trackingVersion, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = QueryFactory.GetChangesQuery(tableName, keyName, trackingVersion);

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    var columnNames = reader.GetColumnNames();
                    var tableData = new TableData() { TableName = tableName };
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var fieldCount = reader.FieldCount;
                        var row = new object[fieldCount];
                        reader.GetValues(row);
                        var dataRow = new TableRowData();
                        for (var i = 0; i < row.Count() - 6; i++)
                        {
                            if (columnNames[i] == keyName) // retrieve key field from data change table to support delete operation
                                dataRow.Add(columnNames[i], row[row.Count() - 6]);
                            else
                                dataRow.Add(columnNames[i], row[i]);
                        }
                        dataRow.Operation = row[row.Count() - 1].ToRowOperation();
                        tableData.Rows.Add(dataRow);
                    }
                    return tableData;
                }
            }
        }

        private bool AreTableOrKeyChanged(string tableName, string keyName)
        {
            return tableName != this.tableName || keyName != this.keyName;
        }
        #endregion [ Private Methods ]

        #region [ Interface IChangeTracker ]
        public async Task<TableData> GetChangesAsync(string tableName, string keyName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException(nameof(tableName));

            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentException(nameof(keyName));

            long currentTrackingVersion = 0;

            if (AreTableOrKeyChanged(tableName, keyName))
            {
                this.lastTrackingVersion = await GetCurrentTrackingVersionAsync(cancellationToken);
                currentTrackingVersion = this.lastTrackingVersion;
                this.tableName = tableName;
                this.keyName = keyName;
            }
            else
            {
                currentTrackingVersion = await GetCurrentTrackingVersionAsync(cancellationToken);
            }


            TableData changes = null;
            if (currentTrackingVersion != lastTrackingVersion)
            {
                changes = await this.GetChangesAsync(this.tableName, this.keyName, this.lastTrackingVersion, cancellationToken);
                changes.TableName = this.tableName;
                this.lastTrackingVersion = currentTrackingVersion;
            }

            return changes;
        }

        public void SetConfiguration(ChangeTrackerConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new ArgumentException(nameof(config.ConnectionString));

            if (config.ConnectionString != this.connectionString)
            {
                this.connectionString = config.ConnectionString;
                this.lastTrackingVersion = 0;
            }
        }

        #endregion [ Interface IChangeTracker ]

    }
}
