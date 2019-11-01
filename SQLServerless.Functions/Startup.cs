using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SQLServerless.Core.Interfaces;
using SQLServerless.Functions;
using SQLServerless.SQLCore.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(Startup))]
public class Startup : IWebJobsStartup
{
    public void Configure(IWebJobsBuilder builder)
    {
        builder.UseSQLTrigger();
        builder.UseSQLBinding();

        builder.Services.AddTransient<IChangeTracker, SQLChangeTracker>();
        builder.Services.AddTransient<IDBService, SQLService>();

        ServiceLocator.DefaultProvider = builder.Services.BuildServiceProvider();
    }
}

