using Microsoft.Azure.WebJobs.Description;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Extensions.Bindings
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class SQLBindingAttribute : Attribute
    {
        [AppSetting(Default = "SQLBinding.Connectionstring")]
        public string ConnectionString { get; set; }

        public string TableName { get; set; }

    }
}
