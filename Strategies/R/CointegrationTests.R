#####################################################################################
############################ Cointegration Test Function#############################
## usage:  res <- cointegrationTests('EWA','EWc','2007-01-01',50)
#####################################################################################
cointegrationTests <- function(A, B, startDate = '2007-01-01', lookback = 100)
{
  library(quantmod)
  library(tseries)
  library(fUnitRoots)
  library(urca)
  cat("Processing stocK:", A, " and ", B, " start date: ", startDate, " lookback: ", lookback, "\n")
  
  #********************************** step 0: download data ***********************#
  aData <- getSymbols(A, from=startDate, auto.assign = F)
  a.p <- aData[,6]
  bData <- getSymbols(B, from=startDate, auto.assign = F)
  b.p <- bData[,6]
  s2 <- merge(a.p, b.p, all=F)
  #interpolated <- na.approx(s2)         # linear interpolation
  #interpolated <- na.locf(s2)           # last observation carry forward
  a.p <- s2[,1]; b.p <- s2[,2]
  
  #************************ step 1: check price I(1) or return I(0) ******************#
  # two price series should have same integration order I(1)
  a.acf <- acf(a.p, plot = F); a.pacf <- pacf(a.p, plot = F)
  b.acf <- acf(b.p, plot = F); b.pacf <- pacf(b.p, plot = F)
  adf.a <- adf.test(a.p, k = 1)
  adf.b <- adf.test(b.p, k = 1)
  a.r <- diff(log(a.p))
  b.r <- diff(log(b.p))
  adf.a.r <- adf.test(a.r[-1], k = 1)
  adf.b.r <- adf.test(b.r[-1], k = 1)
  pp.test(a.p);  pp.test(a.r[-1])          # Phillips-Perron unit root
  pp.test(b.p);  pp.test(b.r[-1])          # Phillips-Perron unit root
  po.test(merge(a.r[-1],b.r[-1]))         # Phillips-Ouliaris Cointegration
  cat("stock A price: ADF(p-value)", c(adf.a$statistic, adf.a$p.value), " stock B price: ADF(p-value)", c(adf.b$statistic, adf.b$p.value),'\n')
  cat("stock A return: ADF(p-value)", c(adf.a.r$statistic, adf.a.r$p.value), " stock B return: ADF(p-value)", c(adf.b.r$statistic, adf.b.r$p.value),'\n')
  
  #************************ step 2: Engle-Granger Regression ******************#
  # 2.1 plot rolling hedging ratios to see if it is stationary
  hedgeRatioAB <- rep(as.numeric(NA), length(a.p))
  for (t in lookback:length(hedgeRatioAB))
  {
    regression_result <- lm(a.p[(t-lookback+1):t] ~ b.p[(t-lookback+1):t])
    hedgeRatioAB[t] <- regression_result[["coefficients"]][2]
  }
  hedgeRatioAB <- xts(hedgeRatioAB, index(a.p))          # make it xts again
  plot(hedgeRatioAB)
  
  hedgeRatioBA <- rep(as.numeric(NA), length(a.p))
  for (t in lookback:length(hedgeRatioBA))
  {
    regression_result <- lm(b.p[(t-lookback+1):t] ~ a.p[(t-lookback+1):t])
    hedgeRatioBA[t] <- regression_result[["coefficients"]][2]
  }
  hedgeRatioBA <- xts(hedgeRatioBA, index(a.p))          # make it xts again
  plot(hedgeRatioBA)
  
  # 2.2 find today's cointegration coefficients/long run relationship
  # regA <- lm(a.p ~ b.p + 0)
  regAB <- lm(a.p[(nrow(a.p)-lookback+1):nrow(a.p)] ~ b.p[(nrow(b.p)-lookback+1):nrow(b.p)])
  summary(regAB)
  hrAB <- adfTest(coredata(regAB$residuals), type='nc')
  
  regBA <- lm(b.p[(nrow(b.p)-lookback+1):nrow(b.p)] ~ a.p[(nrow(a.p)-lookback+1):nrow(a.p)])
  summary(regBA)
  hrBA <- adfTest(coredata(regBA$residuals), type='nc')
  cat("lm A ~ B: h, statistic, p-value: ", c(regAB[['coefficients']][2], hrAB@test$statistic, hrAB@test$p.value),'\n')
  cat("lm B ~ A: h, statistic, p-value: ", c(regBA[['coefficients']][2], hrBA@test$statistic, hrBA@test$p.value),'\n')
  
  #************************ step 3: Johansen Test ******************#
  ## nul hypothesis f r=0 < (no cointegration at all), r<1 (till n-1, where n=2 in this case)
  ## eigenvectors are normalised column vectors ordered in decreasing order of their corresponding eigenvalues
  ## so the first cointegrating relation is strongest
  johansen <- ca.jo(data.frame(a.p,b.p), type='trace', K=2, ecdet='none', spec='longrun')
  summary(johansen)
  cat('johansen lambda: ', johansen@lambda, '\n')
  cat('johansen teststat: ', johansen@teststat, '\n')
  cat("johansen c-value 10pct: ", johansen@cval[,1], " 5pct: ", johansen@cval[,2], " 1pct: ", johansen@cval[,3],'\n')
  
  return(johansen@V[,1])  # first eigenvector
}