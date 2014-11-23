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

using System.Collections;

namespace TradingBase
{
    /// <summary>
    /// holds a collection of BarList (one for a symbol)
    /// accepts ticks and auto-generate bars for each BarList
    /// access bars via blt["GOOG"].RecentBar.Close
    /// </summary>
    public class BarListTracker
    {
        // full symbol --> BarList
        Dictionary<string, BarList> _bdict = new Dictionary<string, BarList>();
        private int _defaultinterval = 300;       // default interval
        public int DefaultInterval { get { return _defaultinterval; } set { _defaultinterval = value; } }

        // string = fullsymbol, int = interval
        public event Action<string, int> GotNewBar;

        public BarListTracker(string[]syms) : this(syms, BarList.AllBarIntervals) { }
        public BarListTracker(string[] syms, int barinterval) : this(syms, new int[]{barinterval}) {}
        public BarListTracker(string[] syms, int[] intervals)
        {
            _defaultinterval = intervals[0];

            foreach (string sym in syms)
            {
                BarList bl = new BarList(sym, intervals);
                bl.DefaultInterval = _defaultinterval;
                bl.NewBarHandler += bl_NewBarHandler;
                _bdict.Add(sym, bl);
            }
        }

        /// <summary>
        /// Relay BarList's NewBar event
        /// </summary>
        void bl_NewBarHandler(string sym, int interval)
        {
            if (GotNewBar != null)
                GotNewBar(sym, interval);
        }

        /// <summary>
        /// clears all data from tracker
        /// </summary>
        public void Reset(bool clearall=false)
        {
            foreach (BarList bl in _bdict.Values)
                bl.Reset();
            
            if (clearall)
                _bdict.Clear();
        }

        public BarList this[string fullsymbol]
        {
            get
            {
                return this[fullsymbol, _defaultinterval];
            }
        }

        public BarList this[string fullsymbol, int interval]
        {
            get
            {
                BarList bl;
                if (_bdict.TryGetValue(fullsymbol,out bl))
                {
                    bl.DefaultInterval = interval;
                }
                else
                {
                    bl = new BarList(fullsymbol, interval);
                }
                return bl;
            }
        }

        public IEnumerator GetEnumerator() { foreach (string sym in _bdict.Keys) yield return sym; }

        // <summary>
        /// give any ticks (trades) to this symbol and tracker will create barlists automatically 
        /// </summary>
        /// <param name="k"></param>
        public void NewTick(Tick k)
        {
            BarList bl;
            if (_bdict.TryGetValue(k.FullSymbol, out bl))
            {
                bl.NewTick(k);
            }
        }
    }
}
