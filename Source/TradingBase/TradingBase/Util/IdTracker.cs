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
    /// Autogenerate next available id for order and strategy
    /// </summary>
    public class IdTracker
    {
        long _nextorderid = 0;
        int _nextstrategyid = 0;

        public IdTracker() : this(DateTime.Now.Ticks) {}
        public IdTracker(long initialordderid)
        {
            _nextorderid = initialordderid;
            _nextstrategyid = 0;
        }
        /// <summary>
        /// Get a new order id
        /// </summary>
        public long NextOrderId
        {
            set { _nextorderid = value; }
            get
            {
                long next = System.Threading.Interlocked.Increment(ref _nextorderid);
                return next;
            }
        }

        /// <summary>
        /// Get a new strategy id
        /// </summary>
        public int NextStrategyId
        {
            set { _nextstrategyid = value; }
            get{
                _nextstrategyid++;
                return _nextstrategyid;
            }
        }
    }
}
