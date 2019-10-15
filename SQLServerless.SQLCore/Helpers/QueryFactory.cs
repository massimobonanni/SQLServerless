using System;
using System.Collections.Generic;
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
    }
}
