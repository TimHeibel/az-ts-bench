using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using MathNet.Numerics.Statistics;
using Kusto.Data;
using System.Threading;

namespace ADX_Queries
{
    public static class QueryADX
    {
        [FunctionName("QueryADX")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                #region Deserialize Request
                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                }
                Request request = JsonConvert.DeserializeObject<Request>(requestBody);
                #endregion

                #region Setup
                // Flush Query Plan Caches, to get a "Cold Start"
                string flushCacheString = "clear database cache query_results";

                // Get the Connection String and AAD Values for the Database
                string adxConnectionString = Environment.GetEnvironmentVariable("ADX_CONNECTION_STRING");
                string applicationClientId = Environment.GetEnvironmentVariable("APPLICATION_CLIENT_ID");
                string applicationKey = Environment.GetEnvironmentVariable("APPLICATION_KEY");
                string authority = Environment.GetEnvironmentVariable("APPLICATION_AUTHORITY");

                // Initialize Stopwatch Instance, that measures the duration
                Stopwatch stopwatch = new Stopwatch();

                List<int> rowsList = new List<int>();
                List<List<double>> queryDurationsList = new List<List<double>>();
                #endregion

                #region Get Kusto Query Client
                var kustoClient = Kusto.Data.Net.Client.KustoClientFactory.CreateCslQueryProvider(new KustoConnectionStringBuilder(
               adxConnectionString).WithAadApplicationKeyAuthentication(
                applicationClientId: applicationClientId,
                applicationKey: applicationKey,
                authority: authority));
                log.LogInformation("Created Client");
                #endregion

                #region Execute Benchmark
                for (int i = 0; i < request.queries.Length; i++)
                {
                    List<double> queryDurations = new List<double>();
                    log.LogInformation($"-----STARTING NEW QUERY----- \n QueryType: {request.queries[i].queryType} \n #Runs: {request.queries[i].numberOfRuns} \n Query: {request.queries[i].queryString}");

                    // As the client has not yet established a connection, the first query is significantly slower.This is not due to caching, as subsequent triggerings of the app (when the results would have been cached) also come with a slower first query. Furthermore the query results cache can be checked, via ".show database cache query_results", which is empty. Therefore the first run is not recorded
                    for (int j = -1; j < request.queries[i].numberOfRuns; j++)
                    {
                        log.LogInformation($"Run #{j + 1}");
                        int rows = 0;
                        stopwatch.Restart();
                        var reader = kustoClient.ExecuteQuery(request.queries[i].queryString);
                        while (reader.Read())
                        {
                            rows++;
                        }

                        stopwatch.Stop();
                        // skip first run in evaluation
                        if (j != -1)
                        {
                            queryDurations.Add(stopwatch.ElapsedMilliseconds);
                        }
                        else
                        {
                            rowsList.Add(rows);
                        }

                    }
                    queryDurationsList.Add(queryDurations);
                }
                #endregion

                #region Cleanup, Log Raw Values
                log.LogInformation("Closed Connection");

                for (int i = 0; i < queryDurationsList.Count; i++)
                {
                    log.LogInformation($"Query: {request.queries[i].queryString} Durations:{ String.Join(", ", queryDurationsList[i].ToArray())}");
                }
                #endregion

                #region Calculate Statistics, create Result Object
                List<Result> resultList = new List<Result>();
                for (int i = 0; i < request.queries.Length; i++)
                {
                    Result result = new Result()
                    {
                        QueryString = request.queries[i].queryString,
                        QueryType = request.queries[i].queryType,
                        Minimum = queryDurationsList[i].Minimum(),
                        Mean = Math.Round(queryDurationsList[i].Mean(), 4),
                        Median = queryDurationsList[i].Median(),
                        Maximum = queryDurationsList[i].Maximum(),
                        StandardDeviation = Math.Round(queryDurationsList[i].StandardDeviation(), 4),
                        NumberOfRowsReturned = rowsList[i],
                        DatabaseSize = request.databaseSize,
                        RowsInDataBase = request.rowsInDataBase,
                        EstimatedMonthlyCosts = request.estimatedMonthlyCosts,
                        NumberOfRuns = request.queries[i].numberOfRuns
                    };
                    resultList.Add(result);
                }
                #endregion

                #region Save to Blob Storage
                string storageAccountCredentials = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONNECTION_STRING");
                string containerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME");
                string appendBlobFileName = Environment.GetEnvironmentVariable("APPENDBLOB_FILE_NAME");

                // Connect to CloudStorageAccout
                CloudStorageAccount cloudStorageAccount;
                CloudStorageAccount.TryParse(storageAccountCredentials, out cloudStorageAccount);

                // Create CloudBlobClient
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                // Get Reference to Container "egress" 
                CloudBlobContainer blobContainer = cloudBlobClient.GetContainerReference(containerName);

                // Create Container if it does not exist
                if (!await blobContainer.ExistsAsync())
                {
                    log.LogWarning($"Container did not exist. Creating {blobContainer.Name}");
                    await blobContainer.CreateIfNotExistsAsync();
                    log.LogInformation($"Sucessfully created Blob Container");
                }

                // Get reference to Results File
                CloudAppendBlob cloudAppendBlob = blobContainer.GetAppendBlobReference(appendBlobFileName);

                // Use existing Blob File
                if (await cloudAppendBlob.ExistsAsync())
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultList) + Environment.NewLine)))
                    {
                        await cloudAppendBlob.AppendBlockAsync(ms);
                        log.LogInformation($"Sucessfully appended Input Message to Blob File");
                    }
                }

                // Create new Blob File if it does not exist
                else
                {
                    await cloudAppendBlob.CreateOrReplaceAsync();
                    log.LogInformation($"Sucessfully created new Blob File: {cloudAppendBlob.Name}");

                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultList) + Environment.NewLine + "")))
                    {
                        await cloudAppendBlob.AppendBlockAsync(ms);
                    }
                }
                #endregion

                return new OkObjectResult(resultList);
            }
            catch (Exception e)
            {
                log.LogInformation($"Caught Unexpected Exception: {e.Message} ");
                return new BadRequestObjectResult(e.Message);
            }
        }
    }

    #region Result Class
    public class Result
    {
        public string QueryString { get; set; }
        public string QueryType { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Median { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
        public int NumberOfRowsReturned { get; set; }
        public string DatabaseSize { get; set; }
        public int RowsInDataBase { get; set; }
        public double EstimatedMonthlyCosts { get; set; }
        public int NumberOfRuns { get; set; }
    }
    #endregion

    #region Request Class
    public class Request
    {
        public string databaseSize { get; set; }
        public int rowsInDataBase { get; set; }
        public double estimatedMonthlyCosts { get; set; }
        public Query[] queries { get; set; }
    }

    public class Query
    {
        public string queryString { get; set; }
        public string queryType { get; set; }
        public int numberOfRuns { get; set; }
    }
    #endregion
}
