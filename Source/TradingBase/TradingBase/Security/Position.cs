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
    /// A security position
    /// </summary>
    public class Position : IConvertible
    {    
        public Position() : this("") { }
        public Position(Position p) : this(p._fullsymbol, p.AvgPrice, p.Size, p.ClosedPL, p.Account) { }
        public Position(string symbol) { _fullsymbol = symbol; }
        public Position(string symbol, decimal price, int size) : this(symbol, price, size, 0,"") { }
        public Position(string symbol, decimal price, int size, decimal closedpl) : this(symbol, price, size, closedpl, "") { }
        public Position(string symbol, decimal price, int size, decimal closedpl, string account) 
        { 
            _fullsymbol = symbol; 
            if (size == 0) 
                price = 0; 
            _avgprice = price; 
            _size = size; 
            _closedpl = closedpl; 
            _acct = account;
            if (!this.isValid) 
                throw new Exception("Can't construct invalid position!"); 
        }
        public Position(Trade t) 
        {
            if (!t.IsValid) throw new Exception("Can't construct a position object from invalid trade.");
            _fullsymbol = t.FullSymbol; _avgprice = t.TradePrice; _size = t.TradeSize; _date = t.TradeDate; _time = t.TradeTime; _acct = t.Account;
            if (_size>0) _size *= t.Side ? 1 : -1;
        }
        private string _acct = DefaultSettings.DefaultAccount;
        private string _fullsymbol = "";
        private int _size = 0;
        private decimal _avgprice = 0;
        private int _date = 0;
        private int _time = 0;
        private decimal _closedpl = 0;
        private decimal _openpl = 0;
        public bool isValid
        {
            get { return (_fullsymbol!="") && (((AvgPrice == 0) && (Size == 0)) || ((AvgPrice != 0) && (Size != 0))); }
        }
        /// <summary>
        /// Realized PnL
        /// </summary>
        public decimal ClosedPL { set { _closedpl = value; } get { return _closedpl; } }
        public decimal OpenPL { set { _openpl = value; } get { return _openpl; } }
        public string FullSymbol { set { _fullsymbol = value; } get { return _fullsymbol; } }
        public decimal AvgPrice { set { _avgprice = value; } get { return _avgprice; } }
        public int Size { set { _size = value; } get { return _size; } }
        public int UnsignedSize { get { return Math.Abs(_size); } }
        public bool isLong { get { return _size > 0; } }
        public bool isFlat { get { return _size==0; } }
        public bool isShort { get { return _size < 0; } }
        public int FlatSize { get { return _size * -1; } }
        public string Account { set { _acct = value; }  get { return _acct; } }
        // returns any closed PL calculated on position basis (not per share)
        /// <summary>
        /// Adjusts the position by applying a new position.
        /// </summary>
        /// <param name="pos">The position adjustment to apply.</param>
        /// <returns></returns>
        public decimal Adjust(Position pos)
        {
            if ((_fullsymbol!="") && (this._fullsymbol != pos._fullsymbol)) throw new Exception("Failed because adjustment symbol did not match position symbol");
            if (_acct == DefaultSettings.DefaultAccount) _acct = pos.Account;
            if (_acct != pos.Account) throw new Exception("Failed because adjustment account did not match position account.");
            if ((_fullsymbol=="") && pos.isValid) _fullsymbol = pos._fullsymbol;
            if (!pos.isValid) throw new Exception("Invalid position adjustment, existing:" + this.ToString() + " adjustment:" + pos.ToString());

            decimal pl = 0;
            if (! pos.isFlat)
            {
                bool oldside = isLong;
                pl = Calc.ClosePL(this, pos.ToTrade());
                if (this.isFlat) this._avgprice = pos.AvgPrice; // if we're leaving flat just copy price
                else if ((pos.isLong && this.isLong) || (!pos.isLong && !this.isLong)) // sides match, adding so adjust price
                    this._avgprice = ((this._avgprice * this._size) + (pos.AvgPrice * pos.Size)) / (pos.Size + this.Size);
                this._size += pos.Size; // adjust the size
                if (oldside != isLong) _avgprice = pos.AvgPrice; // this is for when broker allows flipping sides in one trade
                if (this.isFlat) _avgprice = 0; // if we're flat after adjusting, size price back to zero
                _closedpl += pl; // update running closed pl

                return pl;
            }

            _openpl = Calc.OpenPL(pos.AvgPrice, this);

            return pl;
        }
        /// <summary>
        /// Adjusts the position by applying a new trade or fill.
        /// </summary>
        /// <param name="t">The new fill you want this position to reflect.</param>
        /// <returns></returns>
        public decimal Adjust(Trade t) { return Adjust(new Position(t)); }

        public override string ToString()
        {
            return FullSymbol+" "+Size+"@"+AvgPrice.ToString("F2")+" ["+Account+"]";
        }
        public Trade ToTrade()
        {
            DateTime dt = (_date*_time!=0) ? Util.ToDateTime(_date, _time) : DateTime.Now;
            return (Trade)new Trade(FullSymbol, AvgPrice, Size, dt);
        }

        public static Position Deserialize(string msg)
        {
            
            string[] r = msg.Split(',');
            string sym = r[0];
            decimal price = Convert.ToDecimal(r[1], System.Globalization.CultureInfo.InvariantCulture);
            int size = Convert.ToInt32(r[2]);
            decimal cpl = Convert.ToDecimal(r[4], System.Globalization.CultureInfo.InvariantCulture);
            string act = r[5];
            Position p = new Position(sym,price,size,cpl,act);
            return p;
        }

        public static string Serialize(Position p)
        {
            string[] r = new string[] { 
                p._fullsymbol, 
                p.AvgPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 
                p.Size.ToString("F0", System.Globalization.CultureInfo.InvariantCulture), 
                p.ClosedPL.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 
                p.Account 
            };
            return string.Join(",", r);
        }

        #region Conversion
        public decimal ToDecimal(IFormatProvider provider)
        {
            return AvgPrice;
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return (double)AvgPrice;
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return (Int16)Size;
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Size;
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Size;
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return !isFlat;
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Size, conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        /// <summary>
        /// convert from position to decimal (price)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static implicit operator decimal(Position p)
        {
            return p.AvgPrice;
        }
        /// <summary>
        /// convert from position to integer (size)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static implicit operator int(Position p)
        {
            return p.Size;
        }
        /// <summary>
        /// convert from
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static implicit operator bool(Position p)
        {
            return !p.isFlat;
        }
        #endregion
    }
}
