-- CREATE TABLE
CREATE TABLE CLG2
(
	Bartime datetime NOT NULL primary key,
	Openprice float not null,
	Highprice float not null,
	Lowprice float not null,
	Closeprice float not null,
	Volume float not null
	-- UNIQUE (Bartime)
);

-- CREATE INDEX on the table, avoiding duplicate rows
CREATE UNIQUE INDEX CLG2_IDX ON CLG2 (Bartime ASC)
	WITH (IGNORE_DUP_KEY = ON);
GO

CREATE TABLE OneMinuteBar
(
	Symbol char(100) Not NUll,
	Bartime datetime NOT NULL,
	Openprice float not null,
	Highprice float not null,
	Lowprice float not null,
	Closeprice float not null,
	Volume float not null,
	primary key (Symbol, Bartime)
	-- UNIQUE (Bartime)
);

-- CREATE INDEX on the table, avoiding duplicate rows
CREATE UNIQUE INDEX OneMinuteBar_IDX ON OneMinuteBar (Symbol, Bartime ASC)
	WITH (IGNORE_DUP_KEY = ON);
GO

-- Bulk insert, see sp_bulk_insert
BULK INSERT SPY
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\SPY STK SMART_20130324.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

-- Call stored procedure
exec sp_insertbarfromcsv '20130329','ESM3','ESU3', 'NQM3','NQU3','CLK3','CLM3'

-- Get data out of database, export to csv
-- Tools - Options - Query results - sql server - results to grid (or text) -> Include column headers when copying or saving the results.
select 
	'SPY' as symbol, 
	convert(varchar,Bartime,101) as d, 
	convert(varchar,Bartime,108) as t,
	Openprice as O,
	Highprice as HIGH,					-- HIGH is used to tell TickConverter that its a bar
	Lowprice as LOW,
	Closeprice as C, 
	Volume as Volume,
	1 as BI		-- 1s
from MarketData.dbo.ESG1
where Bartime between '2013-03-28 09:30:00' and '2013-03-28 16:00:00'
order by d asc, t asc