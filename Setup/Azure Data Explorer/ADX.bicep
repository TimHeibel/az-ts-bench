param cluster_name string = 'adxClusterTsdbBench'
param location string = resourceGroup().location
param skuName string = 'Dev(No SLA)_Standard_E2a_v4'
param skuTier string = 'Basic'
param skuCapacity int = 1

resource Clusters_clusteradx_name_resource 'Microsoft.Kusto/Clusters@2021-08-27' = {
  name: cluster_name
  location: location
  sku: {
    name: skuName
    tier: skuTier
    capacity: skuCapacity
  }
  zones: [
    '1'
    '3'
    '2'
  ]
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    trustedExternalTenants: [
      {
        value: '*'
      }
    ]
    enableDiskEncryption: false
    enableStreamingIngest: false
    enablePurge: false
    enableDoubleEncryption: false
    engineType: 'V3'
    acceptedAudiences: []
    restrictOutboundNetworkAccess: 'Disabled'
    allowedFqdnList: []
    publicNetworkAccess: 'Enabled'
    allowedIpRangeList: []
    enableAutoStop: true
  }
}

resource Clusters_clusteradx_name_tsdb_bench 'Microsoft.Kusto/Clusters/Databases@2021-08-27' = {
  parent: Clusters_clusteradx_name_resource
  name: 'adx_database_tsdb_benchmark'
  location: location
  kind: 'ReadWrite'
  properties: {
    softDeletePeriod: 'P365D'
    hotCachePeriod: 'P1D'
  }
}

// resource Clusters_clusteradx_name_tsdb_bench_8ee4d786_ffaf_4c6c_90a6_8b06c60dce0a 'Microsoft.Kusto/Clusters/Databases/PrincipalAssignments@2021-08-27' = {
//   parent: Clusters_clusteradx_name_tsdb_bench
//   name: '8ee4d786-ffaf-4c6c-90a6-8b06c60dce0a'
//   properties: {
//     principalId: 'tim.heibel@ml-pa.com'
//     role: 'Admin'
//     principalType: 'User'
//     tenantId: '23090d33-f0df-44f5-9356-137a7bdfe69c'
//   }
//   dependsOn: [
//     Clusters_clusteradx_name_resource
//   ]
// }
