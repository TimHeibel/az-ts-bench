## Deploy Resource
The Bicep template is parameterized, and the parameters can be set via the CLI Input. Following Parameterizations were used to create SQL Databases of different scales:
```ps
az deployment group create --resource-group <ResourceGroupName> --template-file PostgreSQL.bicep --parameters skuName='GP_Gen5_2'
```  

## Configure Database
- In Azure Portal navigate to the Azure Database for PostgreSQL server and under `Connection Security` add the ClientIP Address and set
`Allow Azure services and resources to access this server` to `On`
- Connect to the database server via the Connection String found in the Portal and using the username (use the one from Azure Portal, Postgres adds '@server-postgres-tsdb-benchmark' to the one specified in the bicep file) and password specified in the Bicep File
    - It is recommended to use [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio?view=sql-server-ver15)
- In order to preload the TimescaleDB Extension, please follow these [steps](https://docs.microsoft.com/en-us/azure/postgresql/concepts-extensions#installing-timescaledb)

## Setup Table
- Depending on the case to benchmark, execute the SQL scripts on the database