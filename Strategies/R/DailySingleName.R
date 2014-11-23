#####################################################################################
############################ Cointegration Test Function#############################
## usage:  res <- DailySingleName()
#####################################################################################
DailySingleName <- function(ticker='^GSPC', startDate = '2007-01-01')
{
  ###-------------------------------- Load Data -------------------------------###
  library(quantmod)
  SPX <- getSymbols(ticker, from=startDate, auto.assign = F)
  spx.p <- SPX[,6]
  spx.r <- spx.p/lag(spx.p)-1
  # remove the first element which is NA
  spx.r <- spx.r[!is.na(spx.r)]
  names(spx.r) <- 'daily return'

  summary(spx.r)
  # 2008-10-13 it jumped 11.6%
  # spx.r[which(spx.r>0.11)]
  library(moments)
  result <- c(skewness(spx.r), kurtosis(spx.r))
  # skew and kurtosis suggests that return is far from normal
  names(result) <- c('skew','kurtosis')
  print(result)
  
  ###-------------------------------- Normality Test-------------------------------###
  hist(spx.r, freq=F,ylim=c(0,0.5), breaks=20)
  curve(dnorm(x), from=-5,to=5,col='red',add=T)
  qqnorm(spx.r)
  qqline(spx.r,col=2)
  # shapiro test also finds spx.r far from normal
  shapiro.test(coredata(spx.r))
  
  
  ###------------------------------- Stationarity Test ----------------------------###
  library(tseries)
  # price has unit root, not stationary
  adf.test(spx.p)
  # ADF test rejects null hypothesis of unit root, suggesting stationarity
  adf.test(spx.r)    		# Augmented Dickey-Fuller unit root
  # PP test also rejects null hypothesis of unit root and suggests stationarity
  pp.test(spx.p)
  pp.test(spx.r)				# Phillips-Perron unit root
  library(pracma)
  # Hurst test shows trending
  hurstexp(spx.p)
  
  
  ###-------------------------------- ARIMA Test -------------------------------###
  # at 5%, acf has negative lag 1, lag 5, lag 18, and positive lag 16
  acf(spx.r, lag.max = 20, plot=T)
  pacf(spx.r, lag.max=20, plot=T)
  library(forecast)
  # It automatically picks ARIMA(3,0,3)
  auto.arima(coredata(spx.r))		# automatically find the p,d,q
  spx.r.arima <- arima(spx.r, order=c(3,0,3))
  ## five period forecast
  spx.r.arima.forecast <- forecast.Arima(spx.r.arima, h=5)
  plot.forecast(spx.r.arima.forecast)
  # residual is AR(0)
  acf(spx.r.arima.forecast$residuals, lag.max = 20)
  # test autocorrelation of forecast errors
  Box.test(spx.r.arima.forecast$residuals, lag = 20, type = "Ljung-Box") 	
  # test if the forecast errors are normally distributed
  plot.ts(spx.r.arima.forecast$residuals) 			# make a time plot of forecast errors
  
  
  ###-------------------------------- GARCH Test -------------------------------###
  # r^2 is autocorrelated
  acf((spx.r - mean(spx.r))^2)  				# conditional heteroscedasticity
  spx.r.garch <- garch(spx.r,trace = F)
  spx.r.res <- spx.r.garch$res[-1]
  acf(spx.r.res)
  acf(spx.r.res^2)
  
  
  return(result)
}