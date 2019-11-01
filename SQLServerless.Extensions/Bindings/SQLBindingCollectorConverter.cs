using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;
using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;

namespace SQLServerless.Extensions.Bindings
{

    public class SQLBindingCollectorConverter : IConverter<SQLBindingAttribute, IAsyncCollector<TableRowData>>
    {
        private readonly INameResolver _nameResolver;
        private readonly IDBService _dbService;

        public SQLBindingCollectorConverter(INameResolver nameResolver, IDBService dbService)
        {
            if (nameResolver == null)
                throw new ArgumentNullException(nameof(nameResolver));
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));

            _nameResolver = nameResolver;
            _dbService = dbService;
        }

        public IAsyncCollector<TableRowData> Convert(SQLBindingAttribute attribute)
        {
            return new SQLBindingAsyncCollector(attribute, _dbService, _nameResolver);
        }
    }
}
