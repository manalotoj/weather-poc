using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherFunctionApp.Functions
{
    public static class WeatherQueueProcessor
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

        [FunctionName(nameof(GetWeatherQueueWriteToCosmosDb))]
        [Disable("GetWeatherQueueWriteToCosmosDbDisabled")]
        public static async Task GetWeatherQueueWriteToCosmosDb(
            [QueueTrigger("work-items", Connection = "queueConn")]string myQueueItem,
            [CosmosDB(
                    databaseName: "default",
                    collectionName: "weather",
                    ConnectionStringSetting = "cosmosDbConn")]IAsyncCollector<string> output,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            Coordinates coords = JsonConvert.DeserializeObject<Coordinates>(myQueueItem);
            string result = await GetWeatherForLine(coords, log);
            if (result != null)
            {
                await output.AddAsync(result);
            }
        }



        [FunctionName(nameof(GetWeatherQueueWriteToQueue))]
        [Disable("GetWeatherQueueWriteToQueueDisabled")]
        public static async Task GetWeatherQueueWriteToQueue(
            [QueueTrigger("work-items", Connection = "queueConn")]string myQueueItem,
            [Queue("weather-items", Connection = "queueConn")]IAsyncCollector<string> outputQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            Coordinates coords = JsonConvert.DeserializeObject<Coordinates>(myQueueItem);
            string result = await GetWeatherForLine(coords, log);
            if (result != null)
            {
                await outputQueueItem.AddAsync(result);
            }
        }

        public static async Task<string> GetWeatherForLine(Coordinates coords, ILogger log)
        {
            string requestUrl = $"https://api.weather.com/v1/geocode/{coords.Latitude}/{coords.Longitude}/forecast/fifteenminute.json?language=en-US&units=e&apiKey=0425fb35c24e4066a5fb35c24e5066ab";
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage())
            {

                request.RequestUri = new Uri(requestUrl);
                request.Method = HttpMethod.Get;
                request.Headers.Add("ContentType", "application/json");

                response = await HttpClient.SendAsync(request);
            }
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return json;
            }
            else
            {
                return null;
            }

        }
    }
}
