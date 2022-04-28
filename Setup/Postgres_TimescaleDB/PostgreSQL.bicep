param skuName string
param location string = resourceGroup().location
param name string = 'postgres-server-tsdb-benchmark'
param administratorLogin string = 'benchmarkAdmin'
param administratorLoginPassword string = 'bench@123456'

resource symbolicname 'Microsoft.DBforPostgreSQL/servers@2017-12-01-preview' = {
  name: name
  location: location
  sku: {
    name: skuName
  }
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    sslEnforcement: 'Disabled'
    storageProfile: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
      storageAutogrow: 'Disabled'
      storageMB: 5120
    }
    version: '10.0'
    createMode: 'Default'
  }
}
