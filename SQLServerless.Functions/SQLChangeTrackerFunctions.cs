using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SQLServerless.Core.Entities;
using SQLServerless.Extensions.Triggers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Functions
{
    public static class SQLChangeTrackerFunctions
    {

        [FunctionName(nameof(ContactsChangeTracker))]
        public static void ContactsChangeTracker(
           [SQLTrigger("dbo.Contacts", "Id")] TableData req,
           ILogger log)
        {
            log.LogWarning($"Change in {req.TableName} table");

            foreach (var row in req.Rows)
            {
                string message = string.Empty;
                foreach (var item in row)
                {
                    message += $"{item}; ";
                }
                log.LogWarning(message);
            }
        }

    }
}
