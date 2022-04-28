# General
This folder contains the source code for the Azure Functions that query the databases and store the results.

# Setup
It is recommended to deploy the Azure Functions via Visual Studio. For hosting, any Plan is suitable, as there are no high demands neither on CPU nor Storage. 
During the deployment it might be necessary to create a storage account. This does not change anything performance wise. Therefore it doesn't matter what is chosen.
Use [this guide](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs?tabs=in-process#publish-to-azure) to deploy the resource. 
After the Function App has been deployed, it is necessary to set the Environment Variables for the code which are equal to the Application Settings for the Function App. 
[This guide](https://docs.microsoft.com/en-us/azure/azure-functions/functions-premium-plan?tabs=portal#available-instance-skus) can be used to set the needed parameters.
Following Parameters can be set:
- "APPLICATION_CLIENT_ID"
- "APPLICATION_KEY"
- "APPLICATION_AUTHORITY",
- "STORAGE_ACCOUNT_CONNECTION_STRING"
- "BLOB_CONTAINER_NAME"
- "APPENDBLOB_FILE_NAME"
- "ADX_CONNECTION_STRING"
- "COSMOS_DATABASE_NAME"
- "COSMOS_KEY"
- "COSMOS_URL"
- "POSTGRESQL_DB_CONNECTION_STRING"
- "SQL_DB_CONNECTION_STRING"


# Benchmarking
After the Function has been set up, the benchmarking can begin. Therefore queries need to be created, which can be scripted but was done not fully automated for exemplary purposes. After the queries have been created, the HTTP requests can be composed.
A request may look as follows: 
```json
{
    "databaseSize ": "GP_Gen5_2",
    "rowsInDataBase ": 100000000,
    "estimatedMonthlyCosts ": 339.15,
    "queries ": [
        {
            "queryType ": "Point",
            "queryString ": "SELECT * FROM benchTable WHERE deviceId= 'sim000773' AND time = '2022-03-07 19:38:46.460'",
            "numberOfRuns ": 3
        },
        {
            "queryType ": "Point",
            "queryString ": "SELECT * FROM benchTable WHERE deviceId='sim002917' AND time = '2022-03-07 19:38:46.477 '",
            "numberOfRuns ": 3
        }
    ]
}
 ```