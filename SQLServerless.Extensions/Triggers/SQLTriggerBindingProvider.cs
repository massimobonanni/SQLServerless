using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Microsoft.Extensions.Logging;
using SQLServerless.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerless.Extensions.Triggers
{

    public class SQLTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly INameResolver _nameResolver;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IChangeTracker _changeTracker;

        public SQLTriggerBindingProvider(INameResolver nameResolver,
            ILoggerFactory loggerFactory, IChangeTracker changeTracker)
        {
            this._nameResolver = nameResolver;
            this._loggerFactory = loggerFactory;
            this._changeTracker = changeTracker;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var parameter = context.Parameter;

            var triggerAttribute = parameter.GetCustomAttribute<SQLTriggerAttribute>(inherit: false);
            if (triggerAttribute is null)
                return Task.FromResult<ITriggerBinding>(null);

            triggerAttribute.ConnectionString = GetTriggerAttributeConnectionString(triggerAttribute);
            triggerAttribute.PollingInSeconds = GetTriggerAttributePollingInSecs(triggerAttribute);

            return Task.FromResult<ITriggerBinding>(
                new SQLTriggerBinding(parameter, _nameResolver, _changeTracker, triggerAttribute));
        }

        private string GetTriggerAttributeConnectionString(SQLTriggerAttribute triggerAttribute)
        {
            if (string.IsNullOrEmpty(triggerAttribute.ConnectionString))
            {
                var connectionString = _nameResolver.Resolve("SQLTrigger.ConnectionString");

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("ConnectionString is mandatory");

                return connectionString;
            }

            return triggerAttribute.ConnectionString;
        }

        private int GetTriggerAttributePollingInSecs(SQLTriggerAttribute triggerAttribute)
        {
            var pollingInSecsString = _nameResolver.Resolve("SQLTrigger.PollingInSecs");
            int pollingInSecs = 0;
            if (int.TryParse(pollingInSecsString, out pollingInSecs))
            {
                return pollingInSecs;
            }

            return triggerAttribute.PollingInSeconds;
        }
    }
}
