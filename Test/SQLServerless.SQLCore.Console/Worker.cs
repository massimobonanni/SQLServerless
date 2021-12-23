using SQLServerless.Core.Interfaces;

namespace SQLServerless.SQLCore.Console
{
    public class Worker : BackgroundService
    {
        private readonly IChangeTracker _changeTracker;
        private readonly ILogger<Worker> _logger;

        public Worker(IChangeTracker changeTracker, ILogger<Worker> logger)
        {
            _changeTracker = changeTracker;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            const string tableName = "dbo.Contacts";
            const string key = "Id";

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("looking for changes in {tableName} based on primary key {primaryKey}", tableName, key);
                var changes = await _changeTracker.GetChangesAsync(tableName, key, stoppingToken);
                if (changes != null && changes.Rows.Any())
                {
                    foreach (var item in changes.Rows)
                        _logger.LogInformation("processed {@item}", item);
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}