using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SQLServerless.Extensions.Triggers
{

    public class SQLTriggerListener : IListener
    {

        private readonly ITriggeredFunctionExecutor _executor;
        private CancellationTokenSource _listenerStoppingTokenSource;

        private readonly IChangeTracker _changeTracker;
        private readonly SQLTriggerAttribute _attribute;

        private Task _listenerTask;

        public SQLTriggerListener(ITriggeredFunctionExecutor executor,
            IChangeTracker changeTracker, SQLTriggerAttribute attribute)
        {
            this._executor = executor;
            this._changeTracker = changeTracker;
            this._attribute = attribute;
        }

        public void Cancel()
        {
            StopAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _listenerStoppingTokenSource = new CancellationTokenSource();
                var factory = new TaskFactory();
                var token = _listenerStoppingTokenSource.Token;
                _listenerTask = factory.StartNew(async () => await ListenerAction(token), token);
            }
            catch (Exception)
            {
                throw;
            }

            return _listenerTask.IsCompleted ? _listenerTask : Task.CompletedTask;
        }

        private async Task ListenerAction(CancellationToken token)
        {
            this._changeTracker.SetConfiguration(new ChangeTrackerConfiguration()
            {
                ConnectionString = this._attribute.ConnectionString
            });

            TableData tableChanges = null;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    tableChanges = await this._changeTracker.GetChangesAsync(this._attribute.TableName, this._attribute.KeyName);
                }
                catch (Exception ex)
                {
                    tableChanges = null;
                }

                if (tableChanges != null && tableChanges.Rows.Any())
                {
                    await _executor.TryExecuteAsync(new TriggeredFunctionData()
                    {
                        TriggerValue = tableChanges
                    }, token);
                }

                await Task.Delay(TimeSpan.FromSeconds(this._attribute.PollingInSeconds), token);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_listenerTask == null)
                return;

            try
            {
                _listenerStoppingTokenSource.Cancel();
            }
            finally
            {
                await Task.WhenAny(_listenerTask, Task.Delay(Timeout.Infinite, cancellationToken));

            }
        }
    }
}
