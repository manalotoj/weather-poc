using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace WeatherFunctionApp
{
    public static class HttpStart
    {
        /// <summary>
        /// Enables execution of orchestrations via Http
        /// </summary>
        /// <param name="req"></param>
        /// <param name="starter"></param>
        /// <param name="functionName">name of orchesration function to execute</param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("HttpStart")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "orchestrators/{functionName}")] HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClientBase starter,
            string functionName,
            ILogger log)
        {
            // Function input comes from the request content.
            dynamic eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await starter.StartNewAsync(functionName, eventData);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            var res = starter.CreateCheckStatusResponse(req, instanceId);
            res.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(10));
            return res;
        }
    }
}
