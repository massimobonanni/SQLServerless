using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using SQLServerless.Core.Interfaces;
using System.Threading;
using SQLServerless.Core.Entities;

namespace SQLServerless.Extensions.Bindings
{
    public class SQLBinder
    {
        private readonly SQLBindingAttribute _attribute;
        private readonly IDBService _dbService;
        private readonly INameResolver _nameResolver;

        public SQLBinder(SQLBindingAttribute attribute,
            IDBService dbService, INameResolver nameResolver)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));
            if (nameResolver == null)
                throw new ArgumentNullException(nameof(nameResolver));

            this._attribute = attribute;
            this._dbService = dbService;
            this._nameResolver = nameResolver;

            this._dbService.SetConfiguration(new Core.Entities.DBConfiguration()
            {
                ConnectionString = attribute.ConnectionString
            });
        }

        public Task InsertAsync(TableData table,CancellationToken cancellationToken)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            return _dbService.InsertTableDataAsync(table,cancellationToken);
        }
    }
}
