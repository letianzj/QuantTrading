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

using System.ComponentModel;

namespace TradingBase
{
    /// <summary>
    /// Derive own strategy from StategyBase
    /// public class MyStrategy : StrategyBase
    /// </summary>
    public class StrategyBase : StrategyInterface
    {

        #region input messages received by strategy

        /// <summary>
        /// Called when new ticks are recieved
        /// here is where you respond to ticks, eg to populate a barlist
        /// this.MyBarList.newTick(tick);
        /// </summary>
        /// <param name="tick"></param>
        public virtual void GotTick(Tick k)
        {
        }
        /// <summary>
        /// Called when new orders received
        /// track or respond to orders here, eg:
        /// this.MyOrders.Add(order);
        /// </summary>
        /// <param name="order"></param>
        public virtual void GotOrder(Order o)
        {
        }
        /// <summary>
        /// Called when orders are filled as trades.
        /// track or respond to trades here, eg:
        /// positionTracker.Adjust(fill);
        /// </summary>
        /// <param name="fill"></param>
        public virtual void GotFill(Trade f)
        {
        }
        /// <summary>
        /// Called if a cancel has been processed
        /// </summary>
        /// <param name="cancelid"></param>
        public virtual void GotOrderCancel(long id)
        {

        }
        /// <summary>
        /// called when a position update is received (usually only when the strategy is initially loaded)
        /// </summary>
        /// <param name="p"></param>
        public virtual void GotPosition(Position p)
        {
        }
        /// <summary>
        /// called when historical bar arrives.   
        /// </summary>
        public virtual void GotHistoricalBar(Bar b)
        {
        }
        #endregion

        #region strategy output
        // strategy decisions to send out
        public event Action<string> SendDebugEvent;
        // int = strategy id who sends this order
        public event Action<Order, int> SendOrderEvent;
        // order id and strategy id
        public event Action<long, int> SendCancelEvent;
        // int = strategy id
        public event Action<Basket, int> SendBasketEvent;
        // int = strategy id
        public event Action<int> SendMarketDepthEvent;
        public event Action<BarRequest> SendReqHistBarEvent;
        // decimal price, int time, string label, and color
        public event Action<decimal, int, string, System.Drawing.Color> SendChartLabelEvent;
        // StrategyBase id and indicator string
        public event Action<int, string> SendIndicatorsEvent;

        /// <summary>
        /// sends a debug message about what your strategy is doing at the moment.
        /// </summary>
        /// <param name="msg"></param>
        public virtual void SendDebug(string msg) { if (SendDebugEvent != null) SendDebugEvent(msg); }
        /// <summary>
        /// sends an order
        /// </summary>
        /// <param name="o"></param>
        public virtual void SendOrder(Order o) 
        { 
            o.VirtualOwner = ID; 
            if (SendOrderEvent != null) 
                SendOrderEvent(o, ID); 
        }
        /// <summary>
        /// cancels an order (must have the id)
        /// </summary>
        /// <param name="id">order id</param>
        public virtual void SendCancel(long id) { if (SendCancelEvent != null) SendCancelEvent(id, ID); }

        /// <summary>
        /// requests ticks for a basket of securities
        /// </summary>
        /// <param name="syms"></param>
        public virtual void SendBasket(string[] syms)
        {
            if (SendBasketEvent != null)
                SendBasketEvent(new Basket(syms), ID);
            else
                SendDebug("SendBasket not supported in this application.");
        }
        /// <summary>
        /// requests market depth for the baasket
        /// </summary>
        /// <param name="syms"></param>
        public virtual void SendMarketDepthRequest(int depth)
        {
            if (SendMarketDepthEvent != null)
                SendMarketDepthEvent(depth);
            else
                SendDebug("SendMarketDepth not supported in this application.");
        }

        /// <summary>
        /// send historical bar request
        /// </summary>
        public virtual void SendHistoricalBarRequest(BarRequest br)
        {
            if (SendReqHistBarEvent != null)
                SendReqHistBarEvent(br);
            else
                SendDebug("SendreqHistBar is not supported in this application");
        }

        /// <summary>
        /// draws a label with default color (violet)
        /// </summary>
        public virtual void SendChartLabel(decimal price, int time, string text) { if (SendChartLabelEvent != null) SendChartLabelEvent(price, time, text, System.Drawing.Color.Purple); }
        /// <summary>
        /// draws text directly on a point on chart
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        /// <param name="text"></param>
        public virtual void SendChartLabel(decimal price, int time, string text, System.Drawing.Color c) { if (SendChartLabelEvent != null) SendChartLabelEvent(price, time, text, c); }
        /// <summary>
        /// draws line with default color (orage)
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        public virtual void SendChartLabel(decimal price, int time) { SendChartLabel(price, time, null, System.Drawing.Color.Orange); }

        /// <summary>
        /// send indicators as array of strings for later analysis
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void SendIndicators(string[] indicators) { if (SendIndicatorsEvent != null) SendIndicatorsEvent(ID, string.Join(",", indicators)); }
        /// <summary>
        /// sends indicators as a comma seperated string (for later analsis)
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void SendIndicators(string indicators) { if (SendIndicatorsEvent != null) SendIndicatorsEvent(ID, indicators); }

        #endregion

        #region control
        // An empty constructor is required for derived class
        public StrategyBase() { }
        /// <summary>
        /// Call this to reset your strategy parameters.
        /// You might need to reset groups of indicators or internal counters.
        /// eg : MovingAverage = 0;
        /// Note that the control parameter such as LookbackPeriod should be set  only once at initialization.
        /// </summary>
        public virtual void Reset(bool popup = true)
        {
        }

        /// <summary>
        /// Call this to shutdown your strategy
        /// </summary>
        /// <example>
        /// D("shutting down everything");
        /// foreach (Position p in pt)
        /// sendorder(new MarketOrderFlat(p));
        /// isactive = false;
        /// </example>
        public virtual void Shutdown()
        {
            _isactive = false;
        }
        #endregion

        #region strategy information
        protected int _id = int.MaxValue;
        protected string _name = "";
        protected string _fullname = "";
        protected bool _isactive = true;
        protected string[] _inds = new string[0];
        // Order Id and strategy id tracker
        protected IdTracker _idtracker;

        protected List<string> _symbols = new List<string>();
        [Description("Symbols of interest")]
        public List<string> Symbols { get { return _symbols; } set { _symbols = value; } }

        /// <summary>
        /// numeric tag for this strategy used by programs that load strategies
        /// </summary>
        public int ID { get { return _id; } set { _id = value; } }
        /// <summary>
        /// Custom name of strategy set by you
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// Full name of this strategy set by programs (includes namespace)
        /// </summary>
        public string FullName { get { return _fullname; } set { _fullname = value; } }

        /// <summary>
        /// Whether strategy can be used or not
        /// </summary>
        public bool IsActive { get { return _isactive; } set { _isactive = value; } }
        /// <summary>
        /// Names of the indicators used by your strategy.
        /// Length must correspond to actual indicator values send with SendIndicators event
        /// </summary>
        public string[] Indicators { get { return _inds; } set { _inds = value; } }

        /// <summary>
        /// This must be called in the main program
        /// </summary>
        /// <param name="tracker"></param>
        public void SetIdTracker(IdTracker tracker)
        {
            _idtracker = tracker;
        }
        #endregion
    }
}
