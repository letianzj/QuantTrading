#####################################################################################
############################## EOD Daily Pairs Scan #################################
## usage:  res<-DailyPairs("EWA","EWC", 1, 250)
#####################################################################################
DailyPairs <- function(A, B, hprior, lookback = 250)
{
  # initialization
  # setwd("C:\QuantTrading\Strategies\R")
  # rm(list = setdiff(ls(),lsf.str()))
  # library(quantmod)
  library(fUnitRoots)
  library(urca)
  
  today <- Sys.Date()
  startDate <- today - 365*2          # make sure having two year data
  
  #******************** 0. load data **************************#
  aData <- getSymbols(A, from=startDate, auto.assign = F)
  a.p <- aData[,6]
  bData <- getSymbols(B, from=startDate, auto.assign = F)
  b.p <- bData[,6]
  s2 <- merge(a.p, b.p, all=F)
  #interpolated <- na.approx(s2)         # linear interpolation
  #interpolated <- na.locf(s2)           # last observation carry forward
  lookback <- min(lookback, nrow(s2))    # in case history is less than lookback
  a.p <- s2[(nrow(s2)-lookback+1):nrow(s2),1]
  b.p <- s2[(nrow(s2)-lookback+1):nrow(s2),2]
  result <- c(coredata(a.p[length(a.p)]),coredata(b.p[length(b.p)]))        # record today's price
  names(result) <- c('P1','P2')
  
  #******************** 1. find hedge ratio *************************#
  regAB <- lm(a.p ~ b.p)
  regAB.summary <- summary(regAB)
  alpha <- coef(regAB.summary)[1,1]; beta <- coef(regAB.summary)[2,1]
  alpha.p <- coef(regAB.summary)[1,4]; beta.p <- coef(regAB.summary)[2,4]
  residual.adf <- adfTest(coredata(regAB$residual), type='nc')
  result <- c(result, alpha, alpha.p, beta, beta.p, residual.adf@test$statistic, residual.adf@test$p.value)
  names(result)[3:8] <- c('alphaAB','alphaAB.p','betaAB','betaAB.p','residualAB','residualAB.p')
  
  regBA <- lm(b.p ~ a.p)
  regBA.summary <- summary(regBA)
  alpha <- coef(regBA.summary)[1,1]; beta <- coef(regBA.summary)[2,1]
  alpha.p <- coef(regBA.summary)[1,4]; beta.p <- coef(regBA.summary)[2,4]
  residual.adf <- adfTest(coredata(regBA$residual), type='nc')
  result <- c(result, alpha, alpha.p, beta, beta.p, residual.adf@test$statistic, residual.adf@test$p.value)
  names(result)[9:14] <- c('alphaBA','alphaBA.p','betaBA','betaBA.p','residualBA','residualBA.p')
  
  
  johansen <- ca.jo(data.frame(a.p,b.p), type='trace', K=2, ecdet='none', spec='longrun')
  # column 3:5
  hpost <- - johansen@V[2,1]
  if (abs(hprior-hpost) < 0.2)       # take hprior
  {
    h <- hprior 
  }
  else
  {
    h <- hpost
  }
  
  spread <- a.p[length(a.p)]-h*b.p[length(b.p)]
  result <- c(result, hpost, spread)
  names(result)[15:16] <- c('H_Posterior', 'Spread')
  
  
  #******************** 1. find spread series and half-life  *************************#
  spreads <- a.p - h*b.p
  z <- (spread-mean(spreads))/sd(spreads)
  
  slag <- lag(spreads, 1)
  deltas <- spreads - slag
  regression_result <- lm(deltas[-1] ~ slag[-1])
  halflife <- -log(2) / regression_result[["coefficients"]][2]
  
  # ADF test
  spreads.adf <- adfTest(coredata(spreads), type='nc')
  result <- c(result, halflife, spreads.adf@test$statistic, spreads.adf@test$p.value, mean(spreads),sd(spreads),z)
  names(result)[17:22] <- c('Half-Life','ADF', 'ADF(p)', 'Mean', 'Sd', 'Z')
  
  bb20 <- BBands(spreads, sd = 2.0)[length(spreads)]
  # column 11:14
  result <- c(result, bb20[,1],bb20[,2],bb20[,3],bb20[,4])
  names(result)[23:26] <- c('BBdn(20,2)','BBmavg(20,2)','BBup(20,2)','BBpctB(20,2)')
  
  return(result)
}