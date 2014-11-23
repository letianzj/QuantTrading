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

using System.ComponentModel;
using TradingBase;

namespace ClassicStrategies
{
    /// <summary>
    /// This straegy will always enter long on every tick, unless it's already in a position.
    /// 
    /// If it's in a position, it will exit at pre-defined stop and loss targets.  (then get back in)
    ///
    /// </summary>
    public class AlwaysEnter : StrategyBase
    {
        /// <summary>
        /// symbol --> position
        /// </summary>
        Dictionary<string, Position> _positions = new Dictionary<string, Position>();

        int _entrysize = 100;
        [Description("size used when entering positions.  Negative numbers would be short entries.")]
        public int EntrySize { get { return _entrysize; } set { _entrysize = value; } }
        decimal _profittarget = .1m;
        [Description("profit target in dollars when position is exited")]
        public decimal ProfitTarget { get { return _profittarget; } set { _profittarget = value; } }

        public AlwaysEnter()
        {
        }

        public override void Reset(bool popup = true)
        {
            _positions.Clear();
        }
        public override void GotTick(Tick tick)
        {
            // ignore quotes
            // after fill, tick size is adjusted. So use tick price to judge
            if (tick.TradePrice == 0) return;
            // get current position
            Position p = RetrievePosition(tick.FullSymbol);
            // if we're flat, enter
            if (p.isFlat && _isactive)
            {
                SendDebug("entering long");
                SendOrder(new MarketOrder(tick.FullSymbol, _entrysize, _idtracker.NextOrderId));
            }
            // otherwise if we're up 10/th of a point, flat us
            else if ((Calc.OpenPT(tick.TradePrice, p) > _profittarget) && _isactive)
            {
                SendDebug("hit profit target");
                SendOrder(new MarketOrderFlat(p,_idtracker.NextOrderId));
            }
        }

        public override void GotFill(Trade f)
        {
            Adjust(new Position(f));
        }

        public override void GotPosition(Position p)
        {
            Adjust(p);
        }

        Position RetrievePosition(string symbol)
        {
            if (_positions.ContainsKey(symbol))
                return _positions[symbol];
            else
                return new Position(symbol, 0, 0, 0);
        }

        void Adjust(Position p)
        {
            if (_positions.ContainsKey(p.FullSymbol))
            {
                Position o = _positions[p.FullSymbol];
                o.Adjust(p);
                _positions[p.FullSymbol] = o;
            }
            else
            {
                _positions.Add(p.FullSymbol, p);
            }
        }
    }
}
