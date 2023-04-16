using System;
using Azure;
using System.Net.Http;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Messaging.EventGrid;

namespace IotHubtoTwins
{
    public class IoTHubtoTwins
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("IoTHubtoTwins")]
#pragma warning disable AZF0001 // Avoid async void
        public async void Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
#pragma warning restore AZF0001 // Avoid async void
        {
            if (adtInstanceUrl == null) log.LogError("Application setting \"ADT_SERVICE_URL\" not set");

            try
            {
                var cred = new DefaultAzureCredential();
                var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), cred);
                log.LogInformation($"ADT service client connection created.");

                if (eventGridEvent != null && eventGridEvent.Data != null)
                {
                    log.LogInformation(eventGridEvent.Data.ToString());

                    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];

                    var body = (string)deviceMessage["body"];
                    var messageBytes = Convert.FromBase64String(body);
                    var messageString = System.Text.Encoding.UTF8.GetString(messageBytes);
                    var message = JsonConvert.DeserializeObject<JObject>(messageString);
                    var temperature = message["temperature"].Value<double>();
                    var humidity = message["humidity"].Value<double>();

                    log.LogInformation($"Device:{deviceId} Temperature is:{temperature}");
                    log.LogInformation($"Device:{deviceId} Humidity is:{humidity}");


                    var updateTwinData = new JsonPatchDocument();
                    updateTwinData.AppendReplace("/temperature", temperature);
                    updateTwinData.AppendReplace("/humidity", humidity);
                    
                    await client.UpdateDigitalTwinAsync(deviceId, updateTwinData);
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error in ingest function: {ex.Message}");
            }
        }
    }
}
