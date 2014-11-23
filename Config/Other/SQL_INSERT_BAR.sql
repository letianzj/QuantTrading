BULK INSERT MarketData.dbo.SPY
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\SPY STK SMART_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.ESG1
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\ESM3 FUT GLOBEX_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.ESG2
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\ESU3 FUT GLOBEX_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.QQQ
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\QQQ STK SMART_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.NQG1
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\NQM3 FUT GLOBEX_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.NQG2
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\NQU3 FUT GLOBEX_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.QMG1
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\QMK3 FUT NYMEX_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO

BULK INSERT MarketData.dbo.QMG2
FROM 'C:\Users\Letian\AppData\Local\TradeLinkTicks\Historical\QMM3 FUT NYMEX_20130327.csv'
WITH
(
	FIRSTROW = 2,
	FIELDTERMINATOR = ',',
	ROWTERMINATOR = '\n'
)
GO