using Microsoft.Azure.WebJobs.Host.Protocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Extensions.Triggers
{
    internal class SQLTriggerParameterDescriptor : TriggerParameterDescriptor
    {
        public string TableName { get; internal set; }

        public string KeyName { get; internal set; }

        public override string GetTriggerReason(IDictionary<string, string> arguments)
        {
            return base.GetTriggerReason(arguments);
        }
    }
}
