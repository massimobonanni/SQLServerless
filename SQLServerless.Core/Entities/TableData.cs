using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Core.Entities
{
    public class TableData
    {
        public TableData()
        {
            Rows = new List<TableRowData>();
        }

        public string TableName { get; set; }
        public List<TableRowData> Rows { get; set; }
    }

    public class TableRowData : Dictionary<string, object>
    {
    }
}
