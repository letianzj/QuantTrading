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
    /// Backtest a given instantiated strategy
    /// Single process Backtest engine (vs optimization)
    /// Read data via MultiSim, feed data to Broker. 
    /// Broker then passes data to strategy and arranges orders from strategy.
    /// </summary>
    public class BacktestEngine
    {
        protected StrategyBase _strategy;
        protected MultiSim _histsim;
        protected Broker _simbroker;

        protected List<string> _tickfiles = new List<string>();        

        protected bool _isbusy = false;
        public bool IsBusy { get { return _isbusy; } }

        // historical tick date
        protected int _date = 0;
        // historical tick time
        protected int _time = 0;

        protected string _dps = "N2";
        public string Dps {get {return _dps;} set {_dps = value;}}
        protected bool _usebidaskfill = false;
        public bool UseBidAskFill { get { return _usebidaskfill; } set { _usebidaskfill = value; } }
        protected bool _adjustincomingticksize = false;
        public bool AdjustIncomingTickSize { get { return _adjustincomingticksize; } set { _adjustincomingticksize = value; } }

        // run backtest
        protected BackgroundWorker _playthread = new BackgroundWorker();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="strategy">strategy has to be initiated by property setter</param>
        /// <param name="tickfiles"></param>
        public BacktestEngine(StrategyBase strategy, List<string> tickfiles)
        {
            _tickfiles = tickfiles;
            _strategy = strategy;

            _playthread.DoWork += new DoWorkEventHandler(Play);
            _playthread.WorkerReportsProgress = false;
            _playthread.WorkerSupportsCancellation = true;
            _playthread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(PlayComplete);

            Reset();           
        }

        /// <summary>
        /// Reset histsim and simbroker
        /// </summary>
        public virtual void Reset()
        {
            try
            {
                // try unbind, swallow exception
                UnbindStrategy(ref _strategy);

                if (_strategy != null)
                    _strategy.Reset();

                _histsim = new MultiSim(_tickfiles.ToArray());
                _histsim.Initialize();
                _histsim.GotTickHandler += new Action<Tick>(_histsim_GotTick);
                _histsim.GotDebugHandler += new Action<string>(_histsim_GotDebugHandler);

                _simbroker = new Broker();
                _simbroker.UseBidAskFills = _usebidaskfill;
                _simbroker.AdjustIncomingTickSize = _adjustincomingticksize;
                _simbroker.GotOrderHandler += new Action<Order>(_broker_GotOrder);
                _simbroker.GotOrderCancelBroadcastHandler += new Action<string, bool, long>(_broker_GotOrderCancel);
                _simbroker.GotFillHandler += new Action<Trade>(_broker_GotFill);

                BindStrategy(ref _strategy);

                _isbusy = false;
            }

            catch
            {
                _isbusy = true;
                Debug("Backtest engine initialization failed.");
            }  
        }

        /// <summary>
        /// Play to next timespan
        /// </summary>
        /// <param name="timespaninseconds"></param>
        public void Start(int timespaninseconds)
        {
            if (_playthread.IsBusy)
                return;

            // run in thread
            _playthread.RunWorkerAsync(timespaninseconds);
            Status("Playing next " + timespaninseconds.ToString() + " seconds ...");
        }

        public void Stop()
        {
            try
            {
                _histsim.Stop();

                if (_playthread.IsBusy)
                    _playthread.CancelAsync();
            }
            catch { }
        }

        protected virtual void Play(object sender, DoWorkEventArgs e)
        {
            _isbusy = true;
            int timespaninseconds = (int)e.Argument;

            int time = (int)(_histsim.NextTickTime % 100000);
            long date = (_histsim.NextTickTime / 100000) * 100000;

            long _playto = 0;

            if (timespaninseconds == int.MaxValue)
                _playto = MultiSim.ENDSIM;
            else
            {
                _playto = date + Util.IntTimeAdd(time, timespaninseconds);
            }

            _histsim.PlayTo(_playto);
            _isbusy = false;
        }

        protected void PlayComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug(e.Error.Message + e.Error.StackTrace);
                Status("Terminated because of an Exception.  See messages.");
                SendPlayComplete(1);            // 1 is error
            }
            else if (e.Cancelled)
            {
                Status("Canceled play.");
                SendPlayComplete(-1);            // -1 is cancelled
            }
            else
            {
                Status("Reached time " + _time.ToString());
                SendPlayComplete(0);             // 0 is ok
            }
        }

        #region histsim handlers
        void _histsim_GotTick(Tick t)
        {
            _date = t.Date;
            _time = t.Time;

            /// Note that for stocks, the total size is 100* that in tick file
            if (t.FullSymbol.Contains("STK"))
            {
                t.BidSize *= 100;
                t.AskSize *= 100;
                t.TradeSize *= 100;
            }

            // execute pending orders
            _simbroker.Execute(t);

            // notify strategy
            if (_strategy != null)
                _strategy.GotTick(t);

            // tell others, e.g. TickTable
            GotTick(t);
        }

        void _histsim_GotDebugHandler(string obj)
        {
            Debug(obj);
        }

        void UnbindSim(ref MultiSim h)
        {
            h.GotTickHandler -= new Action<Tick>(_histsim_GotTick);
            h.GotDebugHandler -= new Action<string>(_histsim_GotDebugHandler);
        }

        public virtual void UnbindEvents()
        {
            UnbindStrategy(ref _strategy);
            UnbindSim(ref _histsim);
        }
        #endregion

        #region bind broker
        void _broker_GotOrder(Order o)
        {
            _strategy.GotOrder(o);

            // tell others, e.g., OrderTable
            GotOrder(o);
        }

        void _broker_GotOrderCancel(string sym, bool side, long id)
        {
            if (_strategy != null)
                _strategy.GotOrderCancel(id);
        }

        void _broker_GotFill(Trade f)
        {
            if (_strategy != null)
                _strategy.GotFill(f);

            // tell others, e.g. FillTable
            GotFill(f);
        }
        #endregion

        #region binding strategy
        void BindStrategy(ref StrategyBase s)
        {
            s.SendOrderEvent += _strategy_SendOrder;
            s.SendCancelEvent += _strategy_SendCancel;
            s.SendDebugEvent += _strategy_SendDebugEvent;
            s.SendIndicatorsEvent += _strategy_SendIndicatorsEvent;
            s.SendChartLabelEvent += _strategy_SendChartLabelEvent;
            s.SendBasketEvent += _strategy_SendBasketEvent;
        }

        void UnbindStrategy(ref StrategyBase s)
        {
            try
            {
                s.SendOrderEvent -= _strategy_SendOrder;
                s.SendCancelEvent -= _strategy_SendCancel;
                s.SendDebugEvent -= _strategy_SendDebugEvent;
                s.SendIndicatorsEvent -= _strategy_SendIndicatorsEvent;
                s.SendChartLabelEvent -= _strategy_SendChartLabelEvent;
                s.SendBasketEvent -= _strategy_SendBasketEvent;
            }
            catch { }
        }

        void _strategy_SendOrder(Order o, int id)
        {
            // override order time to historical time
            //if (o.OrderTime == 0)
            //{
                o.OrderDate = _date;
                o.OrderTime = _time;
            //}

            _simbroker.SendOrder(o);
        }

        void _strategy_SendCancel(long number, int id)
        {
            _simbroker.CancelOrder(number);
        }

        void _strategy_SendDebugEvent(string msg)
        {
            Debug(string.Format("{0}: {1}{2}", _time.ToString(), msg, Environment.NewLine));
        }

        void _strategy_SendIndicatorsEvent(int idx, string data)
        {
            if (_strategy == null) return;
            if (_strategy.Indicators.Length == 0)
                Debug("No indicators defined on strategy: " + _strategy.Name);
            else
            {
                // send out indicator
                string[] dataarr = new string[] { _strategy.ID.ToString(), _strategy.Name, data };
                GotIndicator(string.Join(";", dataarr));
            }
        }

        void _strategy_SendChartLabelEvent(decimal price, int time, string label, System.Drawing.Color c)
        {

        }
        void _strategy_SendBasketEvent(Basket b, int id)
        {

        }
        #endregion        

        #region outgoing
        public event Action<int> SendPlayProgressEvent;
        protected void SendPlayProgress(int progress)
        {
            if (SendPlayProgressEvent != null)
                SendPlayProgressEvent(progress);
        }

        public event Action<int> SendPlayCompleteEvent;
        protected void SendPlayComplete(int indicator)
        {
            if (SendPlayCompleteEvent != null)
                SendPlayCompleteEvent(indicator);
        }
        public event Action<string> SendEngineDebugEvent;
        protected void Debug(string msg)
        {
            if (SendEngineDebugEvent != null)
                SendEngineDebugEvent(msg);
        }

        public event Action<string> SendEngineStatusEvent;
        protected void Status(string msg)
        {
            if (SendEngineStatusEvent != null)
                SendEngineStatusEvent(msg);
        }

        public event Action<Tick> GotTickEvent;
        void GotTick(Tick k)
        {
            if (GotTickEvent != null)
                GotTickEvent(k);
        }

        public event Action<Order> GotOrderEvent;
        void GotOrder(Order o)
        {
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }

        public event Action<Trade> GotFillEvent;
        void GotFill(Trade f)
        {
            if (GotFillEvent != null)
                GotFillEvent(f);
        }

        public event Action<string> GotIndicatorEvent;
        void GotIndicator(string data)
        {
            if (GotIndicatorEvent != null)
                GotIndicatorEvent(data);
        }
        #endregion
    }
}
