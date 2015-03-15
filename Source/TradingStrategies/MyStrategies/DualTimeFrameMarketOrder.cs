// Dual Time Frame momentum/trend following Strategy
// In High Probability Trading Strategies by Robert Miner
// Market order version
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using TradingBase;
using TechnicalAnalysisEngine;

namespace MyStrategies
{
    //************************* Implementation **********************//
    // (0) Look at the higher time frame
    // Bull, not OB:        Long following a bullish reversal as long as bullish reversal is made below the OB zone.
    // Bull, OB:            No new long position. Possible short position following a bearish reversal.
    // Bear, not OS:        Short following a bearish reversal as long as bearish reversal is made above the OS zone.
    // Bear, OS:            No new short position. Possible long position following a bullish reversal.
    // (a) Entry strategy 1: trailing one-bar entry and stop
    // (initial version the protective buy/sell is placed at previous swing high/low)
    // (The capital exposure may be high in that case)
    // Long: set buy-stop one tick above the high, protective sell one tick below the low of the last completed bar
    // Short: set sell-stop one tick below the low, protective buy one tick above the high of the last completed bar
    // If the momentum makes an opposite reversal or reaches the OB/OS zone before the trailing 1 bar is taken out,
    // the entry is voided.
    // (b) Exit strategy one unit: opposite to the entry strategy
    // Immediate exit on opposite direction or wait until it turns if same direction

    // (c) # of flips for high and low time frames track # that indicators flips from long to short or vice versa;
    //      it tells initialization in that if no flips at low frame yet then no trade is entered.
    // (d) OrderId tells the order status as well: 
    //      PrimaryOrderId = 0 --> no orders
    //      PrimaryOrderId != 0 --> primary order submitted
    //      ComplimentaryOrderId = 0 --> waiting for primary order to be filled
    //      ComplimentaryOrderId != 0 --> complementary order submitted
    public class DualTimeFrameMarketOrder : StrategyBase
    {
        //************** parameters of this system *****************************//
        #region Parameters
        decimal _totalprofit = 500;
        [Description("Total profit target")]
        public decimal TotalProfitTarget { get { return _totalprofit; } set { _totalprofit = value; } }

        decimal _totalloss = -200;
        [Description("Total loss tolerance")]
        public decimal TotalLossTolerance { get { return _totalloss; } set { _totalloss = value; } }

        int _transactions = 0;
        int _totaltransactions = 6;
        public int TotalTransactions { get { return _totaltransactions; } set { _totaltransactions = value; } }

        int _tradesize = 1;
        [Description("Entry/Exit trade size")]
        public int TradeSize { get { return _tradesize; } set { _tradesize = value; } }

        decimal _traingamount = 2m;
        [Description("Trailing amount in trailing order")]
        public decimal TrailingAmount { get { return _traingamount; } set { _traingamount = value; } }

        int _starttime = 93000;
        [Description("System start time")]
        public int StartTime { get { return _starttime; } set { _starttime = value; } }

        int _shutdowntime = 161500;
        [Description("System shutdown time")]
        public int ShutdownTime { get { return _shutdowntime; } set { _shutdowntime = value; } }

        // strategy specifics
        int _higherbarinterval = 600;
        [Description("Higher bar interval in seconds")]
        public int HigherBarInterval { get { return _higherbarinterval; } set { _higherbarinterval = value; } }

        int _lowerbarinterval = 60;
        [Description("Lower bar interval in seconds")]
        public int LowerBarInterval { get {return _lowerbarinterval;} set {_lowerbarinterval = value;}}

        int _higherbarsilookback = 14;
        [Description("Higher bar RSI lookback")]
        public int HigherBarRSILookback { get { return _higherbarsilookback; } set { _higherbarsilookback = value; } }

        int _higherbarmacdfast = 12;
        [Description("Higher bar MACD fast lookback")]
        public int HigherBarMACDFast { get { return _higherbarmacdfast; } set { _higherbarmacdfast = value; } }

        int _higherbarmacdslow = 26;
        [Description("Higher bar MACD slow lookback")]
        public int HigherBarMACDSlow { get { return _higherbarmacdslow; } set { _higherbarmacdslow = value; } }

        int _higherbarmacdsignal = 9;
        [Description("Higher bar MACD signal lookback")]
        public int HigherBarMACDSignal { get { return _higherbarmacdsignal; } set { _higherbarmacdsignal = value; } }

        int _higherbaroversell = 30;
        [Description("Higher bar oversell (overbought = 100 - x")]
        public int HigherBarOversell { get { return _higherbaroversell; } set { _higherbaroversell = value; } }

