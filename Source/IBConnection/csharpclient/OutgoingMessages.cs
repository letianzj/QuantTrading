using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBApi
{
    public class OutgoingMessages
    {
        public const int RequestMarketData = 1;
        public const int CancelMarketData = 2;
        public const int PlaceOrder = 3;
        public const int CancelOrder = 4;
        public const int RequestOpenOrders = 5;
        public const int RequestAccountData = 6;
        public const int RequestExecutions = 7;
        public const int RequestIds = 8;
        public const int RequestContractData = 9;
        public const int RequestMarketDepth = 10;
        public const int CancelMarketDepth = 11;
        public const int RequestNewsBulletins = 12;
        public const int CancelNewsBulletin = 13;
        public const int ChangeServerLog = 14;
        public const int RequestAutoOpenOrders = 15;
        public const int RequestAllOpenOrders = 16;
        public const int RequestManagedAccounts = 17;
        public const int RequestFA = 18;
        public const int ReplaceFA = 19;
        public const int RequestHistoricalData = 20;
        public const int ExerciseOptions = 21;
        public const int RequestScannerSubscription = 22;
        public const int CancelScannerSubscription = 23;
        public const int RequestScannerParameters = 24;
        public const int CancelHistoricalData = 25;
        public const int RequestCurrentTime = 49;
        public const int RequestRealTimeBars = 50;
        public const int CancelRealTimeBars = 51;
        public const int RequestFundamentalData = 52;
        public const int CancelFundamentalData = 53;
        public const int ReqCalcImpliedVolat = 54;
        public const int ReqCalcOptionPrice = 55;
        public const int CancelImpliedVolatility = 56;
        public const int CancelOptionPrice = 57;
        public const int RequestGlobalCancel = 58;
        public const int RequestMarketDataType = 59;
        public const int RequestPositions = 61;
        public const int RequestAccountSummary = 62;
        public const int CancelAccountSummary = 63;
        public const int CancelPositions = 64;
    }
}
