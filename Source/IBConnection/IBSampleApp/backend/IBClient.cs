using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;
using System.Windows.Forms;
using IBSampleApp.messages;

namespace IBSampleApp
{
    public class IBClient : EWrapper
    {
        private EClientSocket clientSocket;
        private int nextOrderId;
        private IBSampleApp parentUI;
        private int clientId;

        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        public IBClient(IBSampleApp parent)
        {
            parentUI = parent;
            clientSocket = new EClientSocket(this);
        }

        public EClientSocket ClientSocket
        {
            get { return clientSocket; }
            set { clientSocket = value; }
        }

        public int NextOrderId
        {
            get { return nextOrderId; }
            set { nextOrderId = value; }
        }

        public virtual void error(Exception e)
        {
            addTextToBox("Error: " + e);
        }

        public virtual void error(string str)
        {
            addTextToBox("Error: " + str + "\n");
        }

        public virtual void error(int id, int errorCode, string errorMsg)
        {
           parentUI.HandleMessage(new ErrorMessage(id, errorCode, errorMsg));
        }

        public virtual void connectionClosed()
        {
            parentUI.IsConnected = false;
            parentUI.HandleMessage(new ConnectionStatusMessage(false));
        }

        public virtual void currentTime(long time)
        {
            addTextToBox("Current Time: " + time + "\n");
        }

        public virtual void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            addTextToBox("Tick Price. Ticker Id:" + tickerId + ", Type: " + TickType.getField(field) + ", Price: " + price + "\n");
            parentUI.HandleMessage(new TickPriceMessage(tickerId, field, price, canAutoExecute));
        }

        public virtual void tickSize(int tickerId, int field, int size)
        {
            addTextToBox("Tick Size. Ticker Id:" + tickerId + ", Type: " + TickType.getField(field) + ", Size: " + size + "\n");
            parentUI.HandleMessage(new TickSizeMessage(tickerId, field, size));
        }

        public virtual void tickString(int tickerId, int tickType, string value)
        {
            addTextToBox("Tick string. Ticker Id:" + tickerId + ", Type: " + TickType.getField(tickType) + ", Value: " + value + "\n");
        }

        public virtual void tickGeneric(int tickerId, int field, double value)
        {
            addTextToBox("Tick Generic. Ticker Id:" + tickerId + ", Field: " + TickType.getField(field) + ", Value: " + value + "\n");
        }

