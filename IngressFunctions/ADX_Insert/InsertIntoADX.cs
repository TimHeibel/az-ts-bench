using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Kusto.Data;
using Kusto.Data.Net.Client;
using System.IO;
using Kusto.Ingest;
using System;

namespace ADXInsert
{
    public static class InsertIntoADX
    {

        [FunctionName("InsertIntoADX")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IOT_HUB_CONNECTION_STRING")] EventData message, ILogger log)
        {
            // Convert EventData message into Stream
            MemoryStream messageStream = new MemoryStream(message.Body.Array);

            // Create Client for Ingestion
            string applicationClientId = Environment.GetEnvironmentVariable("APPLICATION_CLIENT_ID");
            string applicationKey = Environment.GetEnvironmentVariable("APPLICATION_KEY");
            string authority = Environment.GetEnvironmentVariable("APPLICATION_AUTHORITY");

            IKustoIngestClient kustoClient = KustoIngestFactory.CreateStreamingIngestClient(new KustoConnectionStringBuilder(
                @"https://clusteradx.westeurope.kusto.windows.net").WithAadApplicationKeyAuthentication(
                 applicationClientId: applicationClientId,
                 applicationKey: applicationKey,
        authority: authority));

            var kustoIngestionProperties = new KustoIngestionProperties(databaseName: "tsdb_bench", tableName: "benchTable")
            {
                Format = Kusto.Data.Common.DataSourceFormat.json,
                IngestionMapping = new IngestionMapping()
                {
                    IngestionMappingReference = "StandardMapping"
                }
            };
            kustoClient.IngestFromStreamAsync(messageStream, kustoIngestionProperties);
        }
    }
}