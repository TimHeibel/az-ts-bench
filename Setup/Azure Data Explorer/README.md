# Deploy Resources
The Bicep template is parameterized, and the parameters can be set via the CLI Input. The templates contain default values for the name, location and SKU of the resources. It is possible to set them 
```ps
az deployment group create --resource-group <ResourceGroupName> --template-file ADX.bicep 
```



# Setup
After the Resource has been created, a table needs to be created. Notice, that the column 'time' was renamed to 'timestamp', as 'time' is a reserved keyword
```
.create table benchTable (timestamp: datetime,value: real,deviceId: string)
```

Then a JSON Mapping needs to be added. This allows the mapping from the keys in the IoT Hub Message to the  Columns of the Table:
```
.create table benchTable ingestion json mapping 'StandardMapping' '[{"Column": "timestamp", "Properties" : {"Path" : "$.time"}},{ "Column": "value", "Properties" : { "Path" : "$.value"} },{ "Column": "deviceId", "Properties" : { "Path" : "$.deviceId"} }]'
```


Furthermore in order to connect to ADX programatically, a way of authentication needs to be established.
One way of doing so is to create a new app registration. Please follow the steps linked [here](https://docs.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad#--option-1-create-a-new-app-registration-automatically).

Afterwards the registered app needs to be given permission on the database of the resource. It is recommended to give ```User Permissions```.
Please follow the steps linked [here](https://docs.microsoft.com/en-us/azure/data-explorer/manage-database-permissions#manage-permissions-in-the-azure-portal).