        #endregion
        //************** end of parameters *****************************//

        #region member variables
        private PositionTracker _positiontracker;
        private BarListTracker _barlisttracker;
        private OrderTracker _ordertracker;

        bool[] _issymbolactive;
        bool[] _waittobefilled;
        OBOSZone[] _isOBOSZone;
        BullBearTrend[] _isHigherTimeFrameBullBear;
        BullBearTrend[] _isLowerTimeFrameBullBear;

        long[] _currentorderids;
        decimal[] _entrylevel;
        decimal[] _exitlevel;

        int _currenttime = 0;       // keep track of time
        #endregion

        public DualTimeFrameMarketOrder()
        {
            _name = "Dual Time Frame Market Order";
            Indicators = new string[] { "Time", "EMA" };

            _symbols.Add("SDS STK SMART");
            _symbols.Add("SPY STK SMART");
            _symbols.Add("VXX STK SMART");
            _symbols.Add("ESH5 FUT GLOBEX 50");
        }

        #region response
        void _barlisttracker_GotNewBar(string sym, int interval)
        {
            int idx = _symbols.IndexOf(sym);

            if (!_isactive || !_issymbolactive[idx])
            {
                return;
            }

            if (interval == _higherbarinterval)
            {
                BarList bl = _barlisttracker[sym, interval];

                double[] closes = Calc.Decimal2Double(bl.Close());
                // when NewBar is triggered, the latest bar only has one tick
                closes = closes.Take(closes.Length - 1).ToArray();

                // RSI: Overbought/Oversold
                RSIResult rsiresult = AnalysisEngine.RSI(closes, _higherbarsilookback);
                if (rsiresult.Values.Count == 0)        // Not enough bars
                    return;
                else if (rsiresult.Values.Last() < _higherbaroversell / 100)    // Oversold
                {
                    _isOBOSZone[idx] = OBOSZone.OverSoldZone;
                }
                else if (rsiresult.Values.Last() > 1 - _higherbaroversell / 100)    // Overbought
                {
                    _isOBOSZone[idx] = OBOSZone.OverboughtZone;
                }
                else { _isOBOSZone[idx] = OBOSZone.None; }

                // MACD: Bull/Bear
                MACDResult macdresult = AnalysisEngine.MACD(closes, _higherbarmacdfast, _higherbarmacdslow, _higherbarmacdsignal);
                if (macdresult.Signal.Count == 0)           // not enough bars
                    return;
                else if (macdresult.MACD.Last() > macdresult.Signal.Last())
                {
                    // record the switch to bull
                    if (_isHigherTimeFrameBullBear[idx] != BullBearTrend.Bull)
                    {
                        // get the last bar, and record the entry/exit point
                        int count = bl.IntervalCount(interval);
                        Bar lastbar = bl[count - 2, interval];
                        _entrylevel[idx] = lastbar.High;
                        _exitlevel[idx] = lastbar.High - _traingamount;
                    }
                    _isHigherTimeFrameBullBear[idx] = BullBearTrend.Bull;
                }
                else if (macdresult.MACD.Last() < macdresult.Signal.Last())
                {
                    // record the switch to bear
                    if (_isHigherTimeFrameBullBear[idx] != BullBearTrend.Bear)
                    {
                        // get the last bar, and record the entry/exit point
                        int count = bl.IntervalCount(interval);
                        Bar lastbar = bl[count - 2, interval];
                        _entrylevel[idx] = lastbar.Low;
                        _exitlevel[idx] = lastbar.Low + _traingamount;
                    }
                    _isHigherTimeFrameBullBear[idx] = BullBearTrend.Bear;
                }
                else { _isHigherTimeFrameBullBear[idx] = BullBearTrend.None; }
            }
            else if (interval == _lowerbarinterval)
            {
                BarList bl = _barlisttracker[sym, interval];
                double[] closes = Calc.Decimal2Double(bl.Close());
                // when NewBar is triggered, the latest bar only has one tick
                closes = closes.Take(closes.Length - 1).ToArray();

                // MACD: Bull/Bear
                MACDResult macdresult = AnalysisEngine.MACD(closes, _higherbarmacdfast, _higherbarmacdslow, _higherbarmacdsignal);
                if (macdresult.Signal.Count == 0)           // Not enough bars
                    return;
                else if (macdresult.MACD.Last() > macdresult.Signal.Last())
                {
                    _isLowerTimeFrameBullBear[idx] = BullBearTrend.Bull;
                }
                else if (macdresult.MACD.Last() < macdresult.Signal.Last())
                {
                    _isLowerTimeFrameBullBear[idx] = BullBearTrend.Bear;
                }
                else { _isLowerTimeFrameBullBear[idx] = BullBearTrend.None; }
            }
        }

