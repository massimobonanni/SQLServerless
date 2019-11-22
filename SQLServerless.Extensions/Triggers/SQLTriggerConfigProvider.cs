using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Extensions.Triggers
{

    [Extension("SQLTracker")]
    public class SQLTriggerConfigProvider : IExtensionConfigProvider
    {
        private readonly INameResolver _nameResolver;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IChangeTracker _changeTracker;

        public SQLTriggerConfigProvider(INameResolver nameResolver,
            ILoggerFactory loggerFactory, IChangeTracker changeTracker)
        {
            if (nameResolver == null)
                throw new ArgumentNullException(nameof(nameResolver));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            if (changeTracker == null)
                throw new ArgumentNullException(nameof(changeTracker));

            this._nameResolver = nameResolver;
            this._loggerFactory = loggerFactory;
            this._changeTracker = changeTracker;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            var triggerAttributeBindingRule = context.AddBindingRule<SQLTriggerAttribute>();
            triggerAttributeBindingRule.BindToTrigger<TableData>(
                new SQLTriggerBindingProvider(this._nameResolver, this._loggerFactory, this._changeTracker));

        }
    }
}
