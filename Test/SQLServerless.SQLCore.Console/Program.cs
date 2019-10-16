using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using SQLServerless.SQLCore.Implementations;

namespace SQLServerless.SQLCore.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.local.json", true, true)
                .Build();


            var connectionString = config.GetConnectionString("DefaultConnection");

            var changeTracker = new SQLChangeTracker();
            changeTracker.SetConfiguration(new Core.Entities.ChangeTrackerConfiguration() { ConnectionString = connectionString });

            do
            {
                while (!System.Console.KeyAvailable)
                {
                    var changes = changeTracker.GetChangesAsync("dbo.Contacts", "Id").GetAwaiter().GetResult();
                    if (changes != null && changes.Rows.Any())
                    {
                        foreach (var item in changes.Rows)
                        {
                            foreach (var row in item)
                            {
                                System.Console.Write($"{row}; ");
                            }
                            System.Console.WriteLine();
                        }
                    }
                    Thread.Sleep(1000);
                }
            } while (System.Console.ReadKey(true).Key != ConsoleKey.Escape);

        }
    }
}
