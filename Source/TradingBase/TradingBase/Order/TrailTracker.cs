using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradingBase
{
    public delegate void HitOffsetDelegate(string sym, long id, decimal price);
    /// <summary>
    /// simulate a trailing stop for a number of positions
    /// trail StopDist in newTick() and then flat StopPercent
    /// </summary>
    public class TrailTracker
    {
        PositionTracker _pt = null;
        IdTracker _id = null;

        Dictionary<string, int> esize = new Dictionary<string, int>();
        Dictionary<string, int> firecount = new Dictionary<string, int>();

        /// <summary>
        /// default max fires per round turn
        /// </summary>
        public int MaxFireCount = 1;
        /// <summary>
        /// position tracker used by this component
        /// </summary>
        public PositionTracker pt { get { return _pt; } set { _pt = value; } }
        public IdTracker Id { get { return _id; } set { _id = value; } }
        /// <summary>
        /// creates trail tracker (with it's own position tracker)
        /// </summary>
        //public TrailTracker() : this(new PositionTracker(), new IdTracker()) { }
        /// <summary>
        /// creates a trail tracker from an existing position tracker component
        /// </summary>
        /// <param name="pt"></param>
        public TrailTracker(PositionTracker pt, IdTracker id)
        {
            _pt = pt;
            _id = id;
        }
        bool _trailbydefault = true;

        public event Action<string> SendDebug;
        public event HitOffsetDelegate HitOffset;
        public event Action<Order> SendOrder;
        void D(string msg) { if (SendDebug != null) SendDebug(msg); }
        /// <summary>
        /// gets or sets the trail amount for a given symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public OffsetInfo this[string sym]
        {
            get
            {
                // get index
                int idx = symidx(sym);
                // if not there, get default
                if (idx == NOSYM) return _trailbydefault ? new OffsetInfo(_defaulttrail) : new OffsetInfo(0, 0);
                // otherwise return whats there
                return _trail[idx];
            }
            set
            {
                // get index
                int idx = symidx(sym);
                // if not there, save this info
                if (idx == NOSYM)
                {
                    _symidx.Add(sym, _trail.Count);
                    _trail.Add(value);
                    _ref.Add(0);
                    firecount.Add(sym, 0);

                    esize.Add(sym, pt[sym].UnsignedSize);

                    _pendingfill.Add(false);
                }
                else // save it
                    _trail[idx] = value;
                D("trail changed: " + sym + " " + value.ToString());
            }
        }
        /// <summary>
        /// whether trailing stops are created by default for any symbol seen
        /// </summary>
        public bool TrailByDefault { get { return _trailbydefault; } set { _trailbydefault = value; } }
        OffsetInfo _defaulttrail = new OffsetInfo(0, 0);
        /// <summary>
        /// when TrailByDefault is enabled, default trail amount is used for symbols that do not have a trail configured
        /// </summary>
        public OffsetInfo DefaultTrail { get { return _defaulttrail; } set { _defaulttrail = value; } }
        Dictionary<string, int> _symidx = new Dictionary<string, int>();
        List<OffsetInfo> _trail = new List<OffsetInfo>();
        List<decimal> _ref = new List<decimal>();
        List<bool> _pendingfill = new List<bool>();
        const int NOSYM = -1;
        int symidx(string sym)
        {
            int idx = NOSYM;
            if (_symidx.TryGetValue(sym, out idx))
                return idx;
            return NOSYM;
        }
        bool _valid = true;
        /// <summary>
        /// set to true if trailing stop are used, false if not
        /// </summary>
        public bool isValid { get { return _valid; } set { _valid = value; } }
        /// <summary>
        /// must pass ticks as received to this function, in order to have trailing stops executed at proper time.
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            // see if we're turned on
            if (!isValid)
                return;
            // see if we can exit when trail is broken
            if (SendOrder == null)
                return;
            // see if we have anything to trail against
            if (_pt[k.FullSymbol].isFlat)
                return;
            // // pass along as point
            if (k.IsTrade && !UseBidAskExitPrices)
                newPoint(k.FullSymbol, k.TradePrice);
            if (UseBidAskExitPrices && _pt[k.FullSymbol].isLong && k.HasBid)
                newPoint(k.FullSymbol, k.BidPrice);
            else if (UseBidAskExitPrices && _pt[k.FullSymbol].isShort && k.HasAsk)
                newPoint(k.FullSymbol, k.AskPrice);
        }



        bool _usebidask = false;
        /// <summary>
        /// uses bid/ask rather than last trade to price trailing stop amount
        /// </summary>
        bool UseBidAskExitPrices { get { return _usebidask; } set { _usebidask = value; } }
        /// <summary>
        /// pass arbitrary price to use for trail reference price
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="p"></param>
        public void newPoint(string symbol, decimal p)
        {
            // get index for symbol
            int idx = symidx(symbol);
            // setup parameters
            OffsetInfo trail = null;
            decimal refp = 0;
            // see if we trail this symbol
            if ((idx == NOSYM) && _trailbydefault)
            {
                // get parameters
                idx = _trail.Count;
                refp = p;
                trail = new OffsetInfo(_defaulttrail);
                // save them
                _symidx.Add(symbol, idx);
                _ref.Add(refp);
                _pendingfill.Add(false);
                firecount.Add(symbol, 0);

                esize.Add(symbol, pt[symbol].UnsignedSize);

                // just in case user is modifying on seperate thread
                lock (_trail)
                {
                    _trail.Add(trail);
                }
                D(symbol + " trail tracking modified: " + trail.ToString());
            }
            else if ((idx == NOSYM) && !_trailbydefault)
            {
                return;
            }
            else
            {
                // get parameters
                refp = _ref[idx];
                // in case tracker started after trail stop should have been broken.
                if (refp == 0 && _pt[symbol].isValid)
                {
                    refp = _pt[symbol].AvgPrice;
                }
                // just in case user tries to modify on seperate thread
                lock (_trail)
                {
                    trail = _trail[idx];
                }
            }

            // see if we need to update ref price
            if ((refp == 0)
                || (_pt[symbol].isLong && (refp < p))
                || (_pt[symbol].isShort && (refp > p)))
            {
                // update
                refp = p;
                // save it
                _ref[idx] = refp;
                // notify
                v(symbol + " new reference price: " + p);
            }

            // see if we broke our trail
            var testdist = Math.Abs(refp - p);
            var trailtest = testdist > trail.StopDist;
            if (!_pendingfill[idx] && (trail.StopDist != 0) && trailtest && (MaxFireCount > firecount[symbol]))
            {
                // notify
                D(symbol + " hit trailing stop at: " + p.ToString("F2"));
                // mark pending order
                _pendingfill[idx] = true;
                // get order
                Order flat = new MarketOrderFlat(_pt[symbol], trail.StopPercent, trail.NormalizeSize, trail.MinimumLotSize);
                // get order id
                flat.Id = _id.NextOrderId;
                // adjust expectation
                esize[symbol] -= flat.UnsignedSize;
                // count fire
                firecount[symbol]++;
                // send flat order
                SendOrder(flat);
                // notify
                D(symbol + " enforcing trail with: " + flat.ToString() + " esize: " + esize[symbol] + " count: " + firecount[symbol]);
                if (HitOffset != null)
                    HitOffset(symbol, flat.Id, p);
            }
            else if (!_noverb)
            {
                if (_pendingfill[idx])
                    v(symbol + " waiting for trail fill.");
                else if (trail.StopDist == 0)
                    v(symbol + " trail has been disabled.");
                else if (!trailtest)
                {
                    v(symbol + " trail not hit, current dist: " + testdist + " trailamt: " + trail.StopDist);
                }
                else if (MaxFireCount > firecount[symbol])
                {
                    v(symbol + " trail max fire reached at: " + firecount[symbol] + " max: " + MaxFireCount);
                }
            }
        }

        void v(string msg)
        {
            if (_noverb)
                return;
            D(msg);
        }

        bool _noverb = true;

        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        /// <summary>
        /// this must be called once per position tracker, for each position update.
        /// if you are using your own position tracker with this trailing stop(eg from offset tracker, or somewhere else)
        /// you only need to adjust it once, so if you adjust it directly you don't need to call again here.
        /// </summary>
        /// <param name="p"></param>
        public void Adjust(Position p)
        {
            _pt.Adjust(p);
        }

        /// <summary>
        /// this must be called once per position tracker, for each position update.
        /// if you are using your own position tracker with this trailing stop(eg from offset tracker, or somewhere else)
        /// you MUST call TrailTrackers Adjust and NOT call your position tracker's adjust
        /// </summary>
        /// <param name="fill"></param>
        public void Adjust(Trade fill)
        {
            // get index for symbol
            int idx = symidx(fill.FullSymbol);

            // only do following if we're tracking trail for this symbol
            if (idx != NOSYM)
            {
                // get current position size
                int psize = _pt[fill.FullSymbol].UnsignedSize;
                // get trailing information
                OffsetInfo trail = _trail[idx];
                // get actual position size after change
                int asize = psize - fill.UnsignedSize;
                // if expected and actual match, mark pending as false
                if (esize[fill.FullSymbol] == asize)
                {
                    D(fill.FullSymbol + " trailing stop completely filled with: " + fill.ToString());
                    _pendingfill[idx] = false;
                }
                else
                    v(fill.FullSymbol + " trail partial fill: " + fill.ToString() + " e: " + esize[fill.FullSymbol] + " != a: " + asize);

            }
            else
                v(fill.FullSymbol + " fill: " + fill.ToString() + " ignored while trail disabled.");

            _pt.Adjust(fill);
            // if we're flat now, make sure ref price is reset
            if (_pt[fill.FullSymbol].isFlat)
            {
                _ref[idx] = 0;
                v(fill.FullSymbol + " flat, reset trail reference price.");
            }
        }
    }
}
