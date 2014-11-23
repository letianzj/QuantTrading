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
using TradingBase;
using TechnicalAnalysisEngine;

namespace ClassicStrategies
{
    public class SMASimple : StrategyBase
    {
        #region parameters
        decimal _totalprofit = 500;
        [Description("Total profit target")]
        public decimal TotalProfitTarget { get { return _totalprofit; } set { _totalprofit = value; } }

        decimal _totalloss = -200;
        [Description("Total loss tolerance")]
        public decimal TotalLossTolerance { get { return _totalloss; } set { _totalloss = value; } }

        int _transactions = 0;
        int _totaltransactions = 6;
        [Description("Total transactions per day")]
        public int TotalTransactions { get { return _totaltransactions; } set { _totaltransactions = value; } }

        int _tradesize = 100;
        [Description("Entry/Exit trade size")]
        public int TradeSize { get { return _tradesize; } set { _tradesize = value; } }

        int _barinterval = 300;
        [Description("Default bar interval")]
        public int BarInterval { get { return _barinterval; } set { _barinterval = value; } }

        int _barslookback = 14;
        public int BarsLookback { get { return _barslookback; } set { _barslookback = value; } }

        int _shutdowntime = 155500;
        [Description("System shutdown time")]
        public int ShutdownTime { get { return _shutdowntime; } set { _shutdowntime = value; } }

        //List<string> _symbols = new List<string>();//{ "AAPL STK SMARK", "SPY STK SMART" };
        //[Description("Symbols of interest")]
        //public List<string> Symbosl { get { return _symbols; } set { _symbols = value; } }
        #endregion

        #region member variables
        private PositionTracker _positiontracker;
        private BarListTracker _barlisttracker;
        bool[] _issymbolactive;
        bool[] _waittobefilled;
        int _currenttime = 0;       // keep track of time
        #endregion

        public SMASimple()
        {
            _name = "SMA Simple Crossover";
            Indicators = new string[] { "Time", "SMA" };

            _symbols.Add("AAPL STK SMART"); _symbols.Add("SPY STK SMART");
        }

        void _barlisttracker_GotNewBar(string sym, int interval)
        {
            //int idx = Array.IndexOf(_symbols, sym);
            int idx = _symbols.IndexOf(sym);

            if (_isactive && _issymbolactive[idx])
            {
                BarList bl = _barlisttracker[sym, interval];

                double[] closes = Calc.Decimal2Double(bl.Close());
                // when NewBar is triggered, the latest bar only has one tick
                closes = closes.Take(closes.Length - 1).ToArray();

                SMAResult result = AnalysisEngine.SMA(closes, _barslookback);

                if (result.Values.Count == 0)           // Not enough bars
                    return;

                // check
                if (!_waittobefilled[idx])
                {
                    double sma = result.Values.Last();
                    if (_positiontracker[sym].isFlat)
                    {
                        
                        // if our current price is above SMA
                        if (closes.Last() > sma)
                        {
                            SendDebug("Cross above SMA, buy");
                            SendOrder(new MarketOrder(sym, _tradesize));
                            _waittobefilled[idx] = true;
                        }
                        // if our current price is below SMA
                        else if (closes.Last() < sma)
                        {
                            SendDebug("Cross below SMA, sell");
                            SendOrder(new MarketOrder(sym, -_tradesize));
                            _waittobefilled[idx] = true;
                        }
                    }
                    else if ( (_positiontracker[sym].isLong && (closes.Last() < sma))
                        || (_positiontracker[sym].isShort && (closes.Last() > sma)) )
                    {
                        SendDebug("Counter trend, exit.");
                        SendOrder(new MarketOrderFlat(_positiontracker[sym]));
                        _waittobefilled[idx] = true;
                    }

                    // this way we can debug our indicators during development
                    // indicators are sent in the same order as they are named above
                    if (_waittobefilled[idx])
                        SendIndicators(new string[] { "Time: "+_currenttime.ToString(), " SMA:"+sma.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) });
                }
            }
        }

        public override void GotPosition(Position p)
        {
            if (_symbols.Contains(p.FullSymbol))
                _positiontracker.Adjust(p);
        }

        public override void GotFill(Trade f)
        {
            if (_symbols.Contains(f.FullSymbol))
            {
                _positiontracker.Adjust(f);
                //int idx = Array.IndexOf(_symbols, f.FullSymbol);
                int idx = _symbols.IndexOf(f.FullSymbol);
                _waittobefilled[idx] = false;
                
                _transactions++;
                if (_transactions >= _totaltransactions)
                {
                    Shutdown();
                }
            }
        }

        public override void GotOrder(Order o)
        {
            SendDebug("order accepted: " + o.Id);
        }

        public override void GotOrderCancel(long id)
        {
            SendDebug("Order canceled: " + id);
        }

        public override void GotTick(Tick k)
        {
            _currenttime = k.Time;
            if (_currenttime > _shutdowntime)
            {
                Shutdown();
                return;
            }

            _barlisttracker.NewTick(k);     // blt rejects symbols that it doesn't track

            // ignore anything that is not a trade
            if (!k.IsTrade) return;
            // exit conditon
            if ((Calc.OpenPL(k.TradePrice, _positiontracker[k.FullSymbol]) + _positiontracker[k.FullSymbol].ClosedPL > TotalProfitTarget) ||
                    (Calc.OpenPL(k.TradePrice, _positiontracker[k.FullSymbol]) + _positiontracker[k.FullSymbol].ClosedPL < TotalLossTolerance))
            {
                Shutdown(k.FullSymbol);
            }
        }

        /// <summary>
        /// This is called after StrategySetter updates _symbol list
        /// </summary>
        /// <param name="popup"></param>
        public override void Reset(bool popup = true)
        {
            _positiontracker = new PositionTracker(_symbols.Count);
            _barlisttracker = new BarListTracker(_symbols.ToArray(), _barinterval);
            _barlisttracker.GotNewBar += _barlisttracker_GotNewBar;

            // all are active
            _issymbolactive = Enumerable.Repeat<bool>(true, _symbols.Count).ToArray();
            // none are filled
            _waittobefilled = Enumerable.Repeat<bool>(false, _symbols.Count).ToArray();

            _transactions = 0;
        }

        readonly object _lockobj = new object();
        public void Shutdown(string sym)
        {
            lock (_lockobj)
            {
                //int idx = Array.IndexOf(_symbols, sym);
                int idx = _symbols.IndexOf(sym);

                if (idx > -1)
                {
                    if (!_issymbolactive[idx])
                        return;

                    if (!_positiontracker[sym].isFlat)
                    {
                        SendDebug("Shutting down " + sym);
                        SendOrder(new MarketOrderFlat(_positiontracker[sym]));
                    }

                    _issymbolactive[idx] = false;
                }
            }                
        }
        public override void Shutdown()
        {
            lock (_lockobj)
            {
                if (!_isactive)
                    return;

                // flat positions
                SendDebug("Shutting down strategy " + _name);
                foreach (Position p in _positiontracker)
                {
                    if (!p.isFlat)
                        SendOrder(new MarketOrderFlat(p));
                }

                // set inactive
                _isactive = false;
            }
        }
    }
}
