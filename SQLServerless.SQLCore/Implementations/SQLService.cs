using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using SQLServerless.SQLCore.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SQLServerless.SQLCore.Implementations
{
    public class SQLService : IDBService
    {
        private string connectionString;

        #region [ Private Methods ]
        private async Task<SqlConnection> OpenSqlConnectionAsync(CancellationToken cancellationToken)
        {
            SqlConnection connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }

        private SqlCommand CreateSqlCommand(SqlConnection connection, Command command)
        {
            var sqlCommand = connection.CreateCommand();

            sqlCommand.CommandText = command.StoredName;
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (var param in command.Parameters)
            {
                var paramName = param.Key;
                if (!paramName.StartsWith("@"))
                {
                    paramName = string.Concat("@", param.Key);
                }

                var value = command.GetSqlSafeValue(param);

                sqlCommand.Parameters.AddWithValue(paramName, value);
            }
            return sqlCommand;
        }

        private SqlCommand CreateSqlCommand(SqlConnection connection, TableData table)
        {
            var sqlCommand = connection.CreateCommand();

            sqlCommand.CommandText = QueryFactory.GetInsertStatement(table);
            sqlCommand.CommandType = System.Data.CommandType.Text;

            return sqlCommand;
        }
        #endregion [ Private Methods ]

        #region [ Interface IDBService ]
        public async Task<bool> ExecuteCommandAsync(Command command, CancellationToken cancellationToken)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var result = false;
            using (var sqlConnection = await OpenSqlConnectionAsync(cancellationToken))
            using (var sqlCommand = CreateSqlCommand(sqlConnection, command))
            {
                await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
                result = true;
            }

            return result;
        }

        public void SetConfiguration(DBConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.ConnectionString))
                throw new ArgumentException(nameof(config.ConnectionString));

            this.connectionString = config.ConnectionString;
        }

        public async Task<bool> InsertTableDataAsync(TableData table, CancellationToken cancellationToken)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            var result = false;
            using (var sqlConnection = await OpenSqlConnectionAsync(cancellationToken))
            using (var sqlCommand = CreateSqlCommand(sqlConnection, table))
            {
                await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
                result = true;
            }

            return result;
        }
        #endregion [ Interface IDBService ]
    }
}
