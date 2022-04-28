# Deploy Resources
The Bicep template is parameterized, and the parameters can be set via the CLI Input. Following Parameterizations can be used to create SQL Databases of different scales:
```ps
az deployment group create --resource-group <ResourceGroupName> --template-file AzureSQLDB.bicep --parameters skuName='<skuName>' skuCapacity=<skuCapacity>
```

# Configure Database
- In Azure Portal navigate to the SQL Server and under "Firewalls and virtual networks" add the ClientIP Address and set "Allow Azure services and resources to access this server" to On
- Connect to DB Server via the Connection String found in the Portal and using the username and password specified in the bicep File
    - It is recommended to use [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)

## Setup Table
- Depending on the case to benchmark, execute the SQL scripts on the database