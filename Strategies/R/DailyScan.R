#####################################################################################
######################## EOD Daily Single Name Scan  ################################
## usage:  res<-DailyScan("EWA","^GSPC")
## output: price/return statistics and oscillators
#####################################################################################
DailyScan <- function(ticker, idx='^GSPC')
{
  # initialization
  # setwd("C:\QuantTrading\Strategies\R")
  # rm(list = setdiff(ls(),lsf.str()))
  # library(quantmod)
  library(moments)
  
  today <- Sys.Date()
  startdate <- today - 365*3          # back three years
  lookback <- 250                     # lookback period
  
  #******************** 0. load data **************************#
  syms <- getSymbols(Symbols=c(ticker,idx), from=startdate)
  y <- get(syms[1])        # ticker
  rm(list=eval(syms[1]))
  if (length(syms) < 2)
  {
    x <- y        # in case ticker == index
  }
  else
  {
    x <- get(syms[2])   # index
    rm(list=eval(syms[2]))
  }
  
  # cut short two series in case of new stock
  c2 <- merge(x,y)
  s2 <- rowSums(c2)
  NonNAIndex <- which(!is.na(s2))
  #FirstNonNA <- min(NonNAIndex)
  x <- c2[NonNAIndex,1:6]
  y <- c2[NonNAIndex,7:12]
  
  
  
  #******************** 1. get OHLCVA and chg %chg*************************#
  ytoday <- coredata(y[nrow(y),])     # get last row
  ypreday <- coredata(y[nrow(y)-1,])   # get second last row
  
  result <- ytoday
  # column 1:8
  result <- c(result, ytoday[,4]-ypreday[,4], ytoday[,4]/ypreday[,4]-1)
  names(result) <- c('Open','High','Low','Close','Volume','AdjClose','Change','Change(%)')
  
  #*************** 2. get mean, std, correlation, alpha, beta *********************#
  yr <- y[,6]/lag(y)[,6] - 1
  xr <- x[,6]/lag(x)[,6] - 1
  if (length(yr) > lookback)
  {
    yr <- yr[(length(yr)-lookback+1):length(yr)]
    xr <- xr[(length(xr)-lookback+1):length(xr)]
    
    avg <- mean(yr); std <- sd(yr)
    sk <- skewness(yr); kur <- kurtosis(yr)
    c <- cor(yr,xr)
    regression <- lm(yr ~ xr)
    alpha <- regression$coefficients[[1]]
    beta <- regression$coefficients[[2]]
  }
  else
  {
    avg <- NA; std <- NA; sk <- NA; kur <- NA; c <- NA; alpha <- NA; beta <- NA
  }
  
  # column 9:15
  result <- c(result, avg, std, sk, kur, c, alpha, beta)
  names(result)[9:15] <- c('RAvg','RStd','RSkew','RKurtosis','RCorr','Alpha','Beta')
  
  #*************** 3. get technical indicators *********************#
  # http://www.tradinggeeks.net/2014/07/technical-analysis-with-r/
  # 3.1 sma/ema 5 10 20 50 100 200
  # todo: and Double Exponential Smoothing or Holt-Winters
  # in EMA, alpha = 2/(N+1)  EMAt = EMA_t-1 + alpha*(pt - EMA_t-1)
  p <- y[,6]       # price
  if (length(p) > 200)
  {
    SMA5 <- SMA(p,5)[length(p)]
    SMA10 <- SMA(p,10)[length(p)]
    SMA20 <- SMA(p,20)[length(p)]
    SMA50 <- SMA(p,50)[length(p)]
    SMA100 <- SMA(p,100)[length(p)]
    SMA200 <- SMA(p,200)[length(p)]
    EMA14 <- EMA(p,14)[length(p)]
  }
  else
  {
    SMA5 <- NA; SMA10 <- NA; SMA50 <- NA; SMA100 <- NA; SMA200 <- NA; EMA14 <- NA
  }
  
  # column 16:21
  result <- c(result, SMA5, SMA10, SMA50, SMA100, SMA200, EMA14)
  names(result)[16:21] <- c('SMA5','SMA10','SMA50','SMA100','SMA200','EMA14')
  
  # 3.2 MACD(12,26,9)
  if (length(p) > 35)
  {
    macd <- MACD(p, nFast=12, nSlow=26, nSig=9, motype=SMA)[length(p)]
  }
  else
  {
    macd <- matrix(c(NA,NA),nrow=1)
  }
  
  # column 22:23
  result <- c(result, macd[,1],macd[,2])
  names(result)[22:23] <- c('MACD(12,26)','Signal(9)')
  
  # 3.3 RSI (7, 14, 21)
  if (length(p) > 30)
  {
    rsi7 <- RSI(p,n=7)[length(p)]
    rsi14 <- RSI(p,n=14)[length(p)]
    rsi21 <- RSI(p,n=21)[length(p)]
  }
  else
  {
    rsi7 <- NA; rsi14 <- NA; rsi21 <- NA
  }
  
  # column 24:26
  result <- c(result, rsi7, rsi14, rsi21)
  names(result)[24:26] <- c('RSI7','RSI14','RSI21')
  
  # 3.4 z score and Bollinger Bands
  # BBands(20,2)
  # %B = 1 upper band, %B = 0 lower band, %B = 0.5 center 20D SMA
  if (length(p) > 30)
  {
    bb20 <- BBands(p, sd = 2.0)[length(p)]
  }
  else
  {
    bb20 <- matrix(c(NA,NA,NA,NA),nrow=1)
  }
  
  # column 27:30
  result <- c(result, bb20[,1],bb20[,2],bb20[,3],bb20[,4])
  names(result)[27:30] <- c('BBdn(20,2)','BBmavg(20,2)','BBup(20,2)','BBpctB(20,2)')
  
  
  # 3.5 Stoch and WilliamsR
  
  return(result)
}