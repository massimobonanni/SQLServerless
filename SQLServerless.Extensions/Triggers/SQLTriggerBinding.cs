using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerless.Extensions.Triggers
{
    public class SQLTriggerBinding : ITriggerBinding
    {
        public Type TriggerValueType => typeof(TableData);

        public IReadOnlyDictionary<string, Type> BindingDataContract { get; } = new Dictionary<string, Type>();

        private readonly SQLTriggerAttribute _attribute;
        private readonly ParameterInfo _parameter;
        private readonly INameResolver _nameResolver;
        private readonly IChangeTracker _changeTracker;

        private readonly Task<ITriggerData> _emptyBindingDataTask =
            Task.FromResult<ITriggerData>(new TriggerData(null, new Dictionary<string, object>()));

        public SQLTriggerBinding(ParameterInfo parameter, INameResolver nameResolver,
            IChangeTracker changeTracker, SQLTriggerAttribute attribute)
        {
            this._parameter = parameter;
            this._nameResolver = nameResolver;
            this._attribute = attribute;
            this._changeTracker = changeTracker;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            return _emptyBindingDataTask;
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return Task.FromResult<IListener>(new SQLTriggerListener(context.Executor,
                this._changeTracker, this._attribute));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new SQLTriggerParameterDescriptor()
            {
                Name = _parameter.Name,
                Type = "SQLTrigger",
                TableName = _attribute.TableName,
                KeyName = _attribute.KeyName
            };
        }
    }
}
