using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using SQLServerless.SQLCore.Helpers;
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

            TestGetInsertStatementTable();
            //AddContact(connectionString);

            var changeTracker = new SQLChangeTracker();
            changeTracker.SetConfiguration(new Core.Entities.ChangeTrackerConfiguration() { ConnectionString = connectionString });

            do
            {
                while (!System.Console.KeyAvailable)
                {
                    var changes = changeTracker.GetChangesAsync("dbo.Contacts", "Id", default(CancellationToken)).GetAwaiter().GetResult();
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

        private static void AddContact(string connectionString)
        {
            IDBService dbService = new SQLService();
            dbService.SetConfiguration(new Core.Entities.DBConfiguration() { ConnectionString = connectionString });

            var command = new Command("[dbo].[InsertContact]");
            command.Parameters.Add("firstName", "Massimo");
            command.Parameters.Add("lastName", "Bonanni");
            command.Parameters.Add("email", "massimo.bonanni@microsoft.com");

            dbService.ExecuteCommandAsync(command, default(CancellationToken)).GetAwaiter().GetResult();
        }

        private static void TestGetInsertStatement()
        {
            var tablerow = new TableRowData();
            tablerow.Add("FirstName", "Massimo");
            tablerow.Add("LastName", "Bonanni");
            tablerow.Add("Email", "mabonann@microsoft.com");
            tablerow.Add("Height", 175);
            tablerow.Add("BirthDate", new DateTime(1970,2,26));

            var statement = QueryFactory.GetInsertStatement("dbo.Contacts", tablerow);

        }

        private static void TestGetInsertStatementTable()
        {
            var table = new TableData() { TableName = "dbo.Contacts" };

            var tablerow = new TableRowData();
            tablerow.Add("FirstName", "Mario");
            tablerow.Add("LastName", "Rossi");
            tablerow.Add("Email", "marossi@microsoft.com");
            tablerow.Add("Height", 200);
            tablerow.Add("BirthDate", new DateTime(1980, 2, 26));
            table.Rows.Add(tablerow);

            tablerow = new TableRowData();
            tablerow.Add("FirstName", "Luigi");
            tablerow.Add("LastName", "Bianchi");
            tablerow.Add("Email", "lubianchi@microsoft.com");
            tablerow.Add("Height", 180);
            tablerow.Add("BirthDate", new DateTime(1981, 2, 26));
            table.Rows.Add(tablerow);

            var statement = QueryFactory.GetInsertStatement(table);

        }
    }
}
