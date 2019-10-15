using SQLServerless.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerless.Core.Interfaces
{
    public interface IChangeTracker
    {
        Task<TableData> GetChangesAsync(string tableName, string keyName);
    }
}
