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
    /// track order status (submitted, partially filled, etc)
    /// </summary>
    public class OrderTracker
    {
        Dictionary<long, Order> _orders;
        Dictionary<long, int> _sents;       // signed order size
        Dictionary<long, int> _fills;       // signed filled size
        Dictionary<long, bool> _cancels;    // if cancelled

        public OrderTracker(int capacity)
        {
            _orders = new Dictionary<long, Order>(capacity);
            _sents = new Dictionary<long, int>(capacity);           // order size
            _fills = new Dictionary<long, int>(capacity);           // filled size
            _cancels = new Dictionary<long, bool>(capacity);
        }
        public void Clear()
        {
            _orders.Clear();
            _sents.Clear();
            _fills.Clear();
            _cancels.Clear();
        }

        public void GotOrder(Order o)
        {
            if (o.Id == 0)
            {
                Debug(o.FullSymbol + " can't track order with blank id!: " + o.ToString());
                return;
            }

            if (IsTracked(o.Id))
            {
                Debug(" duplicate order id: " + o.Id);
                return;
            }

            // add order
            _orders.Add(o.Id, o);
            _cancels.Add(o.Id, false);
            _fills.Add(o.Id, 0);
            _sents.Add(o.Id, o.OrderSize);

            Debug("order put in tracked: " + o.Id + ", " + o.FullSymbol);
        }

        public void GotCancel(long id)
        {
            if (id == 0) return;

            if (!IsTracked(id))
            {
                Debug("order id not tracked: " + id);
            }

            _cancels[id] = true;
        }

        public void GotFill(Trade f)
        {
            if (f.Id == 0)
            {
                Debug(" fill id is empty: " + f.ToString());
                return;
            }

            if (!IsTracked(f.Id))
            {
                Debug(" fill id not tracked: " + f.Id);
                return;
            }

            // add to fills
            _fills[f.Id] += f.TradeSize;
            
            Debug(f.FullSymbol + " filled size: " + _fills[f.Id] + " ; on trade detail: " + f.ToString());
        }

        #region properties
        public int Count
        {
            get { return _orders.Count; }
        }
        public bool IsTracked(long orderid)
        { 
            return _fills.ContainsKey(orderid); 
        }

        /// <summary>
        /// partially filled
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public bool IsPending(long orderid)
        {
            if (!IsTracked(orderid))
                return false;
            if (_cancels[orderid])
                return false;
            return _sents[orderid] != _fills[orderid];
        }

        public bool IsCompleted(long orderid)
        {
            if (!IsTracked(orderid))
                return false;
            if (_cancels[orderid])
                return false;
            return _sents[orderid] == _fills[orderid];
        }

        public bool IsCanceled(long orderid)
        {
            if (!IsTracked(orderid))
                return false;
            return _cancels[orderid];
        }

        public int Sent(long id)
        {
            if (!IsTracked(id))
                return 0;
            return _sents[id];
        }
        /// <summary>
        /// returns filled size of an order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Filled(long id)
        {
            if (!IsTracked(id))
                return 0;
            return _fills[id];
        }

        /// <summary>
        /// get unfilled portion of order from index
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int this[long id]
        {
            get
            {
                if (!IsTracked(id)) return 0;
                if (_cancels[id])
                    return 0;
                return _sents[id] - _fills[id];
            }
        }

        /// <summary>
        /// gets entire sent order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order SentOrder(long id)
        {
            if (IsTracked(id))
                return _orders[id];
            else
                return new Order();
        }

        #endregion

        public event Action<string> SendDebugEvent;
        protected void Debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
    }
}