        public override void GotPosition(Position p)
        {
            if (_symbols.Contains(p.FullSymbol))
                _positiontracker.Adjust(p);
        }

        public override void GotFill(Trade f)
        {
            if (_symbols.Contains(f.FullSymbol))
            {
                _positiontracker.Adjust(f);
                // int idx = Array.IndexOf(_symbols, f.FullSymbol);
                int idx = _symbols.IndexOf(f.FullSymbol);
                _waittobefilled[idx] = false;

                // Add to OrderTracker

                _transactions++;
                if (_transactions >= _totaltransactions)
                {
                    Shutdown();
                }
            }
        }

        public override void GotOrder(Order o)
        {
            SendDebug("order accepted: " + o.Id);
        }

        public override void GotOrderCancel(long id)
        {
            SendDebug("Order canceled: " + id);
        }

        public override void GotTick(Tick k)
        {
            _currenttime = k.Time;
            if (_currenttime > _shutdowntime)
            {
                Shutdown();
                return;
            }

            _barlisttracker.NewTick(k);             // blt rejects symbols that it doesn't track

            // ignore anything that is not a trade
            if (!k.IsTrade) return;
            
            // exit condition
            if ((Calc.OpenPL(k.TradePrice, _positiontracker[k.FullSymbol]) + _positiontracker[k.FullSymbol].ClosedPL > TotalProfitTarget) ||
                (Calc.OpenPL(k.TradePrice, _positiontracker[k.FullSymbol]) + _positiontracker[k.FullSymbol].ClosedPL < TotalLossTolerance))
            {
                Shutdown(k.FullSymbol);
            }

            // potential orders
            int idx = _symbols.IndexOf(k.FullSymbol);
            //if (_isHigherTimeFrameBullBear[idx] == _isLowerTimeFrameBullBear[idx])
            {

            }
        }
        #endregion

        #region other
        public override void Reset(bool popup = true)
        {
            _positiontracker = new PositionTracker(_symbols.Count);
            _barlisttracker = new BarListTracker(_symbols.ToArray(), new int[] { _higherbarinterval, _lowerbarinterval });
            _barlisttracker.GotNewBar += _barlisttracker_GotNewBar;
            _ordertracker = new OrderTracker(10000);

            // in the none zone
            _isOBOSZone = Enumerable.Repeat<OBOSZone>(OBOSZone.None, _symbols.Count).ToArray();
            // neither bull or bear
            _isHigherTimeFrameBullBear = Enumerable.Repeat<BullBearTrend>(BullBearTrend.None, _symbols.Count).ToArray();
            // all are active
            _issymbolactive = Enumerable.Repeat<bool>(true, _symbols.Count).ToArray();
            // none are filled
            _waittobefilled = Enumerable.Repeat<bool>(false, _symbols.Count).ToArray();

            _currentorderids = Enumerable.Repeat<long>(0L, _symbols.Count).ToArray();
            _entrylevel = Enumerable.Repeat<decimal>(0m, _symbols.Count).ToArray();
            _exitlevel = Enumerable.Repeat<decimal>(0m, _symbols.Count).ToArray();

            _transactions = 0;
            _currenttime = 0;
        }

        readonly object _lockobj = new object();
        public void Shutdown(string sym)
        {
            lock(_lockobj)
            {
                // int idx = Array.IndexOf(_symbols, sym);
                int idx = _symbols.IndexOf(sym);

                if (idx > -1)
                {
                    if (!_issymbolactive[idx])
                        return;

                    if (!_positiontracker[sym].isFlat)
                    {
                        SendDebug("Shutting down " + sym);
                        SendOrder(new MarketOrderFlat(_positiontracker[sym]));
                    }

                    _issymbolactive[idx] = false;
                }
            }
        }

        public override void Shutdown()
        {
            lock(_lockobj)
            {
                if (!IsActive)
                    return;

                // flat positions
                SendDebug("Shutting down strategy " + _name);
                foreach (Position p in _positiontracker)
                {
                    if (!p.isFlat)
                        SendOrder(new MarketOrderFlat(p));
                }

                // set inactive
                _isactive = false;
            }
        }

        private enum OBOSZone
        {
            OverboughtZone,
            OverSoldZone,
            None
        };

        private enum BullBearTrend
        {
            Bull,
            Bear,
            None
        };
        #endregion
    }
}