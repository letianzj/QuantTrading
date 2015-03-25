/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/

// 1. Derive from EWrapper
// 2. Define EClientSocket in the EWrapper, initiate it with (this)
// To use
// 1. send out message (e.g, place order)
// in the main program, use EWrapper.EClientSocket.eConnect()
// 2. Receive Messages, override handlers in EWrapper
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using IBApi;

namespace IBConnectionTest
{
    public partial class IBConnectionTestForm : Form, EWrapper
    {
        int clientId = 0;
        int port = 7496;
        EClientSocket ibClient;
        int requestId = 0;

        ContractOrderDlg orderDlg;

        public IBConnectionTestForm()
        {
            InitializeComponent();

            orderDlg = new ContractOrderDlg();
            //contractStk = new Contract("SPY", SecurityType.Stock, "SMART");
            // new Contract("ES", SecurityType.Future, "GLOBEX","USD", "201306");
            //contractCrncy = new Contract("EUR", SecurityType.Cash, "IDEALPRO");

            ibClient = new EClientSocket(this);
        }

        public virtual void error(Exception e)
        {
            String str = "Error occured: " + e.ToString();
            // Debug is called in IBClient thread; liatbox is not thread-safe
            // listBoxError.Items.Add(str);
            AddTextToErrorListWindow(str);
            //throw e;//remove after testing!
        }

        public virtual void error(string s)
        {
            String str = "Error: " + s;
            // Debug is called in IBClient thread; liatbox is not thread-safe
            // listBoxError.Items.Add(str);
            AddTextToErrorListWindow(str);
        }

        //  Some error messages are also fired when tws is connected.
        public virtual void error(int id, int errorCode, string errorMsg)
        {
            //addTextToBox("Error. Id: " + id + ", Code: " + errorCode + ", Msg: " + errorMsg + "\n");
            String str = "requestId = " + id
                + "; Error code = " + errorCode
                + "; Error message = " + errorMsg;
            // Debug is called in IBClient thread; liatbox is not thread-safe
            // listBoxError.Items.Add(str);
            AddTextToErrorListWindow(str);
        }

        public virtual void currentTime(long time)
        {
            String str = "current time = " + time.ToString();

            AddTextToResponseListWindow(str);
        }


        delegate void AddTextToDebugCallback(string txt);
        private void AddTextToErrorListWindow(string txt)
        {
            if (this.listBoxError.InvokeRequired)
            {

                AddTextToDebugCallback addTxt = new AddTextToDebugCallback(AddTextToErrorListWindow);
                this.Invoke(addTxt, new object[] { txt });
            }
            else
            {
                this.listBoxError.Items.Add(txt);
            }
        }
        private void AddTextToDataListWindow(string txt)
        {
            if (this.listBoxData.InvokeRequired)
            {

                AddTextToDebugCallback addTxt = new AddTextToDebugCallback(AddTextToDataListWindow);
                this.Invoke(addTxt, new object[] { txt });
            }
            else
            {
                this.listBoxData.Items.Add(txt);
            }
        }
        private void AddTextToResponseListWindow(string txt)
        {
            if (this.listBoxResponse.InvokeRequired)
            {

                AddTextToDebugCallback addTxt = new AddTextToDebugCallback(AddTextToResponseListWindow);
                this.Invoke(addTxt, new object[] { txt });
            }
            else
            {
                this.listBoxResponse.Items.Add(txt);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            listBoxResponse.Items.Add("Connecting to Tws using clientId " + clientId);
            ibClient.eConnect("127.0.0.1", port, clientId);

            if (ibClient.IsConnected())
            {
                listBoxResponse.Items.Add("Connected to Tws server version "
                    + ibClient.ServerVersion);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            listBoxData.Items.Clear();
            listBoxResponse.Items.Clear();
            listBoxError.Items.Clear();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            ibClient.eDisconnect();
            listBoxResponse.Items.Add("Disconnected from Tws server");
        }

        private void btnReqMktData_Click(object sender, EventArgs e)
        {            
            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                string genericTicklist = null;
                bool snapshot = false;
                ibClient.reqMktData(requestId, contract, genericTicklist, snapshot);
            }

            
            //Interlocked.Increment(ref requestId);
        }

        private void btnCancelMktData_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.cancelMktData(requestId);
            }
        }

