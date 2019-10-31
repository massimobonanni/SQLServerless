using SQLServerless.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLServerless.SQLCore.Helpers
{
    internal static class QueryFactory
    {
        public static string GetChangeTrackingCurrentVersionQuery()
        {
            return "select CHANGE_TRACKING_CURRENT_VERSION()";
        }

        public static string GetChangesQuery(string tableName, string keyName, long trackingVersion)
        {
            return $"SELECT P.*, CT.SYS_CHANGE_VERSION, CT.SYS_CHANGE_CREATION_VERSION, CT.SYS_CHANGE_OPERATION, CT.SYS_CHANGE_COLUMNS, CT.SYS_CHANGE_CONTEXT FROM {tableName} AS P RIGHT OUTER JOIN CHANGETABLE(CHANGES {tableName}, {trackingVersion}) AS CT ON P.{keyName} = CT.{keyName}";
        }

        public static string GetInsertStatement(string tableName, TableRowData row)
        {
            // INSERT INTO Production.UnitMeasure (Name, UnitMeasureCode, ModifiedDate) VALUES (N'Square Yards', N'Y2', GETDATE());
            var strBuilder = new StringBuilder();
            strBuilder.Append($"INSERT INTO {tableName} (");
            for (int i = 0; i < row.Count; i++)
            {
                strBuilder.Append($"{row.Keys.ElementAt(i)}");
                if (i < row.Count - 1)
                    strBuilder.Append(", ");
            }
            strBuilder.Append(") VALUES (");

            for (int i = 0; i < row.Count; i++)
            {
                strBuilder.Append($"{GetSqlFormattedValue(row.Values.ElementAt(i))}");
                if (i < row.Count - 1)
                    strBuilder.Append(", ");
            }
            strBuilder.Append(") ");

            return strBuilder.ToString();
        }

        private static string GetSqlFormattedValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            Type valueType = value.GetType();

            if (valueType == typeof(string)
                || valueType == typeof(Guid))
            {
                return $"'{value}'";
            }

            if (valueType == typeof(DateTime))
            {
                return $"'{(DateTime)value:yyyy-MM-dd HH:mm:ss}'";
            }

            return value.ToString();
        }
    }
}
