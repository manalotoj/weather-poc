using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherFunctionApp
{
    public static class GetWeatherForecast
    {
        #region HttpClient Reuse
        public static HttpClient HttpClient => lazyClient.Value;
        private static Lazy<HttpClient> lazyClient = new Lazy<HttpClient>(InitializeHttpClient);

        private static HttpClient InitializeHttpClient()
        {
            return new HttpClient()
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }
        #endregion

        #region Debatching

        /// <summary>
        /// Debatches csv file of 500K coordinates
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(DebatchCoordinatesFile))]
        public static async Task DebatchCoordinatesFile([OrchestrationTrigger]DurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation("Starting DebatchCoordinatesFile");

            List<List<string>> groupsOfLines = await context.CallActivityAsync<List<List<string>>>(nameof(GetCoordinates), null);

            log.LogInformation("retreived coordinates file as Stream");

            List<Task> parallelTasks = new List<Task>();
            foreach (var group in groupsOfLines)
            {
                parallelTasks.Add(context.CallActivityAsync(nameof(ProcessGroupOfLines), group));
            }
            await Task.WhenAll(parallelTasks);

            log.LogInformation("All done");
        }

        [FunctionName(nameof(GetCoordinates))]
        public static async Task<List<List<string>>> GetCoordinates([ActivityTrigger] DurableActivityContext context, ILogger log)
        {
            List<List<string>> groupsOfLines = await BatchHelper.GetCoordinates(int.Parse(Config.BatchSize));
            return groupsOfLines;
        }

        [FunctionName(nameof(ProcessGroupOfLines))]
        public static async Task ProcessGroupOfLines([ActivityTrigger] DurableActivityContext context, ILogger log)
        {
            List<string> group = context.GetInput<List<string>>();

            string requestUrl = Config.HttpProcessorUrl;
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage())
            {

                request.RequestUri = new Uri(requestUrl);
                request.Method = HttpMethod.Post;
                request.Headers.Add("ContentType", "application/json");
                request.Content = new StringContent(JsonConvert.SerializeObject(group));

                response = await HttpClient.SendAsync(request);
            }
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation("group sent to azure function");
            }
        }

        #endregion Debatching
    }
}