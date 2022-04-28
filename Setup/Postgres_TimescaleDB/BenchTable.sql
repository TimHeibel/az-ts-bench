DROP TABLE IF EXISTS benchtable;

CREATE TABLE benchtable(
	time timestamptz NOT NULL,
	value float NOT NULL,
	deviceid varchar(50) NOT NULL
);