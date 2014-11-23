using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp
{
    public enum MessageType
    {
        TickPrice = 1,
        TickSize = 2,
        OrderStatus = 3,
        Error = 4,
        OpenOrder = 5,
        AccountValue = 6,
        PortfolioValue = 7,
        AccountUpdateTime = 8,
        NextValidId = 9,
        ContractData = 10,
        ExecutionData = 11,
        MarketDepth = 12,
        MarketDepthL2 = 13,
        NewsBulletins = 14,
        ManagedAccounts = 15,
        ReceiveFA = 16,
        HistoricalData = 17,
        BondContractData = 18,
        ScannerParameters = 19,
        ScannerData = 20,
        TickOptionComputation = 21,
        TickGeneric = 45,
        Tickstring = 46,
        TickEFP = 47,
        CurrentTime = 49,
        RealTimeBars = 50,
        FundamentalData = 51,
        ContractDataEnd = 52,
        OpenOrderEnd = 53,
        AccountDownloadEnd = 54,
        ExecutionDataEnd = 55,
        DeltaNeutralValidation = 56,
        TickSnapshotEnd = 57,
        MarketDataType = 58,
        CommissionsReport = 59,
        Position = 61,
        PositionEnd = 62,
        AccountSummary = 63,
        AccountSummaryEnd = 64,

        //Given that the TWS is not sending a termination message for the historical bars, we produce one
        HistoricalDataEnd = -HistoricalData,
        ScannerDataEnd = -ScannerData,

        ConnectionStatus = 100
    }
}
