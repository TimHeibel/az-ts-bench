using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsertIntoSQLDB
{
    public static class InsertIntoSQLDB
    {
        [FunctionName("InsertIntoSQLDB")]
        public static async Task RunAsync([IoTHubTrigger("messages/events", Connection = "IOT_HUB_CONNECTION_STRING", ConsumerGroup = "%IOT_HUB_CONSUMER_GROUP%")] EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message");

            var jsonMessage = Encoding.UTF8.GetString(message.Body.Array);
            // Deserialize message
            dynamic data = JsonConvert.DeserializeObject(jsonMessage);

            string date = data.time.ToString().Remove(data.time.ToString().Length - 3);
            log.LogInformation(date);
            // Get the connection string from app settings and use it to create a connection.
            var sqlDbConnectionString = Environment.GetEnvironmentVariable("SQL_DB_CONNECTION_STRING");
            using (SqlConnection connection = new SqlConnection(sqlDbConnectionString))
            {
                connection.Open();
                // Build the SQL Insert Statement
                string a = "abd";
                string insertStatement = $"INSERT INTO benchTable VALUES ('{date}', '{data.value}', '{data.deviceId}')";
                using (SqlCommand cmd = new SqlCommand(insertStatement, connection))
                {
                    // Execute the command 
                    var rows = await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}