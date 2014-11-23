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
        

        #endregion
    }
}
