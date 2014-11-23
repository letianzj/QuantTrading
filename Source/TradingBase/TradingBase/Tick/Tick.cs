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
    /// Tick can be a quote (bid/ask) or a trade
    /// Size < 0 means index
    /// </summary>
    [Serializable]
    public class Tick
    {
        public string FullSymbol { get; set; }
        public int Date { get; set; }
        public int Time { get; set; }
        public decimal BidPrice { get; set; }
        public int BidSize { get; set; }
        // public string BidExchange { get; set; }
        public decimal AskPrice { get; set; }
        public int AskSize { get; set; }
        // public string AskExchange { get; set; }
        public decimal TradePrice { get; set; }
        public int TradeSize { get; set; }
        // public string TradeExchange { get; set; }
        public int Depth { get; set; }

        public bool IsIndex { get { return TradeSize < 0; } }
        public bool HasBid { get { return (BidPrice != 0) && (BidSize != 0); } }
        public bool HasAsk { get { return (AskPrice != 0) && (AskSize != 0); } }
        public bool IsFullQuote { get { return HasBid && HasAsk; } }
        public bool IsQuote { get { return !IsTrade && (HasBid || HasAsk); } }
        public bool IsTrade { get { return (TradePrice != 0) && (TradeSize > 0); } }
        public bool HasTick { get { return (IsTrade || HasBid || HasAsk); } }
        public bool IsValid { get { return (FullSymbol != "" && (IsIndex || HasTick)); } }
        public long Datetime { get { return (long)Date * 1000000 + (long)Time; } }

        public Tick():this("")
        {
        }

        public Tick(string fullsymbol)
        {
            FullSymbol = fullsymbol;
            Date = 0;
            Time = 0;
            BidPrice = 0m;
            BidSize = 0;
            AskPrice = 0m;
            AskSize = 0;
            TradePrice = 0m;
            TradeSize = 0;
            Depth = 0;
        }

        public static Tick NewTrade(string fullsym, int date, int time, decimal trade, int size)
        {
            Tick t = new Tick(fullsym);
            t.Date = date;
            t.Time = time;
            t.TradePrice = trade;
            t.TradeSize = size;
            t.BidPrice = 0m;
            t.AskPrice = 0m;
            return t;
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        public static string Serialize(Tick t)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(t.FullSymbol);
            sb.Append(d);
            sb.Append(t.Date);
            sb.Append(d);
            sb.Append(t.Time);
            sb.Append(d);
            sb.Append(t.TradePrice.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(t.TradeSize);
            sb.Append(d);
            sb.Append(t.BidPrice.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(t.BidSize);
            sb.Append(d);
            sb.Append(t.AskPrice.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(t.AskSize);
            sb.Append(d);
            sb.Append(t.Depth);

            return sb.ToString();
        }

        public static Tick Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            Tick t = new Tick();
            decimal d = 0;
            int i = 0;
            
            t.FullSymbol = r[0];
            if (int.TryParse(r[1], out i))
                t.Date = i;
            if (int.TryParse(r[2], out i))
                t.Time = i;
            if (decimal.TryParse(r[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                t.TradePrice = d;
            if (int.TryParse(r[4], out i))
                t.TradeSize = i;
            if (decimal.TryParse(r[5], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                t.BidPrice = d;
            if (int.TryParse(r[6], out i))
                t.BidSize = i;
            if (decimal.TryParse(r[7], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d))
                t.AskPrice = d;            
            if (int.TryParse(r[8], out i))
                t.AskSize = i;         
            if (int.TryParse(r[9], out i))
                t.Depth = i;

            return t;
        }
    }
}
