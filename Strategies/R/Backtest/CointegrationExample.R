setwd("C:/QuantTrading/Strategies/R")

################# step 0: get data for EWA and EWC #################
rm(list=ls())
library(quantmod)
tckrs <- c("EWA","EWC")
getSymbols(tckrs, src='yahoo', from ='2006-04-04', to = '2012-04-09')
EWA.p <- EWA$EWA.Adjusted
EWC.p <- EWC[,6]
ylim <- c(0, max(EWA.p,EWC.p)+10)
plot(x=EWA.p, xlab="Date", ylab="Prices", ylim = ylim,main="Price Chart", type="l")
lines(x=EWC.p, type='l',col='darkgreen')
legend(x = 'topleft', legend = c("EWA", "EWC"), lty=1, col=c("black","darkgreen"))
plot(coredata(EWA.p),coredata(EWC.p), xlab="EWA",ylab="EWC")

################# step 1: ols #################
library("fUnitRoots") # unit test
## 1.1 EWC ~ EWA
#regression.ca <- lm(EWC.p ~ 0+EWA.p)
regression.ca <- lm(EWC.p ~ EWA.p)
# hedgeRatio = 1.135
hedgeRatio <-regression.ca$coefficients[2]
# adf test on spread, which is actually residuals of regression above
# adf=-3.6692, p-value=0.01 that rejects unit root
# EWC-1.135*EWA = I(0)
# in other words they are cointergrated
plot(regression.ca$residuals)
adfTest(coredata(regression.ca$residuals), type='c',lags=1)
## 1.2 EWA ~ EWC
# adf=-3.6856, p-value=0.01, hedgeRatio=0.811
# Note that 1.135*0.811 != 1
regression.ac <- lm(EWA.p ~ EWC.p)
adfTest(coredata(regression.ac$residuals), type="c",lags=1)

################# step 2: Johansen Test #################
## null hypothesis of r=0 < (no cointegration at all), r<1 (till n-1, where n=2 in this case).
library("urca")
# ca.jo is set to lag=2, ecdet=const->intercept but no trend
# we can't reject r<=1, implying only one cointegrating relationship
# Compared with Epchan's book, r=0 is rejected at 95% level. r=1 cannot be rejected at 90% level
coRes.trace <- ca.jo(data.frame(EWA.p,EWC.p),type="trace",K=2,ecdet="const", spec="longrun")
summary(coRes.trace)
coRes.eigen <- ca.jo(data.frame(EWA.p,EWC.p),type="eigen",K=2,ecdet="const", spec="longrun")
summary(coRes.eigen)

################# step 3: introduce natural resource etf IGE #################
getSymbols('IGE', src='yahoo', from ='2006-04-04', to = '2012-04-09')
IGE.p <- IGE[,6]
# we can only reject r=0 null hypothesis at 95% cl, indicating one pair.
coRes2.trace <- ca.jo(data.frame(EWA.p,EWC.p,IGE.p),type="trace",K=2,ecdet="const", spec="longrun")
summary(coRes2.trace)
# we can't reject r=0
# Instead, Epchan's result shows 3 pairs at 95% CL.
coRes2.eigen <- ca.jo(data.frame(EWA.p,EWC.p,IGE.p),type="eigen",K=2,ecdet="const", spec="longrun")
summary(coRes2.eigen)
## eigenvectors are normalised column vectors ordered in decreasing order of their corresponding eigenvalues
## so the first cointegrating relation is strongest
# Eigenvalues are 0.0118, 0.008315, 0.003156
# Eigenvectors are
# EWA 1 1 1
# EWC -1.193 2.74 6.1
# IGE 0.270 -2.75 -2.234
# Const 1.922 8.959 -93.33
# y <- cbind(EWA.p, EWC.p, IGE.p)
y <- merge(EWA.p, EWC.p, IGE.p)
weight <- coRes2.trace@V
yport <- y %*% weight[-4,1]
yport <- xts(yport, index(y)) # make it xts again
adfTest(yport, lags = 1)
plot(yport)
## Find value of beta and thus the halflife of mean reversion by linear regression fit
ylag <- lag(yport, 1)
deltaY <- yport - ylag
regression_results <- lm(deltaY[-1] ~ ylag[-1]) # Error-correction
summary(regression_results)
# halflife = 21 days, close to 22 days in Epchan's book.
halflife <- -log(2) / regression_results[["coefficients"]][2]

################# step 4: linear mean-reverting strategy #################
# instead of capital, here it usese # of shares
lookback<- round(halflife)
# rollapply includes current date
nShares <- -(yport - rollapply(yport, width = lookback, mean, align="right")) / rollapply(yport, width = lookback, sd, align="right")
# hold yesterday's nshare through today
pnl <- lag(nShares,1) * (yport-lag(yport,1))
pnl[is.na(pnl)] <- 0
plot(cumsum(pnl))
ret <- pnl/abs(yport*lag(nShares,1))
ret[is.na(ret)]<-0
# cumret in percentage
cumret <- cumprod(1+ret)-1
plot(cumret)
