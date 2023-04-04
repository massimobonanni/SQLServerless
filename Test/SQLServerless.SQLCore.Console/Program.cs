using SQLServerless.Core.Entities;
using SQLServerless.Core.Interfaces;
using SQLServerless.SQLCore.Console;
using SQLServerless.SQLCore.Helpers;
using SQLServerless.SQLCore.Implementations;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IDBService, SQLService>();
        services.AddSingleton<IChangeTracker>(sp =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
            var changeTracker = new SQLChangeTracker();
            changeTracker.SetConfiguration(new ChangeTrackerConfiguration() { ConnectionString = connectionString });
            return changeTracker;
        });
        services.AddHostedService<Worker>();
    })
    .Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();
var connectionString = configuration.GetConnectionString("DefaultConnection");

TestGetInsertStatement();
TestGetInsertStatementTable();
await AddContact(connectionString);
await AddContactWithInsertStatement(connectionString);

await host.RunAsync();

async Task AddContact(string connectionString)
{
    var dbService = host.Services.GetRequiredService<IDBService>();

    dbService.SetConfiguration(new DBConfiguration() { ConnectionString = connectionString });

    var command = new Command("[dbo].[InsertContact]");
    command.Parameters.Add("firstName", "Massimo");
    command.Parameters.Add("lastName", "Bonanni");
    command.Parameters.Add("email", "massimo.bonanni@microsoft.com");

    await dbService.ExecuteCommandAsync(command, default);
}

async Task AddContactWithInsertStatement(string connectionString)
{
    var dbService = host.Services.GetRequiredService<IDBService>();

    dbService.SetConfiguration(new DBConfiguration() { ConnectionString = connectionString });

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

    await dbService.InsertTableDataAsync(table, default);
}

static void TestGetInsertStatement()
{
    var tablerow = new TableRowData();
    tablerow.Add("FirstName", "Massimo");
    tablerow.Add("LastName", "Bonanni");
    tablerow.Add("Email", "mabonann@microsoft.com");
    tablerow.Add("Height", 175);
    tablerow.Add("BirthDate", new DateTime(1970, 2, 26));

    var statement = QueryFactory.GetInsertStatement("dbo.Contacts", tablerow);
}

static void TestGetInsertStatementTable()
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