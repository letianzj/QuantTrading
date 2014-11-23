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
    /// It holds a succession/list of bars for different barintervals
    /// It accepts ticks and automatically create new bars as needed.
    /// </summary>
    public class BarList
    {
        // holds avaialbe intervals
        private int[] _availableintervals = new int[0];
        // holds all raw data
        private BarTimeIntervalData[] _intervaldata = new BarTimeIntervalData[0];
        // interval --> interval index
        private Dictionary<int, int> _intdataidx = new Dictionary<int, int>();

        public string FullSymbol { get { return _fullsymbol; } set { _fullsymbol = value; } }
        public int DefaultInterval { get { return _defaultinterval; } set { _defaultinterval = value; } }
        public int[] AvailableIntervals { get { return _availableintervals; } }
        /// <summary>
        /// gets count for given bar interval
        /// </summary>
        public int IntervalCount(int interval) { return _intervaldata[_intdataidx[interval]].Count(); }
        private string _fullsymbol;
        private int _defaultinterval = 300;       // default interval
        private bool _valid = false;            // if the barlist is valid or not

        public BarList(string fullsymbol) : this(fullsymbol, AllBarIntervals) { }

        public BarList(string fullsymbol, int barinterval) : this(fullsymbol, new int[] { barinterval }) { }

        public BarList(string symbol, int[] intervals)
        {
            _fullsymbol = symbol;
            _availableintervals = intervals;

            // one intervaldata for one interval
            _intervaldata = new BarTimeIntervalData[_availableintervals.Length];
            
            for (int i = 0; i < _availableintervals.Length; i++)
            { 
                try
                {
                    // save index to this data for the interval
                    _intdataidx.Add(_availableintervals[i], i);
                }
                // if key was already present, already had this interval
                catch (Exception) { continue; }
                if (i == 0)
                    _defaultinterval = _availableintervals[0];

                _intervaldata[i] = new BarTimeIntervalData(_availableintervals[i]);

                // subscribe to bar events
                _intervaldata[i].NewBarHandler += new Action<string, int>(OnNewBar);
            }
        }

        /// <summary>
        /// erases all bar data
        /// </summary>
        public void Reset()
        {
            foreach (BarTimeIntervalData id in _intervaldata)
            {
                id.Reset();
            }
        }

        // Change default interval before call the following
        public decimal[] Open() { return _intervaldata[_intdataidx[_defaultinterval]].Open().ToArray(); }
        public decimal[] High() { return _intervaldata[_intdataidx[_defaultinterval]].High().ToArray(); }
        public decimal[] Low() { return _intervaldata[_intdataidx[_defaultinterval]].Low().ToArray(); }
        public decimal[] Close() { return _intervaldata[_intdataidx[_defaultinterval]].Close().ToArray(); }
        public long[] Vol() { return _intervaldata[_intdataidx[_defaultinterval]].Vol().ToArray(); }
        public int[] Date() { return _intervaldata[_intdataidx[_defaultinterval]].Date().ToArray(); }
        public int[] BarOrder() { return _intervaldata[_intdataidx[_defaultinterval]].BarOrder().ToArray(); }

        /// <summary>
        /// gets specific bar in specified interval
        /// </summary>
        public Bar this[int barnumber]
        {
            get
            {
                return _intervaldata[_intdataidx[_defaultinterval]].GetBar(barnumber, FullSymbol);
            }
        }

        /// <summary>
        /// gets a specific bar in specified interval
        /// </summary>
        public Bar this[int barnumber, int interval] 
        { 
            get 
            { 
                return _intervaldata[_intdataidx[interval]].GetBar(barnumber, FullSymbol); 
            } 
        }

        public void NewTick(Tick k)
        {
            // only pay attention to trades and indicies
            if (k.TradePrice == 0) return;
            // make sure we have a symbol defined 
            if (!_valid)
            {
                _fullsymbol = k.FullSymbol;
                _valid = true;
            }
            // make sure tick is from our symbol
            if (_fullsymbol != k.FullSymbol) return;
            // add tick to every requested bar interval
            for (int i = 0; i < _intervaldata.Length; i++)
                _intervaldata[i].NewTick(k);
        }

        /// <summary>
        /// insert a bar at particular place in the list.
        /// REMEMBER YOU MUST REHANDLE GOTNEWBAR EVENT AFTER CALLING THIS.
        /// </summary>
        public static BarList InsertBar(BarList bl, Bar b, int position)
        {

            BarList copy = new BarList(bl.FullSymbol, bl.AvailableIntervals);
            for (int j = 0; j < bl.AvailableIntervals.Length; j++)
            {
                if (bl.AvailableIntervals[j] != b.Interval)
                    continue;
                int count = bl.IntervalCount(b.Interval);
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (i == position)
                        {
                            AddBar(copy, b, j);
                        }
                        AddBar(copy, bl[i, b.Interval], j);
                    }
                }
                else
                    AddBar(copy, b, 0);
            }
            return copy;
        }

        public event Action<string, int> NewBarHandler;
        void OnNewBar(string symbol, int interval)
        {
            var handler = NewBarHandler;
            if (handler != null) handler(symbol, interval);
        }

        internal static void AddBar(BarList b, Bar mybar, int instdataidx)
        {
            b._intervaldata[instdataidx].AddBar(mybar);
        }

        public static int[] AllBarIntervals
        {
            get
            {
                return new int[] {
                    300,        // first is the default interval
                    60,
                    600,
                    900,
                    1800,
                    3600
                };
            }
        }
    }
}
