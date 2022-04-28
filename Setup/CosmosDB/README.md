## Deploy Resource
The Bicep template is parameterized, and the parameters can be set via the CLI Input. Following Parameterizations were used to create SQL Databases of different scales:
```ps
az deployment group create --resource-group <ResourceGroupName> --template-file CosmosSQLDB.bicep --parameters name='<ResourceName>'
```  

## 

```ps
az cosmosdb sql database create --account-name sql-cosmos-tsdb-benchmark --name database-benchmark --resource-group BA_Tim_neu
```

```ps
az cosmosdb sql container create --account-name sql-cosmos-tsdb-benchmark --database-name database-benchmark --name benchTable --partition-key-path "//deviceId" --resource-group BA_Tim_neu --throughput 2000
```



database-benchmark