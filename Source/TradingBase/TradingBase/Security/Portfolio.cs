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
    /// A portfolio contains a bunch of positions
    /// </summary>
    public class Portfolio
    {
        decimal _closedpl = 0;
        private Dictionary<string, Position> _positions = new Dictionary<string, Position>();
        public decimal ClosedPL { get { return _closedpl; } set { _closedpl = value; } }
        public Dictionary<string, Position> Positions { get { return _positions; } set { _positions = value; } }

        public Portfolio() { }

        public Portfolio(List<Position> positions)
        {
            if (positions != null)      // null check
            {
                foreach (Position p in positions)
                {
                    if (_positions.ContainsKey(p.FullSymbol))
                    {
                        // adjust
                        _closedpl += _positions[p.FullSymbol].Adjust(p);
                    }
                    else
                    {
                        _positions.Add(p.FullSymbol, p);
                    }
                }
            }
        }

        public Portfolio(Dictionary<string, Position> positions)
        {
            _positions = positions;
        }

        public decimal Adjust(Trade t)
        {
            return Adjust(new Position(t)); 
        }
        public decimal Adjust(Position p)
        {
            decimal pl = 0;
            if (_positions.ContainsKey(p.FullSymbol))
            {
                // adjust
                pl = _positions[p.FullSymbol].Adjust(p);
            }
            else
            {
                _positions.Add(p.FullSymbol, p);
            }

            _closedpl += pl;
            return pl;
        }

        public void Clear()
        {
            _closedpl = 0m;
            _positions.Clear();
        }
    }
}
