using SQLServerless.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SQLServerless.Core.Interfaces
{
    public interface IDBService
    {
        void SetConfiguration(DBConfiguration config);

        Task<bool> ExecuteCommandAsync(Command command, CancellationToken cancellationToken);

        Task<bool> InsertTableDataAsync(TableData table, CancellationToken cancellationToken);
    }
}
