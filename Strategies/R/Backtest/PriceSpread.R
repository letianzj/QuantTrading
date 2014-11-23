setwd("C:/QuantTrading/Strategies/R")
rm(list=ls())
library(quantmod)

## step 0: get data for EWA and EWC
tckrs <- c('GLD','USO')
getSymbols(tckrs, src='yahoo',from='2006-05-24',to='2012-04-09')
GLD.p <- GLD$GLD.Adjusted
USO.p <- USO[,6]
ylim <- c(0, max(GLD.p,USO.p)+10)
plot(x=GLD.p, xlab="Date", ylab="Prices", ylim=ylim, main="price Chart", type="l")
lines(x=USO.p, type='l', col='darkgreen')
legend(x = 'topleft', legend=c("GLD","USO"), lty=1, col=c('black','darkgreen'))

## rolling heding ratios
## step 1: price spread
## lookback period for calculating the dynamically changing hedge ratio
lookback <- 20
hedgeRatio <- rep(as.numeric(NA), length(GLD.p))

for (t in lookback:length(hedgeRatio))
{
  regression_result <- lm(USO.p[(t-lookback+1):t] ~ GLD.p[(t-lookback+1):t])
  hedgeRatio[t] <- regression_result[["coefficients"]][2]
}

## USO = h * GLD
hedgeRatio <- xts(hedgeRatio,index(GLD.p))   # make it xts again
plot(hedgeRatio)
yport <- USO.p - hedgeRatio*GLD.p
plot(yport)
nShares <- -(yport - rollapply(yport, width=lookback,mean, align="right")) / rollapply(yport, width=lookback, sd, align="right")
pnl <- lag(nShares,1) * ((yport-lag(yport,1))/lag(yport,1))
pnl[is.na(pnl)] <- 0
plot(cumsum(pnl))
# return is P&L divided by gross market value of portfolio
ret <- pnl/lag(yport)
ret[is.na(ret)] <- 0
plot(cumprod(1+ret)-1)

## step 2: log price spread