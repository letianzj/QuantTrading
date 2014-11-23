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
    /// Unlike Trade type in Tick, here it is a trade one does, thus has direction (buy/sell)
    /// It is an order that is executed
    /// thus it can be added to position
    /// </summary>
    [Serializable]
    public class Trade
    {
        /// <summary>
        /// TradeId or OrderId
        /// </summary>
        public long Id { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// e.g. STK, OPT, FUT, FOP,IDX
        /// </summary>
        // public string SecurityType { get; set; }
        // public string Symbol { get; set; }
        // public string Exchange { get; set; }
        public string FullSymbol { get; set; }
        public string Account { get; set; }
        /// <summary>
        /// Size < 0 means sell
        /// </summary>
        public int TradeSize { get; set; }
        public int TradeDate { get; set; }
        public int TradeTime { get; set; }
        public decimal TradePrice { get; set; }

        public int UnsignedSize { get { return Math.Abs(TradeSize); } }
        public bool Side { get { return TradeSize > 0; } }
        public bool IsFilled { get { return (TradePrice != 0) && (TradeSize != 0); } }
        public virtual bool IsValid { get { return (TradeSize != 0) && (TradePrice != 0) && (TradeTime + TradeDate != 0) && (FullSymbol != null) && (FullSymbol != ""); } }

        public Trade() {}
        public Trade(string sym, decimal fillprice, int fillsize, DateTime tradedate) : this(sym,fillprice, fillsize, Util.ToIntDate(tradedate),Util.ToIntTime(tradedate)) {}
        public Trade(string fullname, decimal fillprice, int fillsize, int filldate, int filltime)
        {
            if (fullname != null) FullSymbol = fullname.ToUpper();
            if ((fillsize == 0) || (fillprice == 0)) throw new Exception("Invalid trade: Zero price or size provided.");
            TradeTime = filltime;
            TradeDate = filldate;
            TradeSize = fillsize;
            TradePrice = fillprice;
        }

        public Trade(Trade copy)
        {
            Id = copy.Id;
            FullSymbol = copy.FullSymbol;
            Account = copy.Account;
            TradeDate = copy.TradeDate;
            TradeTime = copy.TradeTime;
            TradePrice = copy.TradePrice;
            TradeSize = copy.TradeSize;
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        public static string Serialize(Trade t)
        {
            string[] trade = new string[] { 
                t.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                t.Account,
                t.TradeDate.ToString(System.Globalization.CultureInfo.InvariantCulture), 
                t.TradeTime.ToString(System.Globalization.CultureInfo.InvariantCulture), 
                t.FullSymbol, 
                t.TradeSize.ToString(System.Globalization.CultureInfo.InvariantCulture), 
                t.TradePrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 
            };

            return string.Join(",", trade);
        }

        public static Trade Deserialize(string tstr)
        {
            string[] ts = tstr.Split(',');
            if (ts.Length < 14) throw new Exception("Invalid trade");

            Trade t = new Trade();
            t.Id = Convert.ToInt64(ts[0], System.Globalization.CultureInfo.InvariantCulture);
            t.Account = ts[1];
            t.TradeDate = Convert.ToInt32(ts[2], System.Globalization.CultureInfo.InvariantCulture);
            t.TradeTime = Convert.ToInt32(ts[3], System.Globalization.CultureInfo.InvariantCulture);
            t.FullSymbol = ts[4];
            t.TradeSize = Convert.ToInt32(ts[5], System.Globalization.CultureInfo.InvariantCulture);
            t.TradePrice = Convert.ToDecimal(ts[6], System.Globalization.CultureInfo.InvariantCulture);

            return t;
        }

        public static string ToChartLabel(Trade fill)
        {
            return (fill.TradeSize + fill.FullSymbol);
        }

        public Security Security { get { return new Security(FullSymbol); } }
    }
}
