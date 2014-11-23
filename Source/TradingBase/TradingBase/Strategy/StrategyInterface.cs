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
    public interface StrategyInterface
    {
        // input messages received by strategy
        void GotTick(Tick tick);
        void GotOrder(Order order);
        void GotFill(Trade fill);
        void GotOrderCancel(long orderid);
        void GotPosition(Position pos);
        void GotHistoricalBar(Bar b);

        // strategy decisions to send out
        event Action<string> SendDebugEvent;
        // int = strategy id who sends this order
        event Action<Order, int> SendOrderEvent;
        // order id and strategy id
        event Action<long, int> SendCancelEvent;
        // int = strategy id
        event Action<Basket, int> SendBasketEvent;
        // int = strategy id
        event Action<int> SendMarketDepthEvent;
        event Action<BarRequest> SendReqHistBarEvent;
        // decimal price, int time, int label, and color
        event Action<decimal, int, string, System.Drawing.Color> SendChartLabelEvent;
        // StrategyBase id and indicator string
        event Action<int, string> SendIndicatorsEvent;

        // control
        void Reset(bool popup = true);
        void Shutdown();

        // StrategyBase Information
        int ID { get; set; }
        /// <summary>
        /// give a name to the strategy; by default is the type.Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Reserved: used for loading from dll; it is type.FullName
        /// </summary>
        string FullName { get; set; }
        bool IsActive { get; set; }
        string[] Indicators { get; set; }
    }
}