        public virtual void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureExpiry, double dividendImpact, double dividendsToExpiry)
        {
            addTextToBox("TickEFP. " + tickerId + ", Type: " + tickType + ", BasisPoints: " + basisPoints + ", FormattedBasisPoints: " + formattedBasisPoints + ", ImpliedFuture: " + impliedFuture + ", HoldDays: " + holdDays + ", FutureExpiry: " + futureExpiry + ", DividendImpact: " + dividendImpact + ", DividendsToExpiry: " + dividendsToExpiry + "\n");
        }

        public virtual void tickSnapshotEnd(int tickerId)
        {
            addTextToBox("TickSnapshotEnd: " + tickerId + "\n");
        }

        public virtual void nextValidId(int orderId)
        {
            parentUI.IsConnected = true;
            NextOrderId = orderId;
            parentUI.HandleMessage(new ConnectionStatusMessage(true));
        }

        public virtual void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            addTextToBox("DeltaNeutralValidation. " + reqId + ", ConId: " + underComp.ConId + ", Delta: " + underComp.Delta + ", Price: " + underComp.Price + "\n");
        }

        public virtual void managedAccounts(string accountsList)
        {
            parentUI.HandleMessage(new ManagedAccountsMessage(accountsList));
        }

        public virtual void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            //addTextToBox("TickOptionComputation. TickerId: " + tickerId + ", field: " + field + ", ImpliedVolatility: " + impliedVolatility + ", Delta: " + delta
              //  + ", OptionPrice: " + optPrice + ", pvDividend: " + pvDividend + ", Gamma: " + gamma + ", Vega: " + vega + ", Theta: " + theta + ", UnderlyingPrice: " + undPrice + "\n");
            parentUI.HandleMessage(new TickOptionMessage(tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice));
        }

        public virtual void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            parentUI.HandleMessage(new AccountSummaryMessage(reqId, account, tag, value, currency));
        }

        public virtual void accountSummaryEnd(int reqId)
        {
            parentUI.HandleMessage(new AccountSummaryEndMessage(reqId));
        }

        public virtual void updateAccountValue(string key, string value, string currency, string accountName)
        {
            parentUI.HandleMessage(new AccountValueMessage(key, value, currency, accountName));
        }

        public virtual void updatePortfolio(Contract contract, int position, double marketPrice, double marketValue, double averageCost, double unrealisedPNL, double realisedPNL, string accountName)
        {
            parentUI.HandleMessage(new UpdatePortfolioMessage(contract, position, marketPrice, marketValue, averageCost, unrealisedPNL, realisedPNL, accountName));
        }

        public virtual void updateAccountTime(string timestamp)
        {
            parentUI.HandleMessage(new UpdateAccountTimeMessage(timestamp));
        }

        public virtual void accountDownloadEnd(string account)
        {
            parentUI.HandleMessage(new AccountDownloadEndMessage(account));
        }

        public virtual void orderStatus(int orderId, string status, int filled, int remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
            parentUI.HandleMessage(new OrderStatusMessage(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld));
        }

        public virtual void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            parentUI.HandleMessage(new OpenOrderMessage(orderId, contract, order, orderState));
        }

        public virtual void openOrderEnd()
        {
            parentUI.HandleMessage(new OpenOrderEndMessage());
        }

        public virtual void contractDetails(int reqId, ContractDetails contractDetails)
        {
            parentUI.HandleMessage(new ContractDetailsMessage(reqId, contractDetails));
        }

        public virtual void contractDetailsEnd(int reqId)
        {
            parentUI.HandleMessage(new ContractDetailsEndMessage());
        }

        public virtual void execDetails(int reqId, Contract contract, Execution execution)
        {
            parentUI.HandleMessage(new ExecutionMessage(reqId, contract, execution));
        }

        public virtual void execDetailsEnd(int reqId)
        {
            addTextToBox("ExecDetailsEnd. " + reqId + "\n");
        }

        public virtual void commissionReport(CommissionReport commissionReport)
        {
            parentUI.HandleMessage(new CommissionMessage(commissionReport));
        }

        public virtual void fundamentalData(int reqId, string data)
        {
            parentUI.HandleMessage(new FundamentalsMessage(data));
        }

        public virtual void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
            parentUI.HandleMessage(new HistoricalDataMessage(reqId, date, open, high, low, close, volume, count, WAP, hasGaps));
        }

        public virtual void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            parentUI.HandleMessage(new HistoricalDataEndMessage(reqId, startDate, endDate));
        }

        public virtual void marketDataType(int reqId, int marketDataType)
        {
            addTextToBox("MarketDataType. " + reqId + ", Type: " + marketDataType + "\n");
        }

        public virtual void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            parentUI.HandleMessage(new DeepBookMessage(tickerId, position, operation, side, price, size, ""));
        }

        public virtual void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            parentUI.HandleMessage(new DeepBookMessage(tickerId, position, operation, side, price, size, marketMaker));
        }

        public virtual void updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
            addTextToBox("News Bulletins. " + msgId + " - Type: " + msgType + ", Message: " + message + ", Exchange of Origin: " + origExchange + "\n");
        }

        public virtual void position(string account, Contract contract, int pos, double avgCost)
        {
            parentUI.HandleMessage(new PositionMessage(account, contract, pos, avgCost));
        }

        public virtual void positionEnd()
        {
            addTextToBox("PositionEnd \n");
        }

        public virtual void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            parentUI.HandleMessage(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count));
        }

        public virtual void scannerParameters(string xml)
        {
            addTextToBox("ScannerParameters. " + xml + "\n");
        }

        public virtual void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            parentUI.HandleMessage(new ScannerMessage(reqId, rank, contractDetails, distance, benchmark, projection, legsStr));
        }

        public virtual void scannerDataEnd(int reqId)
        {
            addTextToBox("ScannerDataEnd. " + reqId + "\r\n");
        }

        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            parentUI.HandleMessage(new AdvisorDataMessage(faDataType, faXmlData));
        }

        public virtual void bondContractDetails(int requestId, ContractDetails contractDetails)
        {
            addTextToBox("Receiving bond contract details.");
        }

        private void addTextToBox(string text)
        {
            parentUI.HandleMessage(new ErrorMessage(-1, -1, text));
        }

    }
}
