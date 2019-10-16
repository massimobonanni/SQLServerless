using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Extensions.Triggers
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SQLTriggerAttribute : Attribute
    {
        public SQLTriggerAttribute(string tableName, string keyName)
        {
            this.TableName = tableName;
            this.KeyName = keyName;
        }

        [AppSetting(Default = "ConnectionString")]
        [AutoResolve]
        public string ConnectionString { get; set; }

        public string TableName { get; internal set; }

        public string KeyName { get; internal set; }

        [AppSetting(Default = "PollingInSecs")]
        [AutoResolve]
        public int PollingInSeconds { get; set; } = 10;
    }
}
