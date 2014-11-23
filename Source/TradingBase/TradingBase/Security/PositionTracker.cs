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
    /// track positions for a portfolio
    /// It supports only one account. Use one tracker for one account.
    /// </summary>
    public class PositionTracker : System.Collections.IEnumerable
    {
        /// <summary>
        /// symbol+account --> position
        /// </summary>
        Dictionary<string, Position> _positions;

        decimal _totalclosedpl = 0;
        string _defaultacct = DefaultSettings.DefaultAccount;
        public string DefaultAccount { get { return _defaultacct; } set { _defaultacct = value; } }
        public decimal TotalClosedPL { get { return _totalclosedpl; } }

        public PositionTracker(int capacity)
        {
            _positions = new Dictionary<string, Position>(capacity);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _positions.Values.GetEnumerator();
        }

        public void Clear()
        {
            _positions.Clear();
            _totalclosedpl = 0;
        }

        public bool IsTracked(string symbol)
        {
            return _positions.ContainsKey(symbol);
        }

        public Position this[string symbol]
        {
            get
            {
                if (IsTracked(symbol))
                    return _positions[symbol];
                else
                    return new Position(symbol, 0, 0, 0, DefaultAccount);  
            }
        }

        public void GotPosition(Position p)
        {
            Adjust(p);
        }

        public void GotFill(Trade f)
        {
            Adjust(f);
        }

        /// <summary>
        /// overwrite existing position, or start new position
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public decimal Adjust(Position newpos)
        {
            if (!IsTracked(newpos.FullSymbol))
                _positions.Add(newpos.FullSymbol, new Position(newpos));
            else
            {
                _positions[newpos.FullSymbol] = new Position(newpos);
                _totalclosedpl += newpos.ClosedPL;
            }
            return 0;
        }

        /// <summary>
        /// Adjust an existing position, or create a new one... given a trade and symbol, account
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Adjust(Trade fill)
        {
            decimal cpl = 0;

            if (!IsTracked(fill.FullSymbol))
                _positions.Add(fill.FullSymbol, new Position(fill));
            else
            {
                cpl += this[fill.FullSymbol].Adjust(fill);
            }
            _totalclosedpl += cpl;
            return cpl;
        }
    }
}
