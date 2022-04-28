# General
This folder contains the queries executed on the various databases and exemplary results of the benchmark. This file contains the testing strategy. 


## Execution
3 Amounts of Data:
- 1.000.000 Rows
- 10.000.000 Rows
- 100.000.000 Rows


## Sizes and Costs of Databases
- SQL DB
    - vCores: 2, 8, 16 
    - Costs: 339.15, 1345.79, 2687.98
    - Costs with existing License: 213.71, 844.03, 1684.46
- Postgres/TimescaleDB
    - vCores: 2, 8, 16
    - Costs: 128.79, 506.78, 1010.78
- Cosmos DB
    - RU/s: 1200, 4800, 9600
        - [Provisioned RU/s = C*T/R](https://docs.microsoft.com/en-us/azure/cosmos-db/convert-vcore-to-request-unit)
            - Note: The formula is not applicable to vCores of relational databases, therefore this was just an approximation, it would have been possible to use the costs to set the RU/s
    - Costs: 64.01, 256.05, 512.11
- ADX
    - vCPUs: 2: Dev(No SLA)_Standard_E2a_v4, 8: Standard_D13_v2, 16: Standard_D14_v2
    - Costs: 100.63,  1077.93, 2155.85


## Query Types
- Point: SELECT * FROM benchTable WHERE deviceId = '\<deviceId>' AND time = '\<time>'
    - 3 different queries
        - variable number of runs (depending on estimated duration, minimum 3)
- Range: SELECT * FROM benchTable WHERE deviceId = '\<deviceId>' AND time BETWEEN '\<time>' AND '\<time>'
    - 10%, 20%, 50% der Daten
        - 3 different queries
            - variable number of runs
- Aggregate: SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = '\<deviceId>'
    - AND time BETWEEN '\<time>' GROUP BY deviceId
    - 5 Runs
    - 3 different queries
    
# Queries
- These were not fully created automatically. It is advised to create a script to create these queries. 
## 1.000.000
### SQLDB
####Point
- SELECT * FROM benchTable WHERE deviceId = 'sim000619' AND time = '2022-03-07 09:57:09.837'
- SELECT * FROM benchTable WHERE deviceId = 'sim002291' AND time = '2022-03-07 09:57:10.913'
- SELECT * FROM benchTable WHERE deviceId = 'sim003412' AND time = '2022-03-07 10:01:25.290'

#### Range
##### 10%
- SELECT * FROM benchTable WHERE deviceId = 'sim000555' AND time BETWEEN  '2022-03-07 10:01:00.757' AND '2022-03-07 10:01:25.320'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 10:01:00.290' AND '2022-03-07 10:01:24.790'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 10:00:59.977' AND '2022-03-07 10:01:24.663'

##### 20%
- SELECT * FROM benchTable WHERE deviceId = 'sim000555' AND time BETWEEN  '2022-03-07 10:00:35.117' AND '2022-03-07 10:01:25.320'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 10:00:34.650' AND '2022-03-07 10:01:24.790'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 10:00:34.460' AND '2022-03-07 10:01:24.663'

##### 50%
- SELECT * FROM benchTable WHERE deviceId = 'sim000555' AND time BETWEEN  '2022-03-07 09:59:18.087' AND '2022-03-07 10:01:25.320'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 09:59:17.460' AND '2022-03-07 10:01:24.790'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 09:59:17.710' AND '2022-03-07 10:01:24.663'

#### Aggregate
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim000269' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim001485' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim003795' GROUP BY deviceId



### ADX
#### Point
- benchTable | where deviceId == 'sim000619' and timestamp == '2022-03-07T09:57:09.835256Z'
- benchTable | where deviceId == 'sim002291' and timestamp == '2022-03-07T09:57:10.913407Z'
- benchTable | where deviceId == 'sim003412' and timestamp == '2022-03-07T10:01:25.289257Z'

#### Range
##### 10%
- benchTable | where deviceId == 'sim000555' and timestamp between (datetime('2022-03-07 10:01:00.757')..datetime('2022-03-07 10:01:25.999')) 
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 10:01:00.000')..datetime('2022-03-07 10:01:24.999')) 
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 10:00:59.000')..datetime('2022-03-07 10:01:24.999')) 

##### 20%
- benchTable | where deviceId == 'sim000555' and timestamp between (datetime('2022-03-07 10:00:35.117')..datetime('2022-03-07 10:01:25.999'))
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 10:00:34.000')..datetime('2022-03-07 10:01:24.999'))
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 10:00:34.460')..datetime('2022-03-07 10:01:24.999'))

##### 50%
- benchTable | where deviceId == 'sim000555' and timestamp between (datetime('2022-03-07 09:59:18.000')..datetime('2022-03-07 10:01:25.999'))
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 09:59:17.000')..datetime('2022-03-07 10:01:24.999'))
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 09:59:17.000')..datetime('2022-03-07 10:01:24.999'))

#### Aggregate
- benchTable | where deviceId == 'sim000269' | summarize avg(value) by deviceId
- benchTable | where deviceId == 'sim001485' | summarize avg(value) by deviceId
- benchTable | where deviceId == 'sim003795' | summarize avg(value) by deviceId



### CosmosDB
#### Point
- SELECT * FROM c WHERE c.deviceId = 'sim000619' AND c.time = '2022-03-07 09:57:09.835256'
- SELECT * FROM c WHERE c.deviceId = 'sim002291' AND c.time = '2022-03-07 09:57:10.913407'
- SELECT * FROM c WHERE c.deviceId = 'sim003412' AND c.time = '2022-03-07 10:00:01.195349'

#### Range
##### 10%
- SELECT * FROM c WHERE c.deviceId = 'sim000555' AND c['time'] > '2022-03-07 10:01:00.000' AND c['time'] <= '2022-03-07 10:01:25.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] > '2022-03-07 10:01:00.000' AND c['time'] <= '2022-03-07 10:01:24.999'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] > '2022-03-07 10:00:59.000' AND c['time'] <= '2022-03-07 10:01:24.999'

##### 20%
- SELECT * FROM c WHERE c.deviceId = 'sim000555' AND c['time'] > '2022-03-07 10:00:35.117' AND c['time'] <= '2022-03-07 10:01:25.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] > '2022-03-07 10:00:34.000' AND c['time'] <= '2022-03-07 10:01:24.999'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] > '2022-03-07 10:00:34.460' AND c['time'] <= '2022-03-07 10:01:24.999'

##### 50%
- SELECT * FROM c WHERE c.deviceId = 'sim000555' AND c['time'] > '2022-03-07 09:59:18.000' AND c['time'] <= '2022-03-07 10:01:25.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] > '2022-03-07 09:59:17.460' AND c['time'] <= '2022-03-07 10:01:24.790'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] > '2022-03-07 09:59:17.710' AND c['time'] <= '2022-03-07 10:01:24.999'

#### Aggregate
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim000269' GROUP BY c.deviceId
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim001485' GROUP BY c.deviceId
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim003795' GROUP BY c.deviceId



### Postgres
#### Point
- SELECT * FROM benchTable WHERE deviceId = 'sim000619' AND time = '2022-03-07T09:57:09.835256Z'
- SELECT * FROM benchTable WHERE deviceId = 'sim002291' AND time = '2022-03-07T09:57:10.913407Z'
- SELECT * FROM benchTable WHERE deviceId = 'sim003412' AND time = '2022-03-07T10:01:25.289257Z'

#### Range
##### 10%
- SELECT * FROM benchTable WHERE deviceId = 'sim000555' AND time BETWEEN  '2022-03-07 10:01:00.757' AND '2022-03-07 10:01:25.999'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 10:01:00.000' AND '2022-03-07 10:01:24.999'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 10:00:59.000' AND '2022-03-07 10:01:24.999'

##### 20%
- SELECT * FROM benchTable WHERE deviceId = 'sim000555' AND time BETWEEN  '2022-03-07 10:00:35.000' AND '2022-03-07 10:01:25.999'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 10:00:34.000' AND '2022-03-07 10:01:24.999'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 10:00:34.000' AND '2022-03-07 10:01:24.999'

##### 50%
- SELECT * FROM benchTable WHERE deviceId = 'sim000555' AND time BETWEEN  '2022-03-07 09:59:18.000' AND '2022-03-07 10:01:25.999'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 09:59:17.000' AND '2022-03-07 10:01:24.999'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 09:59:17.000' AND '2022-03-07 10:01:24.999'

#### Aggregate
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim000269' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim001485' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim003795' GROUP BY deviceId

### TimescaleDB
#### Point
SELECT * FROM benchtable_1mil WHERE deviceid = 'sim000895' AND time = '2022-03-07T19:23:59.057520' 
SELECT * FROM benchtable_1mil WHERE deviceid = 'sim001285' AND time = '2022-03-07T19:23:59.073148' 
SELECT * FROM benchtable_1mil WHERE deviceid = 'sim000659' AND time = '2022-03-07T19:23:59.401272'

#### Range
###### 10%
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:27:50.794818' AND '2022-03-07T19:28:15.154511'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T19:27:50.154339' AND '2022-03-07T19:28:14.701383'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:27:50.763568' AND '2022-03-07T19:28:15.279513'
#### 20%
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:27:24.872644' AND '2022-03-07T19:28:15.154511'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T19:27:24.685140' AND '2022-03-07T19:28:14.701383'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:27:25.263274' AND '2022-03-07T19:28:15.279513'

##### 50%
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:26:07.949838' AND '2022-03-07T19:28:15.154511'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T19:26:07.746710' AND '2022-03-07T19:28:14.701383'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:26:08.449844' AND '2022-03-07T19:28:15.279513'

##### 80%
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:24:51.136381' AND '2022-03-07T19:28:15.154511'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T20:03:16.603115' AND '2022-03-07T19:28:14.701383'
- SELECT * FROM benchtable_1mil WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:24:51.683266' AND '2022-03-07T19:28:15.279513'

#### Aggregate
- SELECT AVG(value), deviceid FROM benchtable_1mil WHERE deviceid = 'sim000841' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable_1mil WHERE deviceid = 'sim001153' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable_1mil WHERE deviceid = 'sim002684' GROUP BY deviceid









## 10.000.000
### SQLDB
#### Point
- SELECT * FROM benchTable WHERE deviceId = 'sim001285' AND time = '2022-03-05 16:43:37.407'
- SELECT * FROM benchTable WHERE deviceId = 'sim001285' AND time = '2022-03-05 16:30:32.867'
- SELECT * FROM benchTable WHERE deviceId = 'sim000659' AND time = '2022-03-05 16:43:34.843'

#### Range
##### 10%
- SELECT * FROM benchTable WHERE deviceId = 'sim003543' AND time BETWEEN  '2022-03-05 17:05:22.950' AND '2022-03-05 17:09:37.407'
- SELECT * FROM benchTable WHERE deviceId = 'sim001549' AND time BETWEEN  '2022-03-05 17:05:27.450' AND '2022-03-05 17:09:42.593'
- SELECT * FROM benchTable WHERE deviceId = 'sim000513' AND time BETWEEN  '2022-03-05 17:05:30.843' AND '2022-03-05 17:09:46.690'

##### 20%
- SELECT * FROM benchTable WHERE deviceId = 'sim003543' AND time BETWEEN  '2022-03-05 17:01:06.963' AND '2022-03-05 17:09:37.407'
- SELECT * FROM benchTable WHERE deviceId = 'sim001549' AND time BETWEEN  '2022-03-05 17:01:11.323' AND '2022-03-05 17:09:42.593'
- SELECT * FROM benchTable WHERE deviceId = 'sim000513' AND time BETWEEN  '2022-03-05 17:01:14.340' AND '2022-03-05 17:09:46.690'

##### 50%
- SELECT * FROM benchTable WHERE deviceId = 'sim003543' AND time BETWEEN  '2022-03-05 16:48:18.643' AND '2022-03-05 17:09:37.407'
- SELECT * FROM benchTable WHERE deviceId = 'sim001549' AND time BETWEEN  '2022-03-05 16:48:21.813' AND '2022-03-05 17:09:42.593'
- SELECT * FROM benchTable WHERE deviceId = 'sim000513' AND time BETWEEN  '2022-03-05 16:48:23.113' AND '2022-03-05 17:09:46.690'

#### Aggregate
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim000249' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim003862' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim000022' GROUP BY deviceId



### ADX
#### Point
- benchTable | where deviceId == 'sim001285' and timestamp == '2022-03-05T16:53:31.474465Z'
- benchTable | where deviceId == 'sim001285' and  timestamp == '2022-03-05T16:30:32.865446Z'
- benchTable | where deviceId == 'sim000659' and timestamp == '2022-03-05T16:43:34.842832Z'

#### Range
##### 10%
- benchTable | where deviceId == 'sim003543' and timestamp between (datetime('2022-03-05 17:05:22.950').. datetime('2022-03-05 17:09:37.408')) 
- benchTable | where deviceId == 'sim001549' and timestamp between (datetime('2022-03-05 17:05:20.950').. datetime('2022-03-05 17:09:37.408')) 
- benchTable | where deviceId == 'sim000513' and timestamp between (datetime('2022-03-05 17:05:20.950').. datetime('2022-03-05 17:09:37.408')) 

##### 20%
- benchTable | where deviceId == 'sim003543' and timestamp between (datetime('2022-03-05 17:01:05.950').. datetime('2022-03-05 17:09:37.408'))
- benchTable | where deviceId == 'sim001549' and timestamp between (datetime('2022-03-05 17:01:04.950').. datetime('2022-03-05 17:09:37.408'))
- benchTable | where deviceId == 'sim000513' and timestamp between (datetime('2022-03-05 17:01:04.950').. datetime('2022-03-05 17:09:37.408'))

##### 50%
- benchTable | where deviceId == 'sim003543' and timestamp between (datetime('2022-03-05 16:48:17.950').. datetime('2022-03-05 17:09:37.408'))
- benchTable | where deviceId == 'sim001549' and timestamp between (datetime('2022-03-05 16:48:14.950').. datetime('2022-03-05 17:09:37.408'))
- benchTable | where deviceId == 'sim000513' and timestamp between (datetime('2022-03-05 16:48:12.950').. datetime('2022-03-05 17:09:37.408'))

#### Aggregate
- benchTable | where deviceId == 'sim000249' | summarize avg(value) by deviceId
- benchTable | where deviceId == 'sim003862' | summarize avg(value) by deviceId
- benchTable | where deviceId == 'sim000022' | summarize avg(value) by deviceId



### CosmosDB
#### Point
- SELECT * FROM c WHERE c.deviceId = 'sim001285' AND c['time'] = '2022-03-05 16:43:22.248903'
- SELECT * FROM c WHERE c.deviceId = 'sim001288'  AND c['time'] = '2022-03-05 16:26:59.191223'
- SELECT * FROM c WHERE c.deviceId = 'sim000659'  AND c['time'] = '2022-03-05 16:26:58.878720'

#### Range
##### 10%
- SELECT * FROM c WHERE c.deviceId = 'sim003543' AND c['time'] >  '2022-03-05 17:01:51' AND c['time'] <= '2022-03-05 17:10:08'
- SELECT * FROM c WHERE c.deviceId = 'sim001549' AND c['time'] >  '2022-03-05 17:01:36' AND c['time'] <= '2022-03-05 17:10:08'
- SELECT * FROM c WHERE c.deviceId = 'sim000513' AND c['time'] >  '2022-03-05 17:02:11' AND c['time'] <= '2022-03-05 17:10:08'

##### 20%
- SELECT * FROM c WHERE c.deviceId = 'sim003543' AND c['time'] >  '2022-03-05 16:57:35' AND c['time'] <= '2022-03-05 17:10:08'
- SELECT * FROM c WHERE c.deviceId = 'sim001549' AND c['time'] >  '2022-03-05 16:57:20' AND c['time'] <= '2022-03-05 17:10:08'
- SELECT * FROM c WHERE c.deviceId = 'sim000513' AND c['time'] >  '2022-03-05 16:57:54' AND c['time'] <= '2022-03-05 17:10:08'

##### 50%
- SELECT * FROM c WHERE c.deviceId = 'sim003543' AND c['time'] >  '2022-03-05 16:51:01' AND c['time'] <= '2022-03-05 17:10:08'
- SELECT * FROM c WHERE c.deviceId = 'sim001549' AND c['time'] >  '2022-03-05 16:49:58.0' AND c['time'] <= '2022-03-05 17:10:08'
- SELECT * FROM c WHERE c.deviceId = 'sim000513' AND c['time'] >  '2022-03-05 16:45:03' AND c['time'] <= '2022-03-05 17:10:08'

#### Aggregate
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim000249' GROUP BY c.deviceId
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim003862' GROUP BY c.deviceId
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim000022' GROUP BY c.deviceId



### Postgres
#### Point
- SELECT * FROM benchtable WHERE deviceid = 'sim001285' AND time = '2022-03-05T16:43:37.405494+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim001285' AND time = '2022-03-05T16:33:22.632956+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim000659' AND time = '2022-03-05T16:43:35.874093+00:00'

#### Range
##### 10%
- SELECT * FROM benchtable WHERE deviceid = 'sim003543' AND time BETWEEN '2022-03-05T17:05:22.951422+00:00' AND '2022-03-05T17:09:37.407367+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim001549' AND time BETWEEN '2022-03-05T17:05:22.951422+00:00' AND '2022-03-05T17:09:38.594919+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim000513' AND time BETWEEN '2022-03-05T17:05:30.842124+00:00' AND '2022-03-05T17:09:46.688706+00:00'

#### 20%
- SELECT * FROM benchtable WHERE deviceid = 'sim003543' AND time BETWEEN '2022-03-05T17:01:06.964154+00:00' AND '2022-03-05T17:09:37.407367+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim001549' AND time BETWEEN '2022-03-05T17:01:11.323576+00:00' AND '2022-03-05T17:09:42.594919+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim000513' AND time BETWEEN '2022-03-05T17:01:14.339228+00:00' AND '2022-03-05T17:09:46.688706+00:00'

##### 50%
- SELECT * FROM benchtable WHERE deviceid = 'sim003543' AND time BETWEEN '2022-03-05T16:48:18.642881+00:00' AND '2022-03-05T17:09:37.407367+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim001549' AND time BETWEEN '2022-03-05T16:48:21.814789+00:00' AND '2022-03-05T17:09:42.594919+00:00'
- SELECT * FROM benchtable WHERE deviceid = 'sim000513' AND time BETWEEN '2022-03-05T16:48:23.111678+00:00' AND '2022-03-05T17:09:46.688706+00:00'

#### Aggregate
- SELECT AVG(value), deviceid FROM benchtable WHERE deviceid = 'sim000249' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable WHERE deviceid = 'sim003862' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable WHERE deviceid = 'sim000022' GROUP BY deviceid

### Timescale
#### Point
- SELECT * FROM benchtable_10mil WHERE deviceid = 'sim000895' AND time = '2022-03-07T19:23:59.057520' 
- SELECT * FROM benchtable_10mil WHERE deviceid = 'sim001285' AND time = '2022-03-07T19:23:59.073148' 
- SELECT * FROM benchtable_10mil WHERE deviceid = 'sim000659' AND time = '2022-03-07T19:23:59.401272'

#### Range
##### 10%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T20:02:26.336770' AND '2022-03-07T20:06:40.949300'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T20:02:25.524266' AND '2022-03-07T20:06:40.183673'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T20:02:25.118015' AND '2022-03-07T20:06:40.808674'

#### 20%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:58:09.927302' AND '2022-03-07T20:06:40.949300'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T19:58:08.739801' AND '2022-03-07T20:06:40.183673'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:58:08.942927' AND '2022-03-07T20:06:40.808674'

##### 50%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:45:21.870479' AND '2022-03-07T20:06:40.949300'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T19:45:21.136102' AND '2022-03-07T20:06:40.183673'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:45:18.995472' AND '2022-03-07T20:06:40.808674'

##### 80%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07T19:32:33.407640' AND '2022-03-07T20:06:40.949300'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07T19:32:34.407651' AND '2022-03-07T20:06:40.183673'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07T19:32:29.329465' AND '2022-03-07T20:06:40.808674'

#### Aggregate
- SELECT AVG(value), deviceid FROM benchtable_new WHERE deviceid = 'sim000841' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable_new WHERE deviceid = 'sim001153' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable_new WHERE deviceid = 'sim002684' GROUP BY deviceid








    
## 100.000.000
### SQLDB
#### Point
- SELECT * FROM benchTable WHERE deviceId = 'sim000773' AND time = '2022-03-07 19:38:46.460'
- SELECT * FROM benchTable WHERE deviceId = 'sim002917' AND time = '2022-03-07 19:38:46.477'
- SELECT * FROM benchTable WHERE deviceId = 'sim003507' AND time = '2022-03-07 19:38:46.460'

#### Range
##### 10%
- SELECT * FROM benchTable WHERE deviceId = 'sim000731' AND time BETWEEN  '2022-03-07 19:23:59.497' AND '2022-03-07 20:07:25.903'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 19:23:59.103' AND '2022-03-07 20:06:39.153'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 19:23:59.183' AND '2022-03-07 20:06:39.917'

##### 20%
- SELECT * FROM benchTable WHERE deviceId = 'sim000731' AND time BETWEEN  '2022-03-07 19:23:59.497' AND '2022-03-07 20:49:33.997'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 19:23:59.103' AND '2022-03-07 20:49:25.060'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 19:23:59.183' AND '2022-03-07 20:49:14.843'

##### 50%
- SELECT * FROM benchTable WHERE deviceId = 'sim000731' AND time BETWEEN  '2022-03-07 19:23:59.497' AND '2022-03-07 22:54:50.200'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 19:23:59.103' AND '2022-03-07 22:54:40.140'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 19:23:59.183' AND '2022-03-07 22:54:02.030'

##### 80%
- SELECT * FROM benchTable WHERE deviceId = 'sim000731' AND time BETWEEN  '2022-03-07 19:23:59.497' AND '2022-03-08 01:03:05.907'
- SELECT * FROM benchTable WHERE deviceId = 'sim001337' AND time BETWEEN  '2022-03-07 19:23:59.103' AND '2022-03-08 01:02:58.000'
- SELECT * FROM benchTable WHERE deviceId = 'sim003547' AND time BETWEEN  '2022-03-07 19:23:59.183' AND '2022-03-08 01:01:58.950'

#### Aggregate
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim000841' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim001153' GROUP BY deviceId
- SELECT AVG(value), deviceId FROM benchTable WHERE deviceId = 'sim002684' GROUP BY deviceId


## ADX
#### Point
- benchTable | where deviceId == 'sim000773' and timestamp == '2022-03-07T19:38:47.490666Z'
- benchTable | where deviceId == 'sim002917' and timestamp == '2022-03-07T19:38:47.490666Z'
- benchTable | where deviceId == 'sim003507' and timestamp == '2022-03-07T19:23:59.495022Z'

#### Range
##### 10%
- benchTable | where deviceId == 'sim000731' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 20:06:45.000')) 
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 20:06:39.999')) 
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 20:06:39.999')) 

##### 20%
- benchTable | where deviceId == 'sim000731' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 20:49:33.999'))
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 20:49:25.666'))
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 20:49:14.999'))

##### 50%
- benchTable | where deviceId == 'sim000731' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 22:57:39.999'))
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 22:57:41.999'))
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-07 22:57:06.666'))

##### 80%
- benchTable | where deviceId == 'sim000731' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-08 01:06:09.999'))
- benchTable | where deviceId == 'sim001337' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-08 01:06:00.000'))
- benchTable | where deviceId == 'sim003547' and timestamp between (datetime('2022-03-07 19:23:59.000').. datetime('2022-03-08 01:05:02.950'))

#### Aggregate
- benchTable | where deviceId == 'sim000841' | summarize avg(value) by deviceId
- benchTable | where deviceId == 'sim001153' | summarize avg(value) by deviceId
- benchTable | where deviceId == 'sim002684' | summarize avg(value) by deviceId



### CosmosDB
#### Point
- SELECT * FROM c WHERE c.deviceId = 'sim000773' AND c['time'] = '2022-03-07 19:23:59.526271'
- SELECT * FROM c WHERE c.deviceId = 'sim002917' AND c['time'] = '2022-03-07 19:23:59.370024'
- SELECT * FROM c WHERE c.deviceId = 'sim003507' AND c['time'] = '2022-03-07 19:24:04.245018'

#### Range
##### 10%
- SELECT * FROM c WHERE c.deviceId = 'sim000731' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 20:07:25.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 20:06:39.999'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 20:06:39.999'

##### 20%
- SELECT * FROM c WHERE c.deviceId = 'sim000731' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 20:49:33.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 20:49:25.666'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 20:49:14.999'

##### 50%
- SELECT * FROM c WHERE c.deviceId = 'sim000731' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 22:54:50.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 22:54:40.999'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-07 22:54:02.666'

##### 80%
- SELECT * FROM c WHERE c.deviceId = 'sim000731' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-08 01:03:05.999'
- SELECT * FROM c WHERE c.deviceId = 'sim001337' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-08 01:02:58.666'
- SELECT * FROM c WHERE c.deviceId = 'sim003547' AND c['time'] >  '2022-03-07 19:23:59.000' AND c['time'] <= '2022-03-08 01:01:58.950'

#### Aggregate
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim000841' GROUP BY c.deviceId
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim001153' GROUP BY c.deviceId
- SELECT AVG(c['value']), c.deviceId FROM c WHERE c.deviceId = 'sim002684' GROUP BY c.deviceId

### Postgres AND Timescale
#### Point
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001285' AND time = '2022-03-08T02:00:00.947298'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001285' AND time = '2022-03-07T19:23:59.073148' 
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000659' AND time < '2022-03-07T19:23:59.401272'

#### Range
##### 10%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07 19:24:38.999' AND '2022-03-07 20:07:25.000'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 20:06:39.999'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 20:06:39.999'

#### 20%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 20:49:33.999'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 20:49:25.666'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 20:49:14.999'

##### 50%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 22:54:36.000'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 22:54:40.000'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-07 22:54:02.666'

##### 80%
- SELECT * FROM benchtable_new WHERE deviceid = 'sim000731' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-08 01:03:05.000'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim001337' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-08 01:02:57.000'
- SELECT * FROM benchtable_new WHERE deviceid = 'sim003547' AND time BETWEEN '2022-03-07 19:23:59.000' AND '2022-03-08 01:01:58.950'

#### Aggregate
- SELECT AVG(value), deviceid FROM benchtable_new WHERE deviceid = 'sim000841' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable_new WHERE deviceid = 'sim001153' GROUP BY deviceid
- SELECT AVG(value), deviceid FROM benchtable_new WHERE deviceid = 'sim002684' GROUP BY deviceid