using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using System;

namespace TimescaleDBInsert
{
    public static class InsertIntoTimescaleDB
    {
        [FunctionName("InsertIntoTimescaleDB")]
        public static async Task RunAsync([IoTHubTrigger("messages/events", Connection = "IOT_HUB_CONNECTION_STRING", ConsumerGroup = "%IOT_HUB_CONSUMER_GROUP%")] EventData message, ILogger log)
        {
            var jsonMessage = Encoding.UTF8.GetString(message.Body.Array);

            // Deserialize message
            dynamic data = JsonConvert.DeserializeObject(jsonMessage);
            string postgresConnectionString = Environment.GetEnvironmentVariable("POSTGRESQL_DB_CONNECTION_STRING");

            // Get the connection string from app settings and use it to create a connection.
            await using var connection = new NpgsqlConnection(postgresConnectionString);
            await connection.OpenAsync();

            // Build the SQL Insert Statement
            string insertStatement = $"INSERT INTO benchTable VALUES ('{data.time.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}', '{data.value}', '{data.deviceId}')";

            // Insert some data
            await using (var cmd = new NpgsqlCommand(insertStatement, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            await connection.CloseAsync();
        }
    }
}