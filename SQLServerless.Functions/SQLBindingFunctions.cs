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

namespace SQLServerless.Functions
{
    public class SQLBindingFunctions
    {
        [FunctionName(nameof(InsertWithBinding))]
        public async Task<IActionResult> InsertWithBinding(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [SQLBinding(TableName = "dbo.Contacts")] IAsyncCollector<TableRowData> sqlRows,
            ILogger log)
        {
            log.LogInformation($"{nameof(InsertWithBinding)} HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var contact = JsonConvert.DeserializeObject<ContactDTO>(requestBody);
            log.LogInformation($"Contact: [{contact}] received");

            await sqlRows.AddAsync(contact.ToRowData());

            return (ActionResult)new OkObjectResult($"Contact: [{contact}] inserted");
        }

    }
}
