# Deploy Resource
The Bicep template is parameterized, and the parameters can be set via the CLI Input. Following Parameterizations were used to create IoT Hubs of different scales:  
Standard Tier 1 (S1): 400,000 messages/day; Message Meter Size 4KB  

 ```bash
 az deployment group create --resource-group <ResourceGroupName> --template-file IoTHub.bicep --parameters skuName='S1' skuCapacity=1
 ```  

Basic Tier 3 (B3): messages/day/unit: 300,000,000; Message Meter Size 4KB; 6000 send operations/unit/sec  

```bash
az deployment group create --resource-group <ResourceGroupName> --template-file IoTHub.bicep --parameters skuName='B3' skuCapacity=4
```

## Creating Consumer Groups
After the Resource has been created, consumer groups need to be added.
```bash
az iot hub consumer-group create --hub-name <IotHubName> --name adx-insert
az iot hub consumer-group create --hub-name <IotHubName> --name cosmos-insert
az iot hub consumer-group create --hub-name <IotHubName> --name sql-insert
az iot hub consumer-group create --hub-name <IotHubName> --name timescale-insert
```

## Creating Devices
In order to connect devices to the IoT Hub, the ```CreateDevices.py``` Project can be used. 
In the ```config.json``` file of the *Data Generation* folder, the parameters can be set.  
The device names are a concatenation of the parameter *device_basename* and the index, which starts at 1 and is padded to be a six figure number. The *n_devices_to_create* tells the script how many devices to create. Furthermore the *iot_hub_hostname* and *iot_hub_owner_connection_string* needs to be passed.