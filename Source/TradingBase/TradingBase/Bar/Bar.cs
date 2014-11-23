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
    public class Bar
    {
        public decimal Open {get; set;}
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public int Date { get; set; }
        /// <summary>
        /// 0 = first bar, 1 = second bar of the day
        /// </summary>
        public int BarOrderInADay {get;set;}
        public string FullSymbol { get; set; }
        public int TradesInBar { get; set; }
        public int Interval { get; set; }

        public bool IsNew { get; set; }
        public bool IsValid { get { return (High >= Low) && (Open != 0) && (Close != 0); } }

        public Bar()
        {
            Interval = 300;         // 5 mins = 300s
            TradesInBar = 0;

            Open = 0;
            High = 0;
            Low = 0;
            Close = 0;
            Volume = 0;
        }

        public Bar(decimal o, decimal h, decimal l, decimal c, long v, int date, int order, string fullsymbol, int interval)
        {
            Interval = interval;
            Open = o;
            High = h;
            Low = l;
            Close = c;
            Volume = v;
            Date = date;
            BarOrderInADay = order;
            FullSymbol = fullsymbol;

            TradesInBar = 0;
        }

        /// <summary>
        /// Normalize time to get bar start time 
        /// </summary>
        public int BarStartTime
        {
            get
            {
                // get datetime
                DateTime dt = Util.ToDateTime(Date,0);
                // add rounded down result
                dt = dt.AddSeconds(BarOrderInADay*Interval);
                // conver back to normal time
                int bt = Util.ToIntTime(dt);
                return bt;
            } 
        }

        /// <summary>
        /// Order of this bar in a day; start with bar 0
        /// </summary>
        public int GetOrder(int time)
        {
            // get time elapsed to this point
            int elap = Util.IntTimeToIntTimeSpan(time);
            // get seconds per bar
            int secperbar = Interval;
            // get number of this bar in the day for this interval
            int bcount = (int)((double)elap / secperbar);
            return bcount;
        }

        public bool NewTick(Tick t)
        {
            if (FullSymbol == "") 
                FullSymbol = t.FullSymbol;
            if (FullSymbol != t.FullSymbol) 
                throw new Exception("Invalid Tick");
            if (BarOrderInADay == 0) 
            { 
                BarOrderInADay = GetOrder(t.Time); 
                Date = t.Date; 
            }
            // check if this bar's tick
            if ((GetOrder(t.Time) != BarOrderInADay) || (Date != t.Date)) 
                return false;
            // if tick doesn't have trade or index, ignore
            if (!t.IsTrade && !t.IsIndex) 
                return true;
            TradesInBar++; // count it
            IsNew = TradesInBar == 1;

            // only count volume on trades, not indicies
            if (!t.IsIndex) Volume += t.TradeSize; // add trade size to bar volume
            if (Open == 0) Open = t.TradePrice;
            if (High == 0) High = t.TradePrice;
            if (Low == 0) Low = t.TradePrice;

            if (t.TradePrice > High) High = t.TradePrice;
            if (t.TradePrice < Low) Low = t.TradePrice;
            Close = t.TradePrice;
            return true;
        }

        public static Tick[] ToTick(Bar bar)
        {
            List<Tick> list = new List<Tick>();
            list.Add(Tick.NewTrade(bar.FullSymbol, bar.Date, bar.BarStartTime, bar.Open,
                (int)((double)bar.Volume / 4)));
            list.Add(Tick.NewTrade(bar.FullSymbol, bar.Date, bar.BarStartTime, bar.High,
                (int)((double)bar.Volume / 4)));
            list.Add(Tick.NewTrade(bar.FullSymbol, bar.Date, bar.BarStartTime, bar.Low,
                (int)((double)bar.Volume / 4)));
            list.Add(Tick.NewTrade(bar.FullSymbol, bar.Date, bar.BarStartTime, bar.Close,
                (int)((double)bar.Volume / 4)));
            return list.ToArray();
        }

        public static Tick[] ToTick2(Bar bar)
        {
            List<Tick> list = new List<Tick>();
            DateTime dt = Util.ToDateTime(bar.Date, bar.BarStartTime).AddSeconds(bar.Interval);   // close at bartime+interval
            list.Add(Tick.NewTrade(bar.FullSymbol, Util.ToIntDate(dt), Util.ToIntTime(dt),
                bar.Close, (int)bar.Volume));
            return list.ToArray();
        }
    }
}
