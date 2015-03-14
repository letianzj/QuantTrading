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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBase
{
    public interface IClient
    {
        /// Properties
        long ServerTime { get; }
        string Account { get; }
        int ClientId { get; set; }
        long NextValidOrderId { get; }
        bool RequestPositionsOnStartup { get; set; }
        int ServerVersion { get; }
        bool isServerConnected { get; }

        void Connect();
        void Disconnect();

        // Outgoing Messages
        void RequestMarketData(Basket b);
        void RequestMarketDepth(int depth);
        void CancelMarketData();         // Cancel all the market data and depth requests
        void RequestHistoricalData(BarRequest br, bool useRTH = false);
        void PlaceOrder(Order o);
        void CancelOrder(long strategyOrderId);

        /// Delegates triggered by incoming messages
        event Action<Tick> GotTickDelegate;
        event Action<Trade> GotFillDelegate;
        event Action<Order> GotOrderDelegate;
        event Action<long> GotOrderCancelDelegate;
        event Action<Position> GotPositionDelegate;
        event Action<Bar> GotHistoricalBarDelegate;
        event Action<string> GotServerInitializedDelegate;
        event Action<string> SendDebugEventDelegate;
    }

    public enum ClientName
    {
        None = 0,
        IB = 1,
        GOOG = 2
    };
}
