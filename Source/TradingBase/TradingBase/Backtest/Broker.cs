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
    /// A simulated broker who processes orders and fills them against external (historical) tick feed.
    /// </summary>
    public class Broker
    {
        public event Action<Order> GotOrderHandler;
        public event Action<Trade> GotFillHandler;
        /// <summary>
        /// int = order id
        /// </summary>
        public event Action<long> GotOrderCancelConfirmHandler;
        /// <summary>
        /// string = symbol, bool = size
        /// </summary>
        public event Action<string, bool, long> GotOrderCancelBroadcastHandler;

        private void OnGotOrder(Order o)
        {
            var handler = GotOrderHandler;
            if (handler != null) handler(o);
        }
        private void OnGotFill(Trade t)
        {
            var handler = GotFillHandler;
            if (handler != null) handler(t);
        }
        private void OnGotOrderCancelConfirm(long orderid)
        {
            var handler = GotOrderCancelConfirmHandler;
            if (handler != null) handler(orderid);
        }
        private void OnGotOrderCancelBroadcast(string sym, bool side, long orderid)
        {
            var handler = GotOrderCancelBroadcastHandler;
            if (handler != null) handler(sym, side, orderid);
        }
        
        public Broker()
        {
            Reset();
        }

        /// <summary>
        /// Resets this instance, clears all orders/trades/accounts held by the broker.
        /// </summary>
        public void Reset()
        {
            CancelOrders();
            _accountlist.Clear();
            _masterorders.Clear();
            _mastertrades.Clear();
            AddAccount(_defaultaccount);
        }

        private Account _defaultaccount = new Account("Default", "Defacto account when account not provided");
        protected Dictionary<Account, List<Order>> _masterorders = new Dictionary<Account, List<Order>>();
        protected Dictionary<string, List<Trade>> _mastertrades = new Dictionary<string, List<Trade>>();
        protected Dictionary<string, Account> _accountlist = new Dictionary<string, Account>();
        /// <summary>
        /// symbols that have open orders.
        /// </summary>
        private List<string> _hasopened = new List<string>();

        bool _usebidaskfill = false;
        /// <summary>
        /// whether bid/ask is used to fill orders.  if false, last trade is used.
        /// </summary>
        public bool UseBidAskFills { get { return _usebidaskfill; } set { _usebidaskfill = value; } }
        bool _adjustincomingticksize = false;
        public bool AdjustIncomingTickSize { get { return _adjustincomingticksize; } set { _adjustincomingticksize = value; } }
        bool _useHighLiquidityFillsEOD = false;
        /// <summary>
        /// whether or not to assume high liquidity fills on sparse data.
        /// (should only be used on daily/EOD data)
        /// </summary>
        public bool UseHighLiquidityFillsEOD { get { return _useHighLiquidityFillsEOD; } set { _useHighLiquidityFillsEOD = value; } }
        FillModeType _fillmode = FillModeType.OwnBook;
        /// <summary>
        /// Gets or sets the fill mode this broker uses when executing orders
        /// </summary>
        public FillModeType FillMode { get { return _fillmode; } set { _fillmode = value; } }
        // # of pending orders
        private int _pendingorders = 0;
        private long _nextorderid = 0;

        /// <summary>
        /// accounts that the broker has
        /// </summary>
        public string[] Accounts
        {
            get
            {
                List<string> alist = new List<string>();
                Account[] accts = new Account[_masterorders.Count];
                _masterorders.Keys.CopyTo(accts, 0);
                for (int i = 0; i < accts.Length; i++)
                    alist.Add(accts[i].ID); return alist.ToArray(); 
            }
        }

        /// <summary>
        /// Executes any open orders allowed by the specified tick.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns>the number of orders executed using the tick.</returns>
        public int Execute(Tick tick)
        {
            if (_pendingorders == 0) return 0;
            if (!tick.IsTrade && !_usebidaskfill) return 0;
            int filledorders = 0;
            Account[] accts = new Account[_masterorders.Count];
            _masterorders.Keys.CopyTo(accts, 0);

            // go through each account
            for (int idx = 0; idx < accts.Length; idx++)
            { 
                Account a = accts[idx];
                // if account has requested no executions, skip it
                if (!a.Execute) continue;
                // make sure we have a record for this account
                if (!_mastertrades.ContainsKey(a.ID))
                    _mastertrades.Add(a.ID, new List<Trade>());
                // track orders being removed and trades that need notification
                List<int> notifytrade = new List<int>();
                List<int> remove = new List<int>();
                // go through each order in the account
                for (int i = 0; i < _masterorders[a].Count; i++)
                {
                    Order o = _masterorders[a][i];
                    //make sure tick is for the right stock
                    if (tick.FullSymbol != o.FullSymbol)
                        continue;
                    bool filled = false;
                    if (UseHighLiquidityFillsEOD)
                    {
                        Order oi = (Order)o;
                        filled = oi.FillHighLiquidityEOD(tick, _usebidaskfill, false);
                    }
                    else if (o.TIF <= TimeInForce.GTC)
                    {
                        filled = o.Fill(tick, _usebidaskfill, false); // fill our trade
                    }
                    else if (o.TIF == TimeInForce.OPG)
                    {
                        // if it's already opened, we missed our shot
                        if (_hasopened.Contains(o.FullSymbol))
                            continue;
                        // otherwise make sure it's really the opening
                        //if (tick.Exchange == OPGEX)
                        {
                            // it's the opening tick, so fill it as an opg
                            filled = o.Fill(tick, _usebidaskfill, true);
                            // mark this symbol as already being open
                            _hasopened.Add(tick.FullSymbol);
                        }

                    }
                    // other orders fill normally, except MOC orders which are at 4:00PM
                    else if (o.TIF == TimeInForce.MOC)
                    {
                        if (tick.Time >= 160000)
                            filled = o.Fill(tick, _usebidaskfill, false); // fill our trade
                    }
                    else
                        filled = o.Fill(tick, _usebidaskfill, false); // fill our trade

                    if (filled)
                    {
                        // get copy of trade for recording
                        Trade trade = new Trade((Trade)o);
                        
                        // remove filled size from size available in trade
                        if (_adjustincomingticksize)
                        {
                            if (_usebidaskfill)
                            {
                                if (o.Side)
                                    tick.AskSize -= trade.UnsignedSize;
                                else
                                    tick.BidSize -= trade.UnsignedSize;
                            }
                            else
                                tick.TradeSize -= trade.UnsignedSize;
                        }
                        
                        // if trade represents entire requested order, mark order for removal
                        if (trade.UnsignedSize == o.UnsignedSize)
                            remove.Add(i);
                        else // otherwise reflect order's remaining size
                            o.OrderSize = (o.UnsignedSize - trade.UnsignedSize) * (o.OrderSide ? 1 : -1);

                        // record trade
                        _mastertrades[a.ID].Add(trade);
                        // mark it for notification
                        notifytrade.Add(_mastertrades[a.ID].Count - 1);
                        // count the trade
                        filledorders++;
                    }
                }
                int rmcount = remove.Count;
                // remove the filled orders
                for (int i = remove.Count - 1; i >= 0; i--)
                    _masterorders[a].RemoveAt(remove[i]);
                // unmark filled orders as pending
                _pendingorders -= rmcount;
                if (_pendingorders < 0) _pendingorders = 0;
                // notify subscribers of trade
                if (a.Notify)
                    for (int tradeidx = 0; tradeidx < notifytrade.Count; tradeidx++)
                        OnGotFill(_mastertrades[a.ID][notifytrade[tradeidx]]);

            }
            return filledorders;
        }

        /// <summary>
        /// Best Bid/Ask for a symbol, based on the order book of the broker received from its client accounts
        /// </summary>
        public Order BestBidOrOffer(string symbol, bool side)
        {
            Order best = new Order();
            Order next = new Order();
            Account[] accts = new Account[_masterorders.Count];
            _masterorders.Keys.CopyTo(accts, 0);
            for (int i = 0; i < accts.Length; i++)
            {
                Account a = accts[i];
                // get our first order
                if (!best.IsValid)
                {
                    // if we don't have a valid one yet, check this account
                    best = new Order(BestBidOrOffer(symbol, side, a));
                    continue;  // keep checking the accounts till we find a valid one
                }
                // now we have our first order, which will be best if we can't find a second one
                next = new Order(BestBidOrOffer(symbol, side, a));
                if (!next.IsValid) continue; // keep going till we have a second order
                best = BestBidOrOffer(best, next); // when we have two, compare and get best
                // then keep fetching next valid order to see if it's better
            }
            return best; // if there's no more orders left, this is best
        }

        /// <summary>
        /// Find the best buy/sell order of an account
        /// </summary>
        public Order BestBidOrOffer(string sym, bool side, Account Account)
        {
            Order best = new Order(); best.Account = Account.ID;
            if (!_masterorders.ContainsKey(Account)) return best;
            List<Order> orders = _masterorders[Account];
            for (int i = 0; i < orders.Count; i++)
            {
                Order o = orders[i];
                if (o.FullSymbol != sym) continue;
                if (o.OrderSide != side) continue;
                if (!best.IsValid)
                {
                    best = new Order(o);
                    continue;
                }
                Order test = BestBidOrOffer(best, o);
                if (test.IsValid) best = new Order(test);
            }
            best.Account = Account.ID;
            return best;
        }

        // takes two orders and returns the better one
        // if orders aren't for same side or symbol or not limit, returns invalid order
        // if orders are equally good, adds them together
        public Order BestBidOrOffer(Order first, Order second)
        {
            if ((first.FullSymbol != second.FullSymbol) || (first.OrderSide != second.OrderSide) || !first.IsLimit || !second.IsLimit)
                return new Order(); // if not comparable return an invalid order
            if ((first.OrderSide && (first.LimitPrice > second.LimitPrice)) || // if first is better, use it
                (!first.OrderSide && (first.LimitPrice < second.LimitPrice)))
                return new Order(first);
            else if ((first.OrderSide && (first.LimitPrice < second.LimitPrice)) || // if second is better, use it
                (!first.OrderSide && (first.LimitPrice > second.LimitPrice)))
                return new Order(second);

            // if it's a tier then add the sizes
            Order add = new Order(first);
            add.OrderSize = add.OrderSize + second.OrderSize;
            return add;
        }

        /// <summary>
        /// Sends the order to the broker. (uses the default account)
        /// </summary>
        /// <param name="o">The order to be send.</param>
        /// <returns>status code</returns>
        public int SendOrder(Order o)
        {
            if (!o.IsValid) return -1;
            // make sure book is clearly stamped
            if (o.Account.Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
            {
                o.Account = _defaultaccount.ID;
                return SendOrderAccount(o, _defaultaccount);
            }
            // get account
            Account a;
            if (!_accountlist.TryGetValue(o.Account, out a))
            {
                a = new Account(o.Account);
                AddAccount(a);
            }
            return SendOrderAccount(o, a);
        }

        /// <summary>
        /// Sends the order to the broker for a specific account.
        /// </summary>
        /// <param name="o">The order to be sent.</param>
        /// <param name="a">the account to send with the order.</param>
        /// <returns>status code</returns>
        public int SendOrderAccount(Order o, Account a)
        {
            if (o.Id == 0) // if order id isn't set, set it
                o.Id = _nextorderid++;
            if (a.Notify)
                OnGotOrder(o);
            AddOrder(o, a);

            return 0;
        }

        /// <summary>
        /// conduct possible match against the broker's other accounts
        /// then add the rest to broker's book
        /// </summary>
        protected void AddOrder(Order o, Account a)
        {
            if (!a.isValid) throw new Exception("Invalid account provided"); // account must be good
            if ((_fillmode == FillModeType.OwnBook) && a.Execute)
            {
                // get best bid or offer from opposite side,
                // see if we can match against this BBO and cross locally
                Order match = BestBidOrOffer(o.FullSymbol, !o.OrderSide);

                // first we need to make sure the book we're matching to allows executions
                Account ma = new Account();
                try
                {
                    if (_accountlist.TryGetValue(match.Account, out ma) && ma.Execute)
                    {
                        // if it's allowed, try to match it
                        bool filled = o.Fill(match);
                        int avail = o.UnsignedSize;
                        // if it matched 
                        if (filled)
                        {
                            // record trade
                            Trade t = (Trade)o;
                            _mastertrades[a.ID].Add(t);
                            // notify the trade occured
                            OnGotFill(t);

                            // update the order's size (in case it was a partial fill)
                            o.OrderSize = (avail - Math.Abs(t.TradeSize)) * (o.OrderSide ? 1 : -1);

                            // if it was a full fill, no need to add order to the book
                            if (Math.Abs(t.TradeSize) == avail) return;
                        }
                    }
                }
                catch (ArgumentNullException) { } // no other side --> match is null
            }
            // add any remaining order to book as new liquidity route
            List<Order> tmp;
            // see if we have a book for this account
            if (!_masterorders.TryGetValue(a, out tmp))
            {
                tmp = new List<Order>();
                _masterorders.Add(a, tmp); // if not, create one
            }
            o.Account = a.ID; // make sure order knows his account
            tmp.Add(o); // record the order
            // increment pending count
            _pendingorders++;
        }

        protected void AddAccount(Account a)
        {
            Account t = null;
            if (_accountlist.TryGetValue(a.ID, out t)) return; // already had it
            _masterorders.Add(a, new List<Order>());
            _mastertrades.Add(a.ID, new List<Trade>());
            _accountlist.Add(a.ID, a);
        }

        public bool CancelOrder(long orderid)
        {
            bool worked = false;
            Account[] accts = new Account[_masterorders.Count];
            _masterorders.Keys.CopyTo(accts, 0);
            for (int i = 0; i < accts.Length; i++)
                worked |= CancelOrder(accts[i], orderid);
            return worked;
        }
        /// <summary>
        /// cancel order for specific account only
        /// </summary>
        /// <param name="a"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public bool CancelOrder(Account a, long orderid)
        {
            List<Order> orderlist = new List<Order>();
            if (!_masterorders.TryGetValue(a, out orderlist)) return false;
            int cancelidx = -1;
            for (int i = orderlist.Count - 1; i >= 0; i--) // and every order
                if (orderlist[i].Id == orderid) // if we have order with requested id
                {
                    if (a.Notify)
                    {
                        OnGotOrderCancelBroadcast(orderlist[i].FullSymbol, orderlist[i].OrderSide, orderid); //send cancel notifcation to any subscribers
                        OnGotOrderCancelConfirm(orderid);
                    }                        
                    cancelidx = i;
                }
            if (cancelidx == -1) return false;
            _masterorders[a].RemoveAt(cancelidx);
            return true;
        }

        public void CancelOrders()
        {
            Account[] accts = new Account[_masterorders.Count];
            _masterorders.Keys.CopyTo(accts, 0);
            for (int idx = 0; idx < accts.Length; idx++)
                CancelOrders(accts[idx]);
        }
        public void CancelOrders(Account a)
        {
            if (!_masterorders.ContainsKey(a)) return;
            List<Order> orders = _masterorders[a];
            for (int i = 0; i < orders.Count; i++)
            {
                Order o = orders[i];
                //send cancel notifcation to any subscribers
                if (a.Notify)
                {
                    OnGotOrderCancelConfirm(o.Id);
                    OnGotOrderCancelBroadcast(o.FullSymbol, o.OrderSide, o.Id);
                }
            }
            _masterorders[a].Clear();  // clear the account
        }

        /// <summary>
        /// Gets the complete execution list for this account
        /// </summary>
        /// <param name="a">account to request blotter from.</param>
        /// <returns></returns>
        public List<Trade> GetTradeList(Account a)
        { 
            List<Trade> res; 
            bool worked = _mastertrades.TryGetValue(a.ID, out res); 
            return worked ? res : new List<Trade>(); 
        }
        /// <summary>
        /// Gets the list of open orders for this account.
        /// </summary>
        /// <param name="a">Account.</param>
        /// <returns></returns>
        public List<Order> GetOrderList(Account a) 
        { 
            List<Order> res; 
            bool worked = _masterorders.TryGetValue(a, out res); 
            return worked ? res : new List<Order>(); 
        }
        public List<Trade> GetTradeList() { return GetTradeList(_defaultaccount); }
        public List<Order> GetOrderList() { return GetOrderList(_defaultaccount); }

        public enum FillModeType
        {
            OwnBook = 0,
            HistBookOnly = 1,
        }
    }
}
