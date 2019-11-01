using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;
using SQLServerless.Core.Interfaces;

namespace SQLServerless.Extensions.Bindings
{
    public class SQLBindingConverter : IConverter<SQLBindingAttribute, SQLBinder>
    {
        private readonly INameResolver _nameResolver;
        private readonly IDBService _dbService;

        public SQLBindingConverter(INameResolver nameResolver, IDBService dbService)
        {
            if (nameResolver == null)
                throw new ArgumentNullException(nameof(nameResolver));
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));

            _nameResolver = nameResolver;
            _dbService = dbService;
        }

        public SQLBinder Convert(SQLBindingAttribute attribute)
        {
            return new SQLBinder(attribute, _dbService, _nameResolver);
        }
    }
}
