#*************** Find the Swing ******************#
# input: ohlcv = xts, swingfilter = 4% by default 
# return = 1: upward reversal; -1: downward reversal; otherwise return 0
#****************************************************************************#
Swing <- function(ohlcv, swingfilter=0.04)
{
  n = nrow(ohlcv);
  
  # step 1: Swing filter is fixed or relative %, default 4%
  
  # step 2: plot first bar, its uptrend if close < (high+Low)/2; downtrend otherwise.
  #   If the assumption is wrong, the chart will correct itself soon.
  
  # step 3: Move to the next day or, if intraday, move to the next bar.
  
  # step 4: If price are in an upswing
  
  return (keyReversal)
}