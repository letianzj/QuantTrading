using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBase
{
    public class OffsetTracker
    {
        public void GotTick(Tick k) { newTick(k); }
        public event Action<string> SendDebug;
        public event HitOffsetDelegate HitOffset;
        void debug(string msg) { if (SendDebug != null) SendDebug(msg); }
        OffsetInfo _default = new OffsetInfo();
        string[] _ignore = new string[0];
        /// <summary>
        /// default offset used by the tracker, in the event no custom offset is set. eg ot["IBM"] = new OffsetInfo();
        /// </summary>
        public OffsetInfo DefaultOffset { get { return new OffsetInfo(_default); } set { value.ProfitId = 0; value.StopId = 0; _default = value; } }
        bool _ignoredefault = false;
        private Dictionary<string, OffsetInfo> _trackedOffsetInfos = new Dictionary<string, OffsetInfo>();      // fullsymbol --> offsetinfo
        /// <summary>
        /// ignore symbols by default.   if true... a symbol has no custom offset defined will be ignored (regardless of ignore list).  the default is false.
        /// </summary>
        public bool IgnoreDefault { get { return _ignoredefault; } set { _ignoredefault = value; } }
        /// <summary>
        /// always ignore these symbols.   this list is only in affect when IgnoreDefault is false.
        /// </summary>
        public string[] IgnoreSyms { get { return _ignore; } set { _ignore = value; } }
        bool _hasevents = false;
        public event Action<Order> SendOrderEvent;
        public event Action<long> SendCancelEvent;
        PositionTracker _pt;
        /// <summary>
        /// a position tracker you can reuse 
        /// </summary>
        public PositionTracker PositionTracker { get { return _pt; } set { _pt = value; } }
        public OffsetTracker() { }
        IdTracker _ids = new IdTracker();
        /// <summary>
        /// id tracker used by offsettracker, you can reuse in other apps you use OT.
        /// </summary>
        public IdTracker Ids { get { return _ids; } set { _ids = value; } }
        public OffsetTracker(IdTracker tracker, int capacity)
        {
            _ids = tracker;
            _pt = new PositionTracker(capacity);
        }

        int _debdecimals = 2;
        /// <summary>
        /// number of decimal places in SendDebug events
        /// (defaults to 2, set to 5 for forex)
        /// </summary>
        public int DebugDecimals { get { return _debdecimals; } set { _debdecimals = value; } }

        /// <summary>
        /// clear single custom offset
        /// </summary>
        /// <param name="sym"></param>
        public void ClearCustom(string sym) { this[sym] = IgnoreDefault ? OffsetInfo.DISABLEOFFSET() : DefaultOffset; }

        object _lock = new object();


        void doupdate(string sym)
        {
            // is update ignored?
            if (IgnoreUpdate(sym)) return;
            // wait till next tick if we send cancels
            bool sentcancel = false;
            // get our offset values
            OffsetInfo off = GetOffset(sym);
            // get position
            Position p = new Position(_pt[sym]);
            // if we're flat, nothign to do
            if (p.isFlat) return;
            // see whats current
            bool cprofit = off.isProfitCurrent(p);
            bool cstop = off.isStopCurrent(p);
            // if not current, mark for update
            bool updateprofit = !cprofit;
            bool updatestop = !cstop;
            // if we're up to date then quit
            if (!updatestop && !updateprofit) return;
            // see if we have stop update
            if ((updatestop && off.hasStop && !CancelOnce)
                || (updatestop && off.hasStop && CancelOnce && !off.StopcancelPending))
            {
                // notify
                if (!off.StopcancelPending)
                    debug(string.Format("attempting stop cancel: {0} {1}", sym, off.StopId));
                // cancel existing stops
                cancel(off.StopId);
                // mark cancel pending
                off.StopcancelPending = true;
                // mark as sent
                sentcancel |= true;
            }
            // see if we have profit update
            if ((updateprofit && off.hasProfit && AllowSimulatenousCancels) ||
                (updateprofit && off.hasProfit && AllowSimulatenousCancels && !sentcancel))
            {
                if (!CancelOnce || (CancelOnce && !off.ProfitcancelPending))
                {
                    // notify
                    if (!off.ProfitcancelPending)
                        debug(string.Format("attempting profit cancel: {0} {1}", sym, off.ProfitId));
                    // cancel existing profits
                    cancel(off.ProfitId);
                    // mark cancel pending
                    off.ProfitcancelPending = true;
                    // mark as sent
                    sentcancel |= true;
                }
            }

            // wait till next tick if we sent cancel
            if (sentcancel && WaitAfterCancel)
                return;
            bool sentorder = false;
            // send stop first
            if (!off.hasStop)
            {
                // since we have no stop, it's cancel can't be pending
                off.StopcancelPending = false;
                // get new stop
                Order stop = Calc.PositionStop(p, off.StopDist, off.StopPercent, off.NormalizeSize, off.MinimumLotSize);
                // mark size
                off.SentStopSize = stop.OrderSize;
                // if it's valid, send and track
                if (stop.IsValid)
                {
                    stop.Id = Ids.NextOrderId;
                    off.StopId = stop.Id;
                    SendOrderEvent(stop);
                    // notify
                    debug(string.Format("sent new stop: {0} {1}", stop.Id, stop.ToString(DebugDecimals)));
                    sentorder = true;
                }
                else if (_verbdebug)
                {
                    debug(sym + " invalid stop: " + stop.ToString(DebugDecimals));
                }

            }

            if ((!off.hasProfit && AllowSimulatenousOrders) || (!off.hasProfit && !AllowSimulatenousOrders && !sentorder))
            {
                // since we have no stop, it's cancel can't be pending
                off.ProfitcancelPending = false;
                // get new profit
                Order profit = Calc.PositionProfit(p, off.ProfitDist, off.ProfitPercent, off.NormalizeSize, off.MinimumLotSize);
                // mark size
                off.SentProfitSize = profit.OrderSize;
                // if it's valid, send and track it
                if (profit.IsValid)
                {
                    profit.Id = Ids.NextOrderId;
                    off.ProfitId = profit.Id;
                    SendOrderEvent(profit);
                    // notify
                    debug(string.Format("sent new profit: {0} {1}", profit.Id, profit.ToString(DebugDecimals)));
                    sentorder = true;
                }
                else if (_verbdebug)
                {
                    debug(sym + " invalid profit: " + profit.ToString(DebugDecimals));
                }
            }
            // make sure new offset info is reflected
            SetOffset(sym, off);

        }

        bool _cancelonce = false;
        /// <summary>
        /// only cancel an offset once per update
        /// </summary>
        public bool CancelOnce { get { return _cancelonce; } set { _cancelonce = value; } }

        bool _waitaftercancel = true;
        /// <summary>
        /// wait till next tick after sending cancel orders
        /// </summary>
        public bool WaitAfterCancel { get { return _waitaftercancel; } set { _waitaftercancel = value; } }

        bool _sendsametime = true;
        /// <summary>
        /// allow stops and profit offsets to be sent at same time
        /// </summary>
        public bool AllowSimulatenousOrders { get { return _sendsametime; } set { _sendsametime = value; } }

        bool _cancelsametime = true;
        /// <summary>
        /// allow stop and profit cancels to be sent at same time
        /// </summary>
        public bool AllowSimulatenousCancels { get { return _cancelsametime; } set { _cancelsametime = value; } }

        bool hascustom(string symbol)
        {
            return _trackedOffsetInfos.ContainsKey(symbol);
        }

        void cancel(OffsetInfo offset)
        {

            bool hit = false;
            string ids = string.Empty;
            if (offset.hasProfit)
            {
                hit |= true;
                ids += offset.ProfitId + " ";
                cancel(offset.ProfitId);
            }
            if (offset.hasStop)
            {
                hit |= true;
                ids += offset.StopId + " ";
                cancel(offset.StopId);
            }
            if (hit)
                debug("canceling offsets: " + ids);

        }
        void cancel(long id) { if (id != 0) SendCancelEvent(id); }
        /// <summary>
        /// cancels all offsets (profit+stops) for given side
        /// </summary>
        /// <param name="side"></param>
        public void CancelAll(bool side)
        {
            debug("canceling offsets for: " + (side ? "long" : "short"));
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel all offsets for side
                if (p.isLong == side)
                    cancel(GetOffset(p.FullSymbol));
            }
        }
        /// <summary>
        /// cancels all offsets for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelAll(string sym)
        {

            bool hit = false;
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.FullSymbol == sym)
                {
                    hit |= true;
                    cancel(GetOffset(sym));
                }
            }
            if (hit)
                debug("canceling offsets for: " + sym);

        }

        /// <summary>
        /// cancels only profit orders for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelProfit(string sym)
        {
            debug("canceling profits for: " + sym);
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.FullSymbol == sym)
                    cancel(GetOffset(sym).ProfitId);
            }
        }

        /// <summary>
        /// cancels only stops for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelStop(string sym)
        {
            debug("canceling stops for: " + sym);
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.FullSymbol == sym)
                    cancel(GetOffset(sym).StopId);
            }
        }

        /// <summary>
        /// cancel profits for side (long is true, false is short)
        /// </summary>
        /// <param name="side"></param>
        public void CancelProfit(bool side)
        {
            debug("canceling profits for: " + (side ? "long" : "short"));
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel profits for side
                if (p.isLong == side)
                    cancel(GetOffset(p.FullSymbol).ProfitId);
            }
        }

        /// <summary>
        /// cancel stops for a side (long is true, false is short)
        /// </summary>
        /// <param name="side"></param>
        public void CancelStop(bool side)
        {
            debug("canceling stops for: " + (side ? "long" : "short"));
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel stops for side
                if (p.isLong == side)
                    cancel(GetOffset(p.FullSymbol).StopId);
            }
        }

        /// <summary>
        /// cancels all tracked offsets
        /// </summary>
        public void CancelAll()
        {
            debug("canceling all pending offsets");

            foreach (KeyValuePair<string, OffsetInfo> kvp in _trackedOffsetInfos)
            {
                cancel(kvp.Value);
            }
        }


        bool HasEvents()
        {
            if (_hasevents) return true;
            if ((SendCancelEvent == null) || (SendOrderEvent == null))
                throw new Exception("You must define targets for SendCancel and SendOffset events.");
            _hasevents = true;
            return _hasevents;
        }

        bool IgnoreUpdate(string sym)
        {
            // if updates are ignored by default
            if (_ignoredefault) // see if we have custom offset
                return !hascustom(sym);
            // otherwise see if it's specifically ignored
            foreach (string isym in _ignore)
                if (sym == isym)
                    return true;
            return false;
        }

        public void ClearCustom()
        {
            _trackedOffsetInfos.Clear();
        }

        long ProfitId(string sym)
        {
            // if we have offset, return it's id
            if (_trackedOffsetInfos.ContainsKey(sym))
            {
                return _trackedOffsetInfos[sym].ProfitId;
            }
            else // no offset id
            {
                return 0;
            }
        }

        long StopId(string sym)
        {
            // if we have offset, return it's id
            if (_trackedOffsetInfos.ContainsKey(sym))
            {
                return _trackedOffsetInfos[sym].StopId;
            }
            else // no offset id
            {
                return 0;
            }
        }

        bool _shutonreinit = true;
        /// <summary>
        /// if a position is provided twice in the same session, assume this is bad and cancel/shutdown offsets.
        /// </summary>
        public bool ShutdownOnReinit { get { return _shutonreinit; } set { _shutonreinit = value; } }

        public void GotPosition(Position p) { Adjust(p); }
        public void GotFill(Trade f) { Adjust(f); }

        /// <summary>
        /// must send new positions here (eg from GotPosition on Response)
        /// </summary>
        /// <param name="p"></param>
        public void Adjust(Position p)
        {
            // did position exist?
            bool exists = !_pt[p.FullSymbol].isFlat;
            if (exists)
                debug(p.FullSymbol + " re-initialization of existing position");
            if (exists && ShutdownOnReinit)
            {
                // get offset
                OffsetInfo oi = GetOffset(p.FullSymbol);
                // disable it
                oi.ProfitPercent = 0;
                oi.StopPercent = 0;
                // save it
                SetOffset(p.FullSymbol, oi);
                // cancel existing orders
                CancelAll(p.FullSymbol);
                // stop processing
                return;
            }
            // update position
            _pt.Adjust(p);
            // if we're flat, nothing to do
            if (_pt[p.FullSymbol].isFlat)
            {
                debug(p.FullSymbol + " initialized to flat.");
                // cancel pending offsets
                CancelAll(p.FullSymbol);
                // reset offset state but not configuration
                SetOffset(p.FullSymbol, new OffsetInfo(this[p.FullSymbol]));
                return;
            }
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(p.FullSymbol);
        }

        /// <summary>
        /// must send new fills here (eg call from Response.GotFill)
        /// </summary>
        /// <param name="t"></param>
        public void Adjust(Trade t)
        {
            // get original size
            int osize = _pt[t.FullSymbol].Size;
            // update position
            _pt.Adjust(t);
            // see if it's our order
            OffsetInfo oi = GetOffset(t.FullSymbol);
            // see what was hit
            bool hp = (t.Id != 0) && (oi.ProfitId == t.Id);
            bool hs = (t.Id != 0) && (oi.StopId == t.Id);
            // if we hit something
            if (hp || hs)
            {
                // notify
                debug(t.FullSymbol + " hit " + (hp ? "profit" : "stop") + ": " + t.Id);
                // see if we should clear offset
                if (hp && (oi.SentProfitSize == t.TradeSize))
                {
                    debug(t.FullSymbol + " profit closed: " + t.Id);
                    oi.ProfitId = 0;
                }
                else if (hp)
                    oi.SentProfitSize -= t.TradeSize;


                if (hs && (oi.SentStopSize == t.TradeSize))
                {
                    debug(t.FullSymbol + " stop closed: " + t.Id);
                    oi.StopId = 0;
                }
                else if (hs)
                    oi.SentStopSize -= t.TradeSize;

                if (HitOffset != null)
                    HitOffset(t.FullSymbol, t.Id, t.TradePrice);
            }
            // if we're flat, nothing to do (or if we switched sides)
            Position p = _pt[t.FullSymbol];
            if (p.isFlat || (osize * p.Size < -1))
            {
                if (p.isFlat)
                    debug(t.FullSymbol + " now flat.");
                else
                    debug(t.FullSymbol + " reversed: " + osize + " -> " + p.Size);
                CancelAll(t.FullSymbol);
                // reset offset state but not configuration
                SetOffset(t.FullSymbol, new OffsetInfo(this[t.FullSymbol]));
            }
            else // save offset
                SetOffset(t.FullSymbol, oi);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(t.FullSymbol);

        }

        /// <summary>
        /// obtain curretn offset information for a symbol.
        /// if no custom value has been set, use default
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public new OffsetInfo this[string symbol] { get { return GetOffset(symbol); } set { SetOffset(symbol, value); } }

        bool _addcust = false;
        public bool AddCustom { get { return _addcust; } set { _addcust = value; } }

        OffsetInfo GetOffset(string sym)
        {
            // see if we have a custom offset
            OffsetInfo oi = null;

            if (_trackedOffsetInfos.ContainsKey(sym))
            {
                oi = _trackedOffsetInfos[sym];
                if (oi == null)
                {
                    _trackedOffsetInfos[sym] = DefaultOffset;
                    oi = DefaultOffset;
                }     
            }
            else
            {
                oi = DefaultOffset;
                _trackedOffsetInfos.Add(sym, oi);
            }

            return oi;
        }

        bool _verbdebug = false;
        /// <summary>
        /// enable verbose debugging messages
        /// </summary>
        public bool VerboseDebugging { get { return _verbdebug; } set { _verbdebug = value; } }

        void SetOffset(string sym, OffsetInfo off)
        {
            // check for index
            if (_trackedOffsetInfos.ContainsKey(sym))
                _trackedOffsetInfos[sym] = off;
            else
                _trackedOffsetInfos.Add(sym, off);

            if (_verbdebug)
                debug(sym + " set offset: " + off.ToString(DebugDecimals));
        }

        /// <summary>
        /// should be called from GotCancel, when cancels arrive from broker.
        /// </summary>
        /// <param name="id"></param>
        public void GotCancel(long id)
        {
            // find any matching orders and reflect them as canceled
            var syms = _trackedOffsetInfos.Keys.ToArray();
            for (int i = 0; i < syms.Length; i++)
            {
                var sym = syms[i];
                // verify it exists
                if (string.IsNullOrWhiteSpace(sym))
                {
                    // should never happen
                    debug("empty or null symbol at offsettracker index#" + i);
                    continue;
                }
                // get offset
                var oi = GetOffset(sym);
                // verify it's set
                if (oi == null)
                {
                    // should never happen
                    debug(sym + " null offsetinfo at offsettracker index#" + i);
                    continue;
                }
                if (oi.StopId == id)
                {
                    debug(string.Format("stop canceled: {0} {1}", sym, id.ToString()));
                    _trackedOffsetInfos[sym].StopId = 0;
                }
                else if (oi.ProfitId == id)
                {
                    debug(string.Format("profit canceled: {0} {1}", sym, id.ToString()));
                    _trackedOffsetInfos[sym].ProfitId = 0;
                }
            }

        }
        /// <summary>
        /// should be called from GotTick, when ticks arrive.
        /// If cancels are not processed on fill updates, they will be resent each tick until they are processed.
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            // otherwise update the offsets for this tick's symbol
            doupdate(k.FullSymbol);
        }

        // track offset ids
        Dictionary<string, long> _profitids = new Dictionary<string, long>();
        Dictionary<string, long> _stopids = new Dictionary<string, long>();
        
    }
}
