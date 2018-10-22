using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WeatherFunctionApp.Functions
{
    public static class QueueWorkHttp
    {
        [FunctionName("QueueWorkHttp")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Queue("work-items", Connection = "queueConn")]IAsyncCollector<string> queueItems,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            List<string> lines = JsonConvert.DeserializeObject<List<string>>(await req.ReadAsStringAsync());

            int count = 0;
            foreach (var line in lines)
            {
                count += 1;
                string[] coords = line.Split(',');
                string longitude = coords[0];
                string latitude = coords[1];
                await queueItems.AddAsync(JsonConvert.SerializeObject(new Coordinates(latitude, longitude)));
            }

            return new OkObjectResult($"{count} lines added to queue.");
        }
    }
}