        private void btnReqMktDepth_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Contract contract = orderDlg.getContract();              

                requestId = Convert.ToInt32(orderDlg.tbRequestId.Text);
                int numRows = Convert.ToInt32(orderDlg.tbNumOfRows.Text);

                ibClient.reqMarketDepth(requestId, contract, numRows);
            }
        }

        private void btnCancelMktDepth_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                requestId = Convert.ToInt32(orderDlg.tbRequestId.Text);

                ibClient.cancelMktDepth(requestId);
            }
        }

        private void btnHistData_Click(object sender, EventArgs e)
        {
            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                // DateTime endDateTime = DateTime.ParseExact(orderDlg.tbEndDate.Text, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
                string endDateTime = orderDlg.tbEndDate.Text;
                // TimeSpan duration = new TimeSpan(Int32.Parse(orderDlg.tbDuration.Text, CultureInfo.InvariantCulture), 0, 0);
                string duration = orderDlg.tbDuration.Text + " D";          // Days
                string barSize = orderDlg.tbBarSize.Text; barSize = "1 Day";
                string whatToShow = orderDlg.tbWhatToShow.Text;
                int useRTH = Int32.Parse(orderDlg.tbRTH.Text, CultureInfo.InvariantCulture);
                int dateFormat = Int32.Parse(orderDlg.tbDateFormat.Text, CultureInfo.InvariantCulture);
                ibClient.reqHistoricalData(requestId, contract, endDateTime, duration, barSize, whatToShow, useRTH, dateFormat);
            }
        }


        private void btnCancelHistData_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.cancelHistoricalData(requestId);
            }
        }


        private void btnRTBars_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                // BarDataType whatToShow = (BarDataType)EnumDescConverter.GetEnumValue
                //    (typeof(BarDataType), orderDlg.tbWhatToShow.Text);
                string whatToShow = orderDlg.tbWhatToShow.Text;
                
                bool useRTH = Boolean.Parse(orderDlg.tbRTH.Text);

                // Only 5 second bars are supported
                int barSize = 5;
                ibClient.reqRealTimeBars(requestId, contract, barSize, whatToShow, useRTH);
            }
        }

        private void btnCancelRTBars_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.cancelRealTimeBars(requestId);
            }
        }

        private void btnTime_Click(object sender, EventArgs e)
        {
            ibClient.reqCurrentTime();
        }

        private void btnMarketScanner_Click(object sender, EventArgs e)
        {
            ibClient.reqScannerParameters();
            // ibClient.reqScannerSubscription();       // subscribe
            // ibClient.cancelScannerSubscription();    // unsubscribe
        }

        private void btnWhatIf_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Order order = orderDlg.getOrder();
                order.WhatIf = true;
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.placeOrder(requestId, contract, order);
            }
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Order order = orderDlg.getOrder();
                order.WhatIf = false;
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.placeOrder(requestId, contract, order);
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.cancelOrder(requestId);
            }
        }

        private void btnExerciseOptions_Click(object sender, EventArgs e)
        {
            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Order order = orderDlg.getOrder();
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);
                int action = Int32.Parse(orderDlg.tbAction.Text, CultureInfo.InvariantCulture);
                int quantity = Int32.Parse(orderDlg.tbNumOfContracts.Text, CultureInfo.InvariantCulture);
                int over = Int32.Parse(orderDlg.tbOverride.Text, CultureInfo.InvariantCulture);

                ibClient.exerciseOptions(requestId, contract, action, quantity, order.Account, over);
            }
        }

        private void btnExtended_Click(object sender, EventArgs e)
        {
            // Extended Order
        }

        private void btnReqContractData_Click(object sender, EventArgs e)
        {
            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                Contract contract = orderDlg.getContract();

                requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);

                ibClient.reqContractDetails(requestId, contract);
            }
        }

        private void reqOpenOrder_Click(object sender, EventArgs e)
        {
            ibClient.reqOpenOrders();
        }

        private void btnReqAllOpenOrders_Click(object sender, EventArgs e)
        {
            ibClient.reqAllOpenOrders();
        }

        // request to automatically bind any newly entered TWS orders
        // to this API client. NOTE: TWS orders can only be bound to
        // client's with clientId=0.
        private void btnReqAutoOpenOrders_Click(object sender, EventArgs e)
        {
            ibClient.reqAutoOpenOrders(true);
        }

        private void btnReqAccountData_Click(object sender, EventArgs e)
        {
            bool subscribe = true;
            String acctCode = null;
            ibClient.reqAccountUpdates(subscribe, acctCode);
        }

        private void btnReqExecution_Click(object sender, EventArgs e)
        {
            ExecutionFilter filter = new ExecutionFilter();

            requestId = Int32.Parse(orderDlg.tbRequestId.Text, CultureInfo.InvariantCulture);
            ibClient.reqExecutions(requestId, filter);
        }

        private void btnReqNextId_Click(object sender, EventArgs e)
        {
            ibClient.reqIds(20);
        }

        private void btnNewsBulletins_Click(object sender, EventArgs e)
        {
            // ibClient.reqNewsBulletins(true);
            // ibClient.cancelNewsBulletins();
        }

        private void btnLogConfig_Click(object sender, EventArgs e)
        {
            // 5 = detail
            ibClient.setServerLogLevel(5);
        }

        private void btnReqAccounts_Click(object sender, EventArgs e)
        {
            ibClient.reqManagedAccts();
        }

        private void btnFA_Click(object sender, EventArgs e)
        {
            // ibClient.requestFA();
            // ibClient.replaceFA();
        }

        private void btnGlobalCancel_Click(object sender, EventArgs e)
        {
            ibClient.reqGlobalCancel();
        }

        private void btnReqMktDataType_Click(object sender, EventArgs e)
        {
             

            if (orderDlg.ShowDialog() == DialogResult.OK)
            {
                int type;
                if (orderDlg.comboMarketDataType.Text == "REALTIME")
                    type = 1;
                else
                    type = 2;
                ibClient.reqMarketDataType(type);
            }
        }

        #region IB Incoming Msg
        public virtual void tickPrice(int tickerId, int field, double price, int canAutoExecute)
        {
            String str;
            str = "TickPrice @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + tickerId.ToString()
                + ", tickType = " + field.ToString()
                + ", Price = " + price.ToString();
            AddTextToDataListWindow(str);
        }

        public virtual void tickSize(int tickerId, int field, int size)
        {
            String str;
            str = "TickSize @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + tickerId.ToString()
                + ", Size Tick Type = " + field.ToString()
                + ", Size = " + size.ToString();
            AddTextToDataListWindow(str);
        }

        public virtual void tickGeneric(int tickerId, int field, double value)
        {
            String str;
            str = "TickGeneric @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + tickerId.ToString()
                + ", Tick Type = " + field.ToString()
                + ", value = " + value.ToString();
            AddTextToDataListWindow(str);
        }

        public virtual void tickString(int tickerId, int tickType, string value)
        {
            String str;
            str = "TickString @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + tickerId.ToString()
                + ", Tick Type = " + tickType.ToString()
                + ", value = " + value;

            AddTextToDataListWindow(str);
        }

        public virtual void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureExpiry, double dividendImpact, double dividendsToExpiry)
        {
            String str;
            str = "TickEfp @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + tickerId.ToString()
                + ", Tick Type = " + tickType.ToString()
                + ", basis points = " + basisPoints.ToString();
            AddTextToDataListWindow(str);
        }

        public virtual void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            String str;
            str = "TickOptionComputation @" + DateTime.Now.ToShortTimeString();

            AddTextToDataListWindow(str);
        }

        // When reqMktData snapshot field is enabled
        public virtual void tickSnapshotEnd(int tickerId)
        {
            String str;
            str = "TickSnapshotEnd @" + DateTime.Now.ToShortTimeString();

            AddTextToDataListWindow(str);
        }

        public virtual void marketDataType(int reqId, int marketDataType)
        {
            //WARN: when we request this, we never send a requestId
            //This is also not returning anything when invoked
            String str;
            str = "market data type: " + DateTime.Now.ToShortTimeString() + ": TickerId = "
                + reqId.ToString() + ", MarketDataType = " + marketDataType.ToString();
            AddTextToResponseListWindow(str);
        }

        // This event is called whenever the status of an order changes. 
        // It is also fired after reconnecting to TWS if the client has any open orders.
        // It also captures cancelled order -- status
        public virtual void orderStatus(int orderId, string status, int filled, int remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
            String str;
            str = "Order Status @ " + DateTime.Now.ToShortTimeString() 
                + ": OrderId = " + orderId.ToString()
                + ", OrderStatus = " + status.ToString()
                + " filled = " + filled.ToString()
                + " remaining = " + remaining.ToString()
                + " avg filled price = " + avgFillPrice.ToString()
                + " perm Id = " + permId.ToString()
                + " parent Id = " + parentId.ToString()
                + " last fill price = " + lastFillPrice.ToString()
                + " client id = " + clientId.ToString()
                + " whyHeld = " + whyHeld;
            AddTextToResponseListWindow(str);
        }

        // This function is called to feed in open orders.
        // // It is also fired after reconnecting to TWS if the client has any open orders.
        public virtual void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            String str;
            str = "Open order @ " + DateTime.Now.ToShortTimeString()
                + ": OrderId = " + orderId.ToString()
                + ", contract symbol = " + contract.Symbol
                + "order = " + order.OrderId.ToString()
                + "client = " + order.ClientId.ToString()
                + " order state = " + orderState.Status.ToString();

            AddTextToResponseListWindow(str);
        }

        public virtual void nextValidId(int orderId)
        {
            String str;
            str = "Next Valid Id = " + orderId.ToString();

            AddTextToResponseListWindow(str);
        }

        // This function is called after a successful connection to TWS.
        private void ibClient_nextValidId(int orderId)
        {
            String str;
            str = "Next Valid Id = " + orderId.ToString();

            AddTextToResponseListWindow(str);
        }

        // trigged by reqAccountUpdates()
        public virtual void updateAccountValue(string key, string value, string currency, string accountName)
        {
            String str;
            str = "Update account value @ " + DateTime.Now.ToShortTimeString()
                + ": key = " + key
                + ", value = " + value
                + "currency = " + currency
                + "account name = " + accountName;

            AddTextToResponseListWindow(str);
        }

        // trigged by reqAccountUpdates()
        public virtual void updatePortfolio(Contract contract, int position, double marketPrice, double marketValue, double averageCost, double unrealisedPNL, double realisedPNL, string accountName)
        {
            String str;
            str = "Update portfolio @ " + DateTime.Now.ToShortTimeString()
                + ": contract = " + contract.Symbol
                + ", position = " + position.ToString()
                + " market price = " + marketPrice.ToString()
                + " market value = " + marketValue.ToString()
                + " avg cost = " + averageCost.ToString()
                + " unrealized PnL = " + unrealisedPNL.ToString()
                + " realized PnL = " + realisedPNL.ToString()
                + " account = " + accountName;

            AddTextToResponseListWindow(str);
        }

        // trigged by reqAccountUpdates()
        public virtual void updateAccountTime(string timestamp)
        {
            AddTextToResponseListWindow("Update account time: " + timestamp);
        }

        // triggered by  reqNewsBulletins()
        public virtual void updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
            String str;
            str = "Update news bulletin @ " + DateTime.Now.ToShortTimeString()
                + ": newsId = " + msgId.ToString()
                + ", news type = " + msgType.ToString()
                + " newsmessage = " + message
                + " originatingExchange = " + origExchange;

            AddTextToResponseListWindow(str);
        }

        // triggered by reqContractDetails()
        public virtual void contractDetails(int reqId, ContractDetails contractDetails)
        {
            String str;
            str = "Contract Details @ " + DateTime.Now.ToShortTimeString()
                + " long name = " + contractDetails.LongName
                + " cusip = " + contractDetails.Cusip
                + " id = " + contractDetails.Summary.ConId
                + " symbol = " + contractDetails.Summary.Symbol
                + " multiplier = " + contractDetails.Summary.Multiplier ;

            AddTextToResponseListWindow(str);
        }

        // This function is called once all contract details for a given request are received. This helps to define the end of an option chain.
        public virtual void contractDetailsEnd(int reqId)
        {
            String str;
            str = "Contract Details  End @ " + DateTime.Now.ToShortTimeString()
                + " reqId = " + reqId.ToString();

            AddTextToResponseListWindow(str);
        }

        // This function is called only when reqContractDetails function on the EClientSocket object has been called for bonds.
        /* 
        void ibClient_bondContractDetails(int reqId, ContractDetails contract)
        {
            String str;
            str = "Bond Contract Details @ " + DateTime.Now.ToShortTimeString()
                + " long name = " + contract.LongName
                + " cusip = " + contract.Cusip
                + " id = " + contract.Summary.ContractId
                + " symbol = " + contract.Summary.Symbol;

            AddTextToResponseListWindow(str);
        }
        */
        public virtual void bondContractDetails(int reqId, ContractDetails contract)
        {
            String str;
            str = "Bond Contract Details @ " + DateTime.Now.ToShortTimeString()
                + " long name = " + contract.LongName
                + " cusip = " + contract.Cusip
                + " id = " + contract.Summary.ConId
                + " symbol = " + contract.Summary.Symbol;

            AddTextToResponseListWindow(str);
        }

        // This event is fired when the reqExecutions() functions is invoked, or when an order is filled.
        public virtual void execDetails(int reqId, Contract contract, Execution execution)
        {
            String str;
            str = "Execution Details @ " + DateTime.Now.ToShortTimeString()
                + " req id = " + reqId.ToString()
                + " order id = " + execution.OrderId.ToString()
                + " contract = " + contract.Symbol
                + " execution client id = " + execution.ClientId.ToString()
                + " exeuction id = " + execution.ExecId.ToString()
                + " exeuction shares = " + execution.Shares.ToString();

            AddTextToResponseListWindow(str);
        }

        // This function is called once all executions have been sent to a client in response to reqExecutions()
        public virtual void execDetailsEnd(int reqId)
        {
            String str;
            str = "Execution details end @ " + DateTime.Now.ToShortTimeString()
                + " req id = " + reqId.ToString();

            AddTextToResponseListWindow(str);
        }

        // triggered by reqMktDepth()
        public virtual void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            String str;
            str = "update market depth @ " + DateTime.Now.ToShortTimeString()
                + " id = " + tickerId.ToString()
                + " position = " + position.ToString()
                + " market depth operation = " + operation.ToString()
                + " market depth side = " + side.ToString()
                + " price = " + price.ToString()
                + " size = " + size.ToString();

            AddTextToDataListWindow(str);
        }

        // triggered by reqMktDepth()
        public virtual void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            String str;
            str = "update market depth L2 @ " + DateTime.Now.ToShortTimeString()
                + " id = " + tickerId.ToString()
                + " position = " + position.ToString()
                + " market maker = " + marketMaker
                + " market depth operation = " + operation.ToString()
                + " market depth side = " + side.ToString()
                + " price = " + price.ToString()
                + " size = " + size.ToString();

            AddTextToDataListWindow(str);
        }

        // This function is called when a successful connection is made to a Financial Advisor account. 
        // It is also called when the reqManagedAccts() function is invoked.
        // It is also called when tws is connected.
        public virtual void managedAccounts(string accountsList)
        {
            String str;
            str = "Accounts = " + accountsList;

            AddTextToResponseListWindow(str);
        }

        // trigged by requestFA()
        public virtual void receiveFA(int faDataType, string faXmlData)
        {
            String str;
            str = "receiveFA @ " + DateTime.Now.ToShortTimeString();

            AddTextToResponseListWindow(str);
        }

        // This function receives the requested historical data results.
        public virtual void historicalData(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
            String str;
            str = "Hist. Data @" + DateTime.Now.ToShortTimeString() + ": TickerId = " + reqId.ToString()
                + ", Date = " + date
                + ", O = " + open.ToString()
                + ", H = " + high.ToString()
                + ", L = " + low.ToString()
                + ", C = " + close.ToString()
                + ", V = " + volume.ToString()
                + ", barCount = " + count.ToString()
                + ", WAP = " + WAP.ToString()
                + ", hasGaps = " + hasGaps.ToString();

            AddTextToDataListWindow(str);
        }

        // This function is called when the snapshot is received and marks the end of one scan.
        public virtual void scannerDataEnd(int reqId)
        {
            String str;
            str = "scanner data end @ " + DateTime.Now.ToShortTimeString();

            AddTextToResponseListWindow(str);
        }

        // This function receives the requested market scanner data results.
        public virtual void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            String str;
            str = "scanner data @ " + DateTime.Now.ToShortTimeString();

            AddTextToResponseListWindow(str);
        }

        // This function receives an XML document that describes the valid parameters that a scanner subscription can have.
        public virtual void scannerParameters(string xml)
        {
            String str;
            str = "scanner parameters @ " + DateTime.Now.ToShortTimeString();

            AddTextToResponseListWindow(str);
        }

        // This function receives the real-time bars data results.
        public virtual void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            String str;
            str = "real time bar @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + reqId.ToString()
                + ", time = " + time.ToString()
                + ", O = " + open.ToString()
                + ", H = " + high.ToString()
                + ", L = " + low.ToString()
                + ", C = " + close.ToString()
                + ", V = " + volume.ToString()
                + ", barCount = " + count.ToString()
                + ", WAP = " + WAP.ToString();

            AddTextToDataListWindow(str);
        }

        // This function is called to receive Reuters global fundamental market data. 
        public virtual void fundamentalData(int reqId, string data)
        {
            String str;
            str = "Fndmntl. Data @" + DateTime.Now.ToShortTimeString()
                + ": TickerId = " + reqId.ToString()
                + ", Data = " + data;

            AddTextToDataListWindow(str);
        }

        #endregion

        #region other
        public virtual void accountDownloadEnd(string account)
        {
            String str;
            str = "Account Download End: " + account;

            AddTextToResponseListWindow(str);
        }

        public virtual void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            String str;
            str = "Account Summary " + account;

            AddTextToDataListWindow(str);
        }

        public virtual void accountSummaryEnd(int reqId)
        {
            String str;
            str = "Account Summary End, request Id = " + reqId;

            AddTextToDataListWindow(str);
        }

        public virtual void commissionReport(CommissionReport commissionReport)
        {
            String str;
            str = "CommissionReport, id = " + commissionReport.ExecId
                + " commision = " + commissionReport.Commission;

            AddTextToResponseListWindow(str);
        }

        public virtual void connectionClosed()
        {
            String str;
            str = "connection closed";

            AddTextToResponseListWindow(str);
        }

        public virtual void position(string account, Contract contract, int pos)
        {
            String str;
            str = "Position, symbol = " + contract.Symbol
                + ", SecType: " + contract.SecType
                + ", Currency: " + contract.Currency
                + ", Position: " + pos;

            AddTextToResponseListWindow(str);
        }

        public virtual void position(string account, Contract contract, int pos, double avgCost)
        {
            String str;
            str = "Position, symbol = " + contract.Symbol
                + ", SecType: " + contract.SecType
                + ", Currency: " + contract.Currency
                + ", Position: " + pos.ToString()
                + ", AverageCost: " + avgCost.ToString();

            AddTextToResponseListWindow(str);
        }

        public virtual void positionEnd()
        {
            String str;
            str = "Position End";

            AddTextToResponseListWindow(str);
        }

        public virtual void openOrderEnd()
        {
            String str;
            str = "Open Order End";

            AddTextToResponseListWindow(str);
        }

        public virtual void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            String str;
            str = "Historical Data End";

            AddTextToResponseListWindow(str);
        }

        public virtual void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            String str;
            str = "DeltaNeutralValidation. " + reqId + ", ConId: " + underComp.ConId + ", Delta: " + underComp.Delta + ", Price: " + underComp.Price;

            AddTextToResponseListWindow(str);
        }
        #endregion
    }
}
