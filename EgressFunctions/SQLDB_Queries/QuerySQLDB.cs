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

namespace SQLDB_Queries
{
    public static class QuerySQLDB
    {
        [FunctionName("QuerySQLDB")]
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
                string flushCachesString = "DBCC FREEPROCCACHE; DBCC DROPCLEANBUFFERS";

                // Get the Connection String for the Database
                var SQLDBConnectionString = Environment.GetEnvironmentVariable("SQL_DB_CONNECTION_STRING");

                // Initialize Stopwatch Instance for measuring the duration
                Stopwatch stopwatch = new Stopwatch();
                int numberOfRows = 0;
                #endregion

                #region Connect to Database
                SqlConnection connection = new SqlConnection(SQLDBConnectionString);
                await connection.OpenAsync();
                log.LogInformation("Connected to Datbase");
                // Build the SQL Insert Statement
                SqlCommand flushCachesCommand = new SqlCommand(flushCachesString, connection);
                #endregion

                #region Execute Benchmark
                List<List<double>> queryDurationsList = new List<List<double>>();

                // Loop over queries to be made
                for (int i = 0; i < request.queries.Length; i++)
                {
                    List<double> queryDurations = new List<double>();
                    log.LogInformation($"-----STARTING NEW QUERY----- \n QueryType: {request.queries[i].queryType} \n #Runs: {request.queries[i].numberOfRuns} \n Query: {request.queries[i].queryString}");

                    using (SqlCommand command = new SqlCommand(request.queries[i].queryString, connection))
                        for (int j = 0; j < request.queries[i].numberOfRuns; j++)
                        {
                            command.CommandTimeout = 120;
                            log.LogInformation($"Run #{j + 1}");
                            flushCachesCommand.ExecuteNonQuery();
                            {
                                stopwatch.Restart();
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    int rows = 0;
                                    while (reader.Read())
                                    {
                                        rows++;
                                    }
                                    stopwatch.Stop();
                                    numberOfRows = rows;
                                }
                                queryDurations.Add(stopwatch.ElapsedMilliseconds);
                            }
                        }
                    queryDurationsList.Add(queryDurations);
                }

                await connection.CloseAsync();
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
                        NumberOfRowsReturned = numberOfRows,
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

