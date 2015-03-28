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
    /// Order is derived class of trade with extra order information.
    /// It becomes a trade when executed.
    /// </summary>
    [Serializable]
    public class Order : Trade
    {
        public TimeInForce TIF { get { return _tif; } set { _tif = value; } }
        public OrderStatus OrderStatus { get; set; }
        public int OrderDate { get; set; }
        public int OrderTime { get; set; }
        public decimal LimitPrice {get; set;}
        public decimal StopPrice {get; set;}
        public decimal TrailPrice { get; set; }
        public int OrderSize { get; set; }       // < 0 = short, order size != trade size
        public bool OrderSide { get { return OrderSize > 0; } }
        public string OrderType
        {
            get {
                return (IsStop && IsLimit) ? "STP LMT"
                : (IsStop ? "STP"
                : ((IsTrail && IsLimit) ? "TRAIL LIMIT"
                : (IsLimit ? "LMT"
                : (IsTrail ? "TRAIL" : "MKT"))));
            } 
        }

        /// <summary>
        /// owner/originator of this order; it's the strategy id.
        /// </summary>
        public int VirtualOwner { get; set; }
        public bool IsMarket { get { return (LimitPrice == 0) && (StopPrice == 0); } }
        public bool IsLimit { get { return (LimitPrice != 0); } }
        public bool IsStop { get { return (StopPrice != 0); } }
        public bool IsTrail { get { return(TrailPrice != 0); } }
        public new bool IsValid
        {
            get
            {
                if (IsFilled) return base.IsValid;
                return (FullSymbol != null) && (OrderSize != 0);
            }
        }
        public new int UnsignedSize { get { return Math.Abs(OrderSize); } }
        public decimal Price
        {
            get { return IsStop ? StopPrice : LimitPrice; }
        }

        private TimeInForce _tif = TimeInForce.DAY;

        public Order() : base() { }

        /// <summary>
        /// Market Order
        /// </summary>
        public Order(string symbol, int size) : this(symbol, size, 0) {}

        public Order(string symbol, int size, long id)
        {
            this.FullSymbol = symbol;
            this.OrderSize = size;
            this.Id = id;

            this.LimitPrice = 0;
            this.StopPrice = 0;
            this.TrailPrice = 0;
            this.OrderDate = Util.ToIntDate(DateTime.Now);
            this.OrderTime = Util.ToIntTime(DateTime.Now);
            this.Account = "";
            this.Currency = "USD";
        }
        
        public Order(string symbol,int size, long id,  decimal p, decimal s, decimal t) : this(symbol, size, id, p, s, t, Util.ToIntDate(DateTime.Now), Util.ToIntDate(DateTime.Now), TimeInForce.DAY) {}
        public Order(string symbol,int size, long id,  decimal p, decimal s, decimal t, int date, int time, TimeInForce tif = TimeInForce.DAY)
        {
            this.FullSymbol = symbol;
            this.OrderSize = size;
            this.Id = id;
            this.OrderDate = date;
            this.OrderTime = time;
            this.LimitPrice = p;
            this.StopPrice = s;
            this.TrailPrice = t;
            this.Account = "";
            this.TIF = tif;
            this.Currency = "USD";
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        public Order(Order copy)
        {
            this.FullSymbol = copy.FullSymbol;
            this.OrderSize = copy.OrderSize;
            this.Id = copy.Id;
            this.OrderDate = copy.OrderDate;
            this.OrderTime = copy.OrderTime;
            this.OrderSize = copy.OrderSize;
            this.LimitPrice = copy.LimitPrice;
            this.StopPrice = copy.StopPrice;
            this.TrailPrice = copy.TrailPrice;
            this.Account = copy.Account;
            this.TIF = copy.TIF;
            this.Currency = copy.Currency;
        }

        /// <summary>
        /// fill order against bid/ask or trade
        /// It doesn't adjust pass-in tick
        /// </summary>
        /// <param name="k"></param>
        /// <param name="smart"></param>
        /// <param name="fillOPG"></param>
        /// <returns></returns>
        public bool Fill(Tick k, bool bidask = false, bool fillOPG = false)
        {
            if (k.FullSymbol != this.FullSymbol) return false;
            if (!fillOPG && TIF == TimeInForce.OPG) return false;

            decimal p;
            int s;

            if (k.IsTrade)
            {
                p = k.TradePrice;
                s = k.TradeSize;
            }
            else if (bidask)       // fill on bid/ask 
            {
                bool ok = OrderSide ? k.HasAsk : k.HasBid;
                if (!ok) return false;
                p = OrderSide ? k.AskPrice : k.BidPrice;
                s = OrderSide ? k.AskSize : k.BidSize;
            }
            else
            {
                return false;
            }
            
            if ((IsLimit && OrderSide && (p <= LimitPrice)) // buy limit
                || (IsLimit && !OrderSide && (p >= LimitPrice))// sell limit
                || (IsStop && OrderSide && (p >= StopPrice)) // buy stop
                || (IsStop && !OrderSide && (p <= StopPrice)) // sell stop
                || IsMarket)
            {
                this.TradePrice = k.TradePrice;
                this.TradeSize = (s >= UnsignedSize ? UnsignedSize : s) * (OrderSide ? 1 : -1);
                this.TradeTime = k.Time;
                this.TradeDate = k.Date;
                return true;
            }
            return false;
        }

        /// <summary>
        /// for backtest/simulation: fill assuming high liquidity - fill stops and limits at their stop price
        /// rather than at bid, ask, or trade. primarily for use when only daily data is available.
        /// </summary>
        public bool FillHighLiquidityEOD(Tick k, bool bidask, bool fillOPG = false)
        {
            if ((!IsStop && !IsLimit))
                return Fill(k, bidask, fillOPG);

            if (k.FullSymbol != FullSymbol)
                return false;
            if (!fillOPG && (this._tif == TimeInForce.OPG))
                return false;

            // determine size and activation price using bid-ask or trade method
            int s;
            decimal p;
            if (bidask)
            {
                bool ok = OrderSide ? k.HasAsk : k.HasBid;
                if (!ok)
                    return false;
                s = OrderSide ? k.AskSize : k.BidSize;
                p = OrderSide ? k.AskPrice : k.BidPrice;
            }
            else
            {
                if (!k.IsTrade)
                    return false;
                s = k.TradeSize;
                p = k.TradePrice;
            }

            // record the fill
            this.TradeSize = (s >= UnsignedSize ? UnsignedSize : s) * (OrderSide ? 1 : -1);
            this.TradeTime = k.Time;
            this.TradeDate = k.Date;

            if ((IsLimit && OrderSide && (p <= LimitPrice)) // buy limit
             || (IsLimit && !OrderSide && (p >= LimitPrice))) // sell limit
            {
                this.TradePrice = LimitPrice;
                return true;
            }
            else if ((IsStop && OrderSide && (p >= StopPrice)) // buy stop
                  || (IsStop && !OrderSide && (p <= StopPrice))) // sell stop
            {
                this.TradePrice = StopPrice;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to fill incoming order against this order.  If orders match.
        /// It doesn't adjust the pass-in order
        /// </summary>
        /// <param name="o"></param>
        /// <returns>order can be cast to valid Trade and function returns true.  Otherwise, false</returns>
        public bool Fill(Order o)
        {
            // sides must match
            if (OrderSide == o.OrderSide) return false;
            // orders must be valid
            if (!o.IsValid || !this.IsValid) return false;
            // acounts must be different
            if (o.Account == Account) return false;
            if ((IsLimit && OrderSide && (o.LimitPrice <= LimitPrice)) // buy limit cross
                || (IsLimit && !OrderSide && (o.LimitPrice >= LimitPrice))// sell limit cross
                || (IsStop && OrderSide && (o.LimitPrice >= StopPrice)) // buy stop
                || (IsStop && !OrderSide && (o.LimitPrice <= StopPrice)) // sell stop
                || IsMarket)
            {
                this.TradePrice = o.IsLimit ? o.LimitPrice : o.StopPrice;
                if (TradePrice == 0) TradePrice = IsLimit ? LimitPrice : StopPrice;
                this.TradeSize = o.UnsignedSize >= UnsignedSize ? UnsignedSize : o.UnsignedSize;
                this.TradeTime = o.OrderTime;
                this.TradeDate = o.OrderDate;
                return IsFilled;
            }
            return false;
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        /// <summary>
        /// A place holder. Decimals is not used at this time.
        /// </summary>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public string ToString(int decimals)
        {
            return Serialize(this);
        }

        public static string Serialize(Order o)
        {
            if (o.IsFilled) return Trade.Serialize((Trade)o);
            string[] s = new string[] 
            {
                o.OrderDate.ToString(), 
                o.OrderTime.ToString(),
                o.Id.ToString(),
                o.Account,
                o.FullSymbol, 
                o.OrderSize.ToString(), 
                o.LimitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture), 
                o.StopPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
                o.TrailPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
                o.Currency.ToString(), 
                o.TIF.ToString()                
            };
            return string.Join(",",s);
        }

        public new static Order Deserialize(string message)
        {
            Order o = new Order();
            string[] rec = message.Split(','); // get the record

            o.OrderDate = Convert.ToInt32(rec[0]);
            o.OrderTime = Convert.ToInt32(rec[1]);
            o.Id = Convert.ToInt64(rec[2]);
            o.Account = rec[3];
            o.FullSymbol = rec[4];
            o.OrderSize = Convert.ToInt32(rec[5]);
            o.LimitPrice = Convert.ToDecimal(rec[6], System.Globalization.CultureInfo.InvariantCulture);
            o.StopPrice = Convert.ToDecimal(rec[7], System.Globalization.CultureInfo.InvariantCulture);
            o.TrailPrice = Convert.ToDecimal(rec[8], System.Globalization.CultureInfo.InvariantCulture);
            o.Currency = rec[9];
            TimeInForce tif = TimeInForce.DAY;
            Enum.TryParse<TimeInForce>(rec[10], out tif);
            o.TIF = tif;

            return o;
        }
    }

    #region types of order
    /// <summary>
    /// Market Order
    /// </summary>
    public class MarketOrder : Order
    {
        public MarketOrder(string sym, int size) : this(sym, size,0) { }
        public MarketOrder(string sym, int size, long id) : base(sym, size, id) { }
    }

    public class MarketOrderFlat : Order
    {
        public MarketOrderFlat(Position p) : this(p, 0) { }
        public MarketOrderFlat(Position p, long id) : base(p.FullSymbol, p.FlatSize, id) { }
        public MarketOrderFlat(Position p, decimal percent, bool normalizeSize, int MinimumLotSize) :
            this(p, percent, normalizeSize, MinimumLotSize, 0) { }
        public MarketOrderFlat(Position p, decimal percent, bool normalizeSize, int MinimumLotSize, long id) :
            base(p.FullSymbol, normalizeSize ? Calc.Norm2Min((decimal)percent * p.FlatSize, MinimumLotSize) : (int)(percent * p.FlatSize), id) { }
    }

    /// <summary>
    /// Limit orders.
    /// </summary>
    public class LimitOrder : Order
    {
        public LimitOrder(string sym, int size, decimal price) : this(sym, size, price, 0) {}
        public LimitOrder(string sym, int size, decimal price, long orderid) : base(sym, size, orderid, price, 0, 0) { }
    }

    /// <summary>
    /// A stop-loss order.
    /// </summary>
    public class StopOrder : Order
    {
        public StopOrder(string sym, int size, decimal stop) : this(sym, size, stop, 0) { }
        public StopOrder(string sym, int size, decimal stop, long orderid) : base(sym, size, orderid, 0, stop, 0) { }
    }

    /// <summary>
    /// Create stop limit orders.
    /// </summary>
    public class StopLimitOrder : Order
    {
        public StopLimitOrder(string sym, int size, decimal price, decimal stop) : this(sym, size, price, stop, 0) { }
        public StopLimitOrder(string sym, int size, decimal price, decimal stop, long orderid) : base(sym, size, orderid, price, stop, 0) { }
       
    }

    /// <summary>
    /// Create trailing stop order (TRAIL)
    /// </summary>
    public class TrailingStopOrder : Order
    {
        public TrailingStopOrder(string sym, int size, decimal trailing) : this(sym, size, trailing, 0) { }
        public TrailingStopOrder(string sym, int size, decimal trailing, long orderid) : base(sym, size, orderid, 0m, 0m, trailing) { }

    }

    /// <summary>
    /// Create trailing stop limit order (TRAIL LIMIT)
    /// Somehow this order type doesn't work
    /// </summary>
    public class TrailingStopLimitOrder : Order
    {
        public TrailingStopLimitOrder(string sym, int size, decimal limit, decimal trailing) : this(sym, size, limit, trailing, 0) { }
        public TrailingStopLimitOrder(string sym, int size, decimal limit, decimal trailing, long orderid) : base(sym, size, orderid, limit, 0m, trailing) { }

    }
    #endregion
}
