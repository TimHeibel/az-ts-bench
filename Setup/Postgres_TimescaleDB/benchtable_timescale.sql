DROP TABLE IF EXISTS benchtable;

CREATE EXTENSION IF NOT EXISTS timescaledb CASCADE;

CREATE TABLE benchtable_ts(
	time timestamptz NOT NULL,
	value float NOT NULL,
	deviceid varchar(50)  NOT NULL
);

SELECT * FROM create_hypertable('benchtable_ts','time', partitioning_column => 'deviceid', number_partitions => 4);
