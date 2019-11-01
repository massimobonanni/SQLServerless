using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using SQLServerless.Core.Interfaces;

namespace SQLServerless.Extensions.Bindings
{
    [Extension("SQLExtension")]
    public class SQLBindingConfigProvider : IExtensionConfigProvider
    {
        private readonly INameResolver _nameResolver;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDBService _dbService;

        public SQLBindingConfigProvider(INameResolver nameResolver,
            ILoggerFactory loggerFactory, IDBService dbService)
        {
            if (nameResolver == null)
                throw new ArgumentNullException(nameof(nameResolver));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));

            this._nameResolver = nameResolver;
            this._loggerFactory = loggerFactory;
            this._dbService = dbService;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var bindingRule = context.AddBindingRule<SQLBindingAttribute>();
            bindingRule.AddValidator(ValidateConfig);
            bindingRule.BindToCollector<OpenType>(typeof(SQLBindingCollectorConverter), _nameResolver, _dbService);
            bindingRule.BindToInput<SQLBinder>(typeof(SQLBindingConverter), _nameResolver, _dbService);
        }


        private void ValidateConfig(SQLBindingAttribute attribute, Type paramType)
        {
            if (string.IsNullOrEmpty(attribute.ConnectionString))
                throw new InvalidOperationException($"Connectionstring  must be set either via the attribute property or via configuration.");
        }
    }
}
