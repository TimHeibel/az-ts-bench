param skuName string = 'GP_Gen5'
param skuCapacity int = 2
param location string = resourceGroup().location
param name string = 'sql-server-tsdb-benchmark'
param administratorLogin string = 'benchmarkAdmin'
param administratorLoginPassword string = 'bench@123456'

resource sql_server 'Microsoft.Sql/servers@2021-05-01-preview' = {
  name: name
  location: location
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
    minimalTlsVersion: '1.2'
    primaryUserAssignedIdentityId: 'string'
  }
}

resource sql_database 'Microsoft.Sql/servers/databases@2021-05-01-preview' = {
  parent: sql_server
  name: 'sql-db-tsdb-benchmark'
  location: location
  sku: {
    name: skuName
    capacity: skuCapacity
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    licenseType: 'LicenseIncluded'
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Geo'
    isLedgerOn: false
  }
}
