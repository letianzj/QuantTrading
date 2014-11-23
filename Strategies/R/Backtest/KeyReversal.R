#*************** Key reversal is a three bar chart pattern ******************#
# input: ohlcv = xts, n = lookback 
# return = 1: upward reversal; -1: downward reversal; otherwise return 0
#****************************************************************************#
KeyReversal <- function(ohlcv, n)
{
  keyReversal = 0;
  
  end = nrow(ohlcv)
  
  # close[1] > average(close[1],n) and high >= highest(high[1],n)
  #   and low < low[1] and close < close[1]
  if ( (ohlcv[end-1,4] > mean(ohlcv[end-n:end-1,4])) 
      & (ohlcv[end,2] >= max(ohlcv[end-n:end-1,2]))
      & (ohlcv[end,3] < ohlcv[end-n:end-1,3])
      & (ohlcv[end,4] < ohlcv[end-1,4]) )
  {
    keyReversal = -1;
  }
  # pclose[1] < avg and plow < lowest(plow,length)[1] and phigh > phigh[1]
  #   and pclose > close[1]
  else if ( (ohlcv[end-1,4] < mean(ohlcv[end-n:end-1,4])) 
            & (ohlcv[end,3] < min(ohlcv[end-n:end-1,3]))
            & (ohlcv[end,2] > ohlcv[end-n:end-1,2])
            & (ohlcv[end,4] > ohlcv[end-1,4]) )
  {
    keyReversal = 1;
  }
  
  return (keyReversal)
}