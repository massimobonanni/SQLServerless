using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;

namespace SQLServerless.Extensions.Bindings
{

    public class SQLBindingAsyncCollector : IAsyncCollector<TableRowData>
    {
        private readonly SQLBindingAttribute _attribute;
        private readonly INameResolver _nameResolver;
        private readonly IDBService _dbService;

        private readonly List<TableRowData> _rowsToInsert = new List<TableRowData>();

        public SQLBindingAsyncCollector(SQLBindingAttribute attribute,
            IDBService dbService, INameResolver nameResolver)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));
            if (nameResolver == null)
                throw new ArgumentNullException(nameof(nameResolver));

            this._attribute = attribute;
            this._nameResolver = nameResolver;
            this._dbService = dbService;

            this._dbService.SetConfiguration(new DBConfiguration()
            {
                ConnectionString = attribute.ConnectionString
            });
        }

        public Task AddAsync(TableRowData row, CancellationToken cancellationToken = default)
        {
            _rowsToInsert.Add(row);
            return Task.CompletedTask;
        }

        public async Task FlushAsync(CancellationToken cancellationToken = default)
        {
            var table = new TableData()
            {
                TableName = this._attribute.TableName,
                Rows = _rowsToInsert
            };

            await this._dbService.InsertTableDataAsync(table, cancellationToken);

            this._rowsToInsert.Clear();
        }
    }
}
