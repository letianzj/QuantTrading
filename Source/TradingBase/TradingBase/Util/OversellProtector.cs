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
    /// prevent or adjust oversell/overcovers
    /// </summary>
    public class OversellProtector
    {
        PositionTracker _pt;
        IdTracker _idt;

        public OversellProtector(int capacity, IdTracker idt)
        {
            _pt = new PositionTracker(capacity);
            _idt = idt;
        }

        public OversellProtector(PositionTracker pt, IdTracker idt)
        {
            _pt = pt;
            _idt = idt;
        }

        bool _verbosedebug = false;
        public bool VerboseDebugging { get { return _verbosedebug; } set { _verbosedebug = value; } }

        protected virtual void v(string msg)
        {
            if (!_verbosedebug)
                return;
            debug(msg);
        }

        int minlotsize = 1;
        public int MinLotSize { get { return minlotsize; } set { minlotsize = value; } }

        int osa = 0;
        public int OversellsAvoided { get { return osa; } }

        bool _split = false;
        /// <summary>
        /// split oversells into two orders (otherwise, oversold portion is dropped)
        /// </summary>
        public bool Split { get { return _split; } set { _split = value; } }

        public event Action<Order> SendOrderEvent;
        public event Action<string> SendDebugEvent;
        public event Action<long> SendCancelEvent;

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        Dictionary<long, long> _orgid2splitid = new Dictionary<long, long>();

        void cancel(long id)
        {
            if (SendCancelEvent != null)
                SendCancelEvent(id);
            else
                debug("Can't cancel: " + id + " as no SendCancelEvent handler is defined!");
        }

        /// <summary>
        /// ensure that if splits are enabled, cancels for the original order also are copied to the split
        /// </summary>
        /// <param name="id"></param>
        public void sendcancel(long id)
        {
            try
            {
                long cancelsplit = 0;
                // cancel original
                cancel(id);
                // cancel split if it exists
                if (_orgid2splitid.TryGetValue(id, out cancelsplit))
                {
                    debug("cancel received on original order: " + id + ", copying cancel to split: " + cancelsplit);
                    cancel(cancelsplit);
                }
                else
                    v("cancel did not match split order, passed along cancel: " + id);
            }
            catch (Exception ex)
            {
                debug("error encountered processing cancel: " + id + " err: " + ex.Message + ex.StackTrace);
            }

        }

        /// <summary>
        /// track and correct oversells (either by splitting into two orders, or dropping oversell)
        /// </summary>
        /// <param name="o"></param>
        public void sendorder(Order o)
        {
            // get original size
            int osize = o.OrderSize;
            int uosize = o.UnsignedSize;
            // get existing size
            int size = _pt[o.FullSymbol].Size;
            int upsize = _pt[o.FullSymbol].UnsignedSize;
            // check for overfill/overbuy
            bool over = (o.OrderSize * size < -1) && (o.UnsignedSize > Math.Abs(size)) && (upsize >= MinLotSize);
            // detect
            if (over)
            {
                // determine correct size
                int oksize = _pt[o.FullSymbol].FlatSize;
                // adjust
                o.OrderSize = Calc.Norm2Min(oksize,MinLotSize);
                // send 
                sonow(o);
                // count
                osa++;
                // notify
                debug(o.FullSymbol + " oversell detected on pos: "+size+" order adjustment: " + osize + "->" + size + " " + o.ToString());
                // see if we're splitting
                if (Split)
                {
                    // calculate new size
                    int nsize = Calc.Norm2Min(Math.Abs(uosize - Math.Abs(oksize)),MinLotSize);
                    // adjust side
                    nsize *= (o.Side ? 1 : -1);
                    // create order
                    Order newo = new Order(o);
                    newo.OrderSize = nsize;
                    newo.Id = _idt.NextOrderId;
                    if (_orgid2splitid.ContainsKey(o.Id))
                        _orgid2splitid[o.Id] = newo.Id;
                    else
                        _orgid2splitid.Add(o.Id, newo.Id);
                    // send
                    if (nsize!=0)
                        sonow(newo);
                    // notify
                    debug(o.FullSymbol + " splitting oversell/overcover: "+o.ToString()+" to 2nd order: " + newo);
                }
            }
            else
                sonow(o);

        }

        void sonow(Order o)
        {
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }

        public void GotFill(Trade t)
        {
            _pt.Adjust(t);
        }

        public void GotPosition(Position p)
        {
            _pt.Adjust(p);
        }
    }
}
