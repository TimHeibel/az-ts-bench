param skuName string
param skuCapacity int
param name string = 'iot-hub-tsdb-benchmark'
param location string = resourceGroup().location

resource hubname_resource 'Microsoft.Devices/IotHubs@2021-07-01-preview' = {
  name: name
  location: location
  properties: {
    eventHubEndpoints: {
      events: {
        retentionTimeInDays: 1
        partitionCount: 32
      }
    }
    features: 'None'
    disableLocalAuth: false
  }
  sku: {
    name: skuName
    capacity: skuCapacity
  }
}

resource Microsoft_Security_IoTSecuritySolutions_hubname 'Microsoft.Security/IoTSecuritySolutions@2019-08-01' = {
  name: 'iot-hub-tsdb-benchmark'
  location: location
  properties: {
    status: 'Enabled'
    unmaskedIpLoggingStatus: 'Enabled'
    disabledDataSources: []
    displayName: 'iot-hub-tsdb-benchmark'
    iotHubs: [
      hubname_resource.id
    ]
    recommendationsConfiguration: []
  }
}
