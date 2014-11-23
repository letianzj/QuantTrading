-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
-- =============================================
-- Author:		<Letian Wang>
-- Create date: <2014 02 01>
-- Description:	<Bulk Insert Daily Historical Data from CSV>
-- =============================================
ALTER PROCEDURE [dbo].[sp_insertbarfromcsv]
	-- Add the parameters for the stored procedure here
	@dt varchar(30) = '20130328',
	@esg1 varchar(30) = 'ESM3',
	@esg2 varchar(30) = 'ESU3',
	@nqg1 varchar(30) = 'NQM3',
	@nqg2 varchar(30) = 'NQU3',
	@clg1 varchar(30) = 'CLK3',
	@clg2 varchar(30) = 'CLM3',
	@path varchar(50) = 'C:\QuantTrading\TickData\Historical\'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    DECLARE @fullpath varchar(200)
    
	-- Use Dynamci SQL because bulk insert only accepts plain text
	DECLARE @sql varchar(max)
	-- BULK INSERT MarketData.dbo.SPY
    -- FROM @fullpath
    -- WITH
    -- (
    --	FIRSTROW = 2,
    --	FIELDTERMINATOR = ',',
    --	ROWTERMINATOR = '\n'
    -- )

    
	-- SPY
	DECLARE @spy varchar(30)
	Set @spy = 'SPY STK SMART'
    Set @fullpath = @path + @spy + '_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.SPY FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);

	-- ESG1
    Set @fullpath = @path + @esg1 +  ' FUT GLOBEX_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.ESG1 FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);
    
	-- ESG2
    Set @fullpath = @path + @esg2 + ' FUT GLOBEX_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.ESG2 FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);

	-- QQQ
	DECLARE @qqq varchar(30)
	Set @qqq = 'QQQ STK SMART'
    Set @fullpath = @path + @qqq + '_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.QQQ FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);

	-- NQG1
    Set @fullpath = @path + @nqg1 + ' FUT GLOBEX_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.NQG1 FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);

	-- NQG2
    Set @fullpath = @path + @nqg2 + ' FUT GLOBEX_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.NQG2 FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);

	-- CLG1
    Set @fullpath = @path + @clg1 + ' FUT NYMEX_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.CLG1 FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);

	-- CLG2
    Set @fullpath = @path + @clg2 + ' FUT NYMEX_' + @dt + '.csv'
	-- PRINT @fullpath
    Set @sql = 'BULK INSERT MarketData.dbo.CLG2 FROM ''' + @fullpath + '''WITH (FIRSTROW = 2,FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'')';
	exec(@sql);
END
--GO
