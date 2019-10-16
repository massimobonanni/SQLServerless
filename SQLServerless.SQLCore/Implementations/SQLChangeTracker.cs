using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using SQLServerless.SQLCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        private async Task<long> GetCurrentTrackingVersionAsync()
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = QueryFactory.GetChangeTrackingCurrentVersionQuery();

                connection.Open();
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    reader.Read();
                    var value = reader.GetInt64(0);
                    return value;
                }
            }
        }

        private async Task<TableData> GetChangesAsync(string tableName, string keyName, long trackingVersion)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = QueryFactory.GetChangesQuery(tableName, keyName, trackingVersion);

                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var tableData = new TableData();
                    while (await reader.ReadAsync())
                    {
                        var fieldCount = reader.FieldCount;
                        var row = new object[fieldCount];
                        reader.GetValues(row);
                        tableData.Rows.Add(row.Take(row.Count() - 5).ToList());
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
        public async Task<TableData> GetChangesAsync(string tableName, string keyName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException(nameof(tableName));

            if (string.IsNullOrWhiteSpace(keyName))
                throw new ArgumentException(nameof(keyName));

            long currentTrackingVersion = 0;

            if (AreTableOrKeyChanged(tableName, keyName))
            {
                this.lastTrackingVersion = await GetCurrentTrackingVersionAsync();
                currentTrackingVersion = this.lastTrackingVersion;
                this.tableName = tableName;
                this.keyName = keyName;
            }
            else
            {
                currentTrackingVersion = await GetCurrentTrackingVersionAsync();
            }


            TableData changes = null;
            if (currentTrackingVersion != lastTrackingVersion)
            {
                changes = await this.GetChangesAsync(this.tableName, this.keyName, this.lastTrackingVersion);
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
