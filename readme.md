# Run in your local environment 

To run this locally, create a file named local.settings.json in the root of the project. Set its contents to the following:

```json
{
  "IsEncrypted": false,
  "Values": {
    "HttpProcessorUrl": "http://localhost:7071/api/QueueWorkHttp",
    "BatchSize": 200,
    "MaxRecordCount": 200,
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "queueConn": "UseDevelopmentStorage=true",
    "GetWeatherQueueWriteToQueueDisabled": true,
   "GetWeatherQueueWriteToCosmosDbDisabled": false,
    "CosmosDbConn": "AccountEndpoint=https://[account].documents.azure.com:443/;AccountKey=[account-key]"
  }
}
```

Addtionally, in your local storage (“Development”), create two queues with the following names:
-	“weather-items” // this stores the api call results 
-	“work-items” // this stores individual, json formatted coordinates 

# Deploy to Azure

Note that you will not get the desired performance running locally. The project contains an ARM template to deploy to Azure. Use the WeatherFuncApp.ResourceGroup project to deploy from VS2017 (replace the parameters values or you will get exceptions).
The template creates the required app settings automatically.

At the time of writing, only zipped deployments would succeed (might be an issue limited to my local system.)
