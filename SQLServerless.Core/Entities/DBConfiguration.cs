using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Core.Entities
{
    public class DBConfiguration
    {
        public string ConnectionString { get; set; }
        public bool IsServerless { get; set; }
    }
}
