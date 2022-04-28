param location string = resourceGroup().location
param name string = 'sql-cosmos-tsdb-benchmark'

resource name_resource 'Microsoft.DocumentDb/databaseAccounts@2021-10-15-preview' = {
  kind: 'GlobalDocumentDB'
  name: name
  location: location
  properties: {
    databaseAccountOfferType: 'Standard'
    locations: [
      {
        failoverPriority: 0
        locationName: 'West Europe'
      }
    ]
    backupPolicy: {
      type: 'Periodic'
      periodicModeProperties: {
        backupIntervalInMinutes: 240
        backupRetentionIntervalInHours: 8
        backupStorageRedundancy: 'Geo'
      }
    }
    isVirtualNetworkFilterEnabled: false
    virtualNetworkRules: []
    ipRules: []
    enableMultipleWriteLocations: false
    capabilities: []
    enableFreeTier: false
  }
  tags: {
    defaultExperience: 'Core (SQL)'
  }
}
