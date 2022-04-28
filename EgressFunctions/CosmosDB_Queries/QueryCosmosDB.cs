using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosDB_Queries
{
    public static class QueryCosmosDB
    {
        [FunctionName("QueryCosmosDB")]
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
                // Cosmos DB does not cache Results out of the box, so no cache flushing needed

                // Get the Connection String for the Database
                var SQLDBConnectionString = Environment.GetEnvironmentVariable("SQL_DB_CONNECTION_STRING");

                // Initialize Stopwatch Instance for measuring the duration
                Stopwatch stopwatch = new Stopwatch();


                List<int> rowsList = new List<int>();
                List<List<double>> queryDurationsList = new List<List<double>>();
                #endregion

                #region Create/Get Reference to Cosmos Container
                var cosmosUrl = Environment.GetEnvironmentVariable("COSMOS_URL");
                var cosmosKey = Environment.GetEnvironmentVariable("COSMOS_KEY");
                var databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME");

                List<(string, string)> containersToInitialize = new List<(string, string)> { (databaseName, "benchTable") };

                CosmosClient client = await CosmosClient.CreateAndInitializeAsync(cosmosUrl, cosmosKey, containersToInitialize);
                Database database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                Container container = await database.CreateContainerIfNotExistsAsync("benchTable", "/deviceId", 400);
                #endregion
                
                #region Run Benchmark
                // Cosmos DB does not cache results out of the box. Nonetheless the first query is significantly slower, as the
                // SDK has not yet cached information about the connection. Therefore the CreateAndInitialize Method can be used or
                // a quick dummy query can be run. Therefore the i = -1 run of the benchmark is not added to the list.
                for (int i = 0; i < request.queries.Length; i++)
                {
                    List<double> queryDurations = new List<double>();
                    log.LogInformation($"-----STARTING NEW QUERY----- \n QueryType: {request.queries[i].queryType} \n #Runs: {request.queries[i].numberOfRuns} \n Query: {request.queries[i].queryString}");

                    for (int j = -1; j < request.queries[i].numberOfRuns; j++)
                    {
                        log.LogInformation($"Run #{j + 1}");
                        int rows = 0;
                        stopwatch.Restart();

                        using (FeedIterator<dynamic> feedIterator = container.GetItemQueryIterator<dynamic>(request.queries[i].queryString))
                        {
                            while (feedIterator.HasMoreResults)
                            {
                                FeedResponse<dynamic> response = await feedIterator.ReadNextAsync();
                                foreach (var item in response)
                                {
                                    rows++;
                                }
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
                    }
                        queryDurationsList.Add(queryDurations);
                }
                #endregion

                #region Cleanup, Log Raw Values

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
