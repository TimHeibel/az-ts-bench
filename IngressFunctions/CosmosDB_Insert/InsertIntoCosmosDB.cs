using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.Cosmos;

namespace CosmosDBInsert
{
    public static class InsertIntoCosmosDB
    {
        

        [FunctionName("InsertIntoCosmosDB")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IOT_HUB_CONNECTION_STRING", ConsumerGroup = "%IOT_HUB_CONSUMER_GROUP%")] EventData message,
            [CosmosDB(
                databaseName: "database-benchmark",
                collectionName: "benchTable",
                CreateIfNotExists =true,
            CollectionThroughput =6000,
                PartitionKey = "/deviceId",
                ConnectionStringSetting = "CosmosDBConnection")]out dynamic document,

                
            ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message");

            var jsonMessage = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation(jsonMessage);
            // Deserialize message
            dynamic data = JsonConvert.DeserializeObject(jsonMessage);

            #region Create/Get Reference to Cosmos Container
            #endregion

            // Create Document to be inserted
            document = new { id = Guid.NewGuid(), time = data.time.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), value = data.value, deviceId = data.deviceId };

            // Insert Document into DB
            //await container.CreateItemAsync(document);
            log.LogInformation("Created Document");
        }
    }
}
