using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Core.Entities
{
    public class TableData
    {
        public TableData()
        {
            Rows = new List<List<object>>();
        }

        public string TableName { get; set; }
        public List<List<object>> Rows { get; set; }
    }
}
