using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SQLServerless.Extensions.Bindings;
using SQLServerless.Core.Entities;
using SQLServerless.Functions.DTO;
using SQLServerless.Functions.Extensions;
using SQLServerless.Core.Interfaces;
using System.Threading;

namespace SQLServerless.Functions
{
    public class DIFunctions
    {
        private readonly IDBService dbService;

        public DIFunctions(IDBService dbService)
        {
            if (dbService == null)
                throw new ArgumentNullException(nameof(dbService));

            this.dbService = dbService;

            var connectionString=Environment.GetEnvironmentVariable("SQLBinding.Connectionstring", EnvironmentVariableTarget.Process);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Connectionstring  must be set either via the attribute property or via configuration.");
            this.dbService.SetConfiguration(new DBConfiguration() { ConnectionString = connectionString });

        }

        [FunctionName(nameof(InsertWithCustomService))]
        public async Task<IActionResult> InsertWithCustomService(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(InsertWithCustomService)} HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var contact = JsonConvert.DeserializeObject<ContactDTO>(requestBody);
            log.LogInformation($"Contact: [{contact}] received");

            await this.dbService.InsertTableDataAsync(contact.ToTableData("dbo.Contacts"), default(CancellationToken));

            return (ActionResult)new OkObjectResult($"Contact: [{contact}] inserted");
        }

    }
}
