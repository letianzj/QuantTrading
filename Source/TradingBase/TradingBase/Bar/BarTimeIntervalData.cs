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
    /// <summary>
    /// Store OHLCV data of a list of bars
    /// </summary>
    public class BarTimeIntervalData : BarIntervalData
    {
        public List<decimal> Open() { return opens; }
        public List<decimal> Close() { return closes; }
        public List<decimal> High() { return highs; }
        public List<decimal> Low() { return lows; }
        public List<long> Vol() { return vols; }
        public List<int> Date() { return dates; }
        public List<int> BarOrder() { return orders; }
        public bool IsRecentNew() { return _isRecentNew; }
        public int Count() { return _Count; }
        public int Last() { return _Count - 1; }


        internal List<decimal> opens = new List<decimal>();
        internal List<decimal> closes = new List<decimal>();
        internal List<decimal> highs = new List<decimal>();
        internal List<decimal> lows = new List<decimal>();
        internal List<long> vols = new List<long>();
        internal List<int> dates = new List<int>();
        internal List<int> orders = new List<int>();
        internal List<long> ids = new List<long>();
        internal int _Count = 0;
        internal bool _isRecentNew = false;
        public static bool isOldTickBackfillEnabled = true;

        long curr_barid = -1;
        int intervallength = 60;            // 60s
        
        public BarTimeIntervalData(int unitsPerInterval)
        {
            intervallength = unitsPerInterval;
        }

        public Bar GetBar(string symbol) 
        { 
            return GetBar(Last(), symbol); 
        }

        public Bar GetBar(int index, string symbol)
        {
            Bar b = new Bar();
            if (index >= _Count) return b;
            else if (index < 0)
            {
                index = _Count - 1 + index;
                if (index < 0) return b;
            }
            b = new Bar(opens[index], highs[index], lows[index], closes[index], vols[index], dates[index], orders[index], symbol, intervallength);
            if (index == Last()) b.IsNew = _isRecentNew;
            return b;
        }

        public void Reset()
        {
            opens.Clear();
            closes.Clear();
            highs.Clear();
            lows.Clear();
            vols.Clear();
            dates.Clear();
            orders.Clear();
            ids.Clear();
            _Count = 0;
        }

        private void NewBar(long id)
        {
            _Count++;
            opens.Add(0);
            closes.Add(0);
            highs.Add(0);
            lows.Add(decimal.MaxValue);
            vols.Add(0);
            orders.Add(0);
            dates.Add(0);
            ids.Add(id);
        }

        public void AddBar(Bar mybar)
        {
            _Count++;
            closes.Add(mybar.Close);
            opens.Add(mybar.Open);
            dates.Add(mybar.Date);
            highs.Add(mybar.High);
            lows.Add(mybar.Low);
            vols.Add(mybar.Volume);
            orders.Add(GetOrder(mybar.BarStartTime,intervallength));
            ids.Add(GetBarId(mybar.BarStartTime, mybar.Date, intervallength));
        }

        public void NewTick(Tick k)
        {
            // ignore quotes
            if (k.TradePrice == 0) return;
            // get the barcount
            long barid = GetBarId(k.Time, k.Date, intervallength);

            int baridx;

            //(barid != curr_barid) // what if ticks arrive a bit out of sequence?
            // Live datafeeds generally have no requirement that tick timestamps are strictly ordered.
            // as Level1 aggregates data from multiple venues, if timestamp is the exchange timestamp,
            // it is quite possible the order is different from aggregator "received" timestamp
            // The datavendors as a rule don't perform "re-sorting" of ticks based on timestamp 
            // to ensure strict ordering as it is both expensive and arguably unnecessary.
            if (barid > curr_barid)  //  true new bar needs to be formed
            {
                // create a new one
                NewBar(barid);
                // mark it
                _isRecentNew = true;
                // make it current
                curr_barid = barid;
                // set time
                orders[orders.Count - 1] = GetOrder(k.Time,intervallength);
                // set date
                dates[dates.Count - 1] = k.Date;

                baridx = Last();
            }
            else if (isOldTickBackfillEnabled && (barid < curr_barid)) // out-of sequence tick, already formed bar needs updating
            {
                baridx = ids.IndexOf(barid);
                _isRecentNew = false;
            }
            else // bar formed; update tick values
            {
                baridx = Last();

                _isRecentNew = false;
            }

            // blend tick into bar
            // open
            if (baridx >= 0)
            {
                if (opens[baridx] == 0) opens[baridx] = k.TradePrice;
                // high
                if (k.TradePrice > highs[baridx]) highs[baridx] = k.TradePrice;
                // low
                if (k.TradePrice < lows[baridx]) lows[baridx] = k.TradePrice;
                // close
                closes[baridx] = k.TradePrice;
                // volume
                if (k.TradeSize >= 0)
                    vols[baridx] += k.TradeSize;
            }

            // notify barlist
            if (_isRecentNew)
                OnNewBar(k.FullSymbol, intervallength);
        }

        public void NewPoint(string symbol, decimal p, int time, int date, int size)
        {

            // get the barcount
            long barid = GetBarId(time, date, intervallength);
            // if not current bar
            if (barid != curr_barid)
            {
                // create a new one
                NewBar(barid);
                // mark it
                _isRecentNew = true;
                // make it current
                curr_barid = barid;
                // set time
                orders[orders.Count - 1] = GetOrder(time, intervallength);
                // set date
                dates[dates.Count - 1] = date;
            }
            else _isRecentNew = false;
            // blend tick into bar
            // open
            if (opens[Last()] == 0) opens[Last()] = p;
            // high
            if (p > highs[Last()]) highs[Last()] = p;
            // low
            if (p < lows[Last()]) lows[Last()] = p;
            // close
            closes[Last()] = p;
            // volume
            if (size >= 0)
                vols[Last()] += size;
            // notify barlist
            if (_isRecentNew)
                OnNewBar(symbol, intervallength);
        }

        /// <summary>
        /// string symbol, int interval
        /// </summary>
        public event Action<string, int> NewBarHandler;
        /// <summary>
        /// raise/publish event
        /// </summary>
        private void OnNewBar(string symbol, int interval)
        {
            var handler = NewBarHandler;
            if (handler != null) handler(symbol, interval);
        }
        /// <summary>
        /// Bar date + bar order
        /// assuming 
        /// </summary>
        private long GetBarId(int time, int date, int intervallength)
        {
            long bcount = GetOrder(time, intervallength);
            // add the date to the front of number to make it unique
            // maximum 86,400 bars of one second
            bcount += (long)date * 100000;
            return bcount;
        }

        /// <summary>
        /// Order of this bar in a day; start with bar 0
        /// </summary>
        private int GetOrder(int time, int intervallength)
        {
            // get time elapsed to this point
            int elap = Util.IntTimeToIntTimeSpan(time);
            // get number of this bar in the day for this interval
            int bcount = (int)((double)elap / intervallength);
            return bcount;
        }
    }
}
