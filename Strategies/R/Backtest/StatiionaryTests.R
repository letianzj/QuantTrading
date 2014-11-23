setwd("C:/QuantTrading/Strategies/R")
require(quantmod)

rm(list=ls())

# load data
tempsymbol <- getSymbols("USD/CAD",src='oanda')
prices <- get(tempsymbol)
prices <- prices[,1]
rm(list = eval(tempsymbol))
plot(prices)


# 1. regress D_Y against Y_(t-1)
# null hypothesis is beta == 0
prices.lag.1 <- lag(prices)[-1]
prices.diff <- diff(prices)[-1]     
#lm.diff.1 <- lm(prices.diff ~  0 + prices.lag.1)
lm.diff.1 <- lm(prices.diff ~  prices.lag.1)
summary(lm.diff.1)
halflife <- -log(2) / lm.diff.1[["coefficients"]][2]

# 2. Adf test
# null hypothesis is beta == 0 or unit root
library(tseries)
# assume a non-zero  offset/mean but zero drift, with lag = 0 or 1
res<-adf.test(prices, k = 0)
# ADF = -2.4158, p-value = 0.4023. So we can't reject hypothesis of beta = 0
# or we can't reject non-stationarity
# yet beta < 0, showing it's not trending
res<-adf.test(prices, k = 1)

# 3. Hurst Exponent
# H = 0.5 for GBM, H < 0.5 indicating mean-reversion
require(pracma)
hurst(log(prices))
hurstexp(log(prices))

# 4. Variance ratio test
# h = 0 means random walk

# 5. Apply a simple linear mean rerversion strategy
# we invest proportional to its deviation to the mean
lookback <- round(halflife)
# rollapply includes current date
nShares <- -(prices - rollapply(prices, width = lookback, mean, align="right")) / rollapply(prices, width = lookback, sd, align="right")
# hold yesterday's nshare through today
pnl <- lag(nShares,1) * ((prices-lag(prices,1))/lag(prices,1))
pnl[is.na(pnl)] <- 0
plot(cumsum(pnl))
