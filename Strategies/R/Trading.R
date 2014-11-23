###-------------------------------- Setup Environment -------------------------------###
currency("USD")
get("USD", envir=.instrument)
stock("SPY", currrency = "USD", mutiplier = 1)
get("SPY", envir=FinancialInstrument::.instrument)
ls(FinancialInstrument::.instrument)



###-------------------------------- Date and Time -------------------------------###
d <- Sys.time()
class(d)  			# POSIXct POSIXt
sapply( unclass(as.POSIXlt(d)), function(x) x )
## CCYY-MM-DD HH:MM:SS[.s]
dts <- c("2005-10-21 18:47:22", "2005-12-24 16:39:58", "2005-10-28 07:30:05 PDT")
as.POSIXlt(dts)
mydate <- strptime('16/Oct/2005:07:51:00', format='%d/%b/%Y:%H:%M:%S')
options(digits.secs = 3)			# millisecond
mytime <- strptime('20110417 21:37:53.437000', format = "%Y%m%d %H:%M:%OS")


###-------------------------------- read csv file -------------------------------###
### as.is prevents from transforming character columns into factors
spx.p <- read.table("H:/R/Data/SPX.csv", sep=',', col.names = c('Date','price'),
                    head = T, as.is=c(1:2))
write(x=spx.p, file = "spx.dat", sep="\t", row.names = F, col.names=F)
### %y (2 digits), %Y (4 digits)
spx.p <- zoo(spx.p[,2], order.by = as.Date(spx.p[,1], format="%m/%d/%Y"))
start(spx.p); end(spx.p); frequency(spx.p); class(coredata(spx.p));
class(time(spx.p))
spx.p <- xts(spx.p)				# extensible time series for tick data, it uses POSIXct/POSIXlt
plot(spx.p, xlab="Date", ylab="$", main="SPX Daily Price")
appl.p <- read.table("H:/R/Data/AAPl.csv", sep=',', col.names = c('Date','price'),head=T, as.is=c(1:2))
aapl.p <- zoo(aapl.p[,2], as.Date(aapl.p[,1],format="%m/%d/%Y"))
aapl.p <- xts (aapl.p)
aapl.p[asDate("2009-08-20")]			# retrieve
spx.p[index(spx.p) >= as.Date("2009-03-28") & index(spx.p) <= as.Date("2009-04-01")] 	#retrieve
spx.p["200904"]					# retrieve
spx.p["20090301/200903"]				# retrieve


###-------------------------------- Load Data -------------------------------###
library(quantmod)
from.dat <- as.Date("01/01/1940",format="%m%d%Y")
getSymbols("^GSPC", src='yahoo', from ='1950-01-01')			#SPX
class(GSPC)							# xts zoo
## common functions for time series ##
## time(), start(), end(), coredata() ##
head( coredata(GSPC) )


###-------------------------------- Data Processing -------------------------------###
### merge: merges time series and automatically handels of time alignments
### aggregate: create coarser resolution time series with summary statistics
### rollapply: calculating rolling window statistics
### align.time: align time series to a coarser resolution
### to.period: convert time series 
setwd("./R")
esa <- read.table("ESH3 FUT GLOBEX20130208.csv", sep=',', head = T, as.is=c(1:14))
esa <- esa[,1:5]  		# delete bid/ask
esa$Time <- apply(esa, 1, function(row) paste(toString(row[1]),row[2]))		## combine Date and Time
esa<-esa[,2:5]
esa$Time <- strptime(esa$Time,"%Y%m%d %H:%M:%OS")
library(xts)
esa <- xts(esa[,-1],order.by=esa[,1])
esa <- na.locf(esa)
plot.xts(esa['2013-02-08 09:30:00::2013-02-08 10:00:00',"Trade"],col='blue',minor.ticks=F,main='E-mini')

esa.5 <- align.time(esa,60*5)
names(esa.5)[3] <- "Volume"			# must specificaly use name "Volume"
esa.5min <- to.minutes5(esa.5[,c("Trade","Volume")])
colnames(esa.5min) <- c("Open","High","Low","Close","Volume")
chartSeries(esa.5min)



### merge two series, union or intersect ###
paired.p <- merge("spx.p[200904::200908"], aapl.p["200905::200910"], all = F)
dim(paired.p)
### handle missing data
interpolated <- na.approx(spx.p)		# linear interpolation
interploated <- na.locf(spx.p)		# last observation carry forward
### rolling window sma
rollapply(spx.p["201003"], 10, mean)

### Decompose trend and seasonality ###
spxMonthly <- aggregate(spx.p, by = as.yearmon, FUN=tail,1) 	# 1 is the further arg
spxDecomp <- decompose(spxMonthly)
plot(spxDecomp)
spxSeasonallyAdjusted <- spxMonthly - spxDecomp$seasonal
plot(spxSeasonallyAdjusted)



###-------------------------------- from price to return-------------------------------###
# spx.r = periodReturn(spx.p, period = 'daily', subset = '2007::', type = 'log')
# diff(x) = x - lag(x, -1), where -1 lag equals the back operator
spx.r <- diff(log(spx.p))			# log return
spx.r <- spx.r[2:length(spx.r)]		# first row is NA
aapl.r <- diff(log(aapl.p))
aapl.r <- aapl.r[2:length(aapl.r)]
cor(spx.r,aapl.r)
names(spx.r) <- 'spx daily return'
names(aapl.r) <- 'aapl daily return'
library(moments)
skewness(spx.r); kurtosis(spx.r)
## cross-correlation function ccf correlates x[t+h] and y[t]
## if h positive x lags y
acf(merge(spx.r,aapl.r))			# auto-covariance
pacf(merge(spx.r,aapl.r), lag.max= 20, plot=TRUE)

ret <- log(x) - log(lag(x))
row.has.na <- apply(ret,1,function(x) {any(is.na(x))})
print(sum(row.has.na))
ret <- ret[!row.has.na]

rollsigma <- rollapply(ret, 10, sd, align="right")


###-------------------------------- Normality Test-------------------------------###
summary(spx.r)
library(moments)
skewness(spx.r)
kurtosis(spx.r)
hist(spx.r, freq=F,ylim=c(0,0.5), breaks=20)
curve(dnorm(x), form=-5,to=5,col='red',add=T)
qqnorm(spx.r)
qqline(spx.r,col=2)
shapiro.test(coredata(spx.r))


###-------------------------------- Linear Regression -------------------------------###
plot(coredata(spx.r["::2012"]), coredata(aapl.r["::2012"]))		# plot
capm.lm <- lm(aapl.r~spx.r)
# from s, one gets MSE(SSE) as s^2. Then whith R^2, one gets SST
summary(capm.lm)
coef(capm.lm)
anova(capm.lm)
confint(capm.lm)
abline(capm.lm)
acf(resid(capm.lm))
pacf(resid(capm.lm))
### plot with prediction
x <- coredata(spx.r["::2012"])[,1]
y <- coredata(aapl.r["::2012"][,1])
lm2<-lm(y~x)
## 95% confidence interval based on s^, 2.5% each side.
predict(lm2, interval="predict",newdata=data.frame(x=0.01))
plot(coredata(spx.r["2013"]), coredata(aapl.r["2013"]))
#Rolling regression (unweighted), with prediction intervals
x <- rollapply(
  as.zoo(Ad(GSPC)),
  width=300, by.column=FALSE,
  FUN=function(x) {
    r <- lm(x ~ index(x))
    tail(predict(r,interval="prediction"))
  })
plot(index(GSPC),Ad(GSPC),type="l",lwd=3,las=1)
lines(index(x),x$fit,col="purple",lwd=3)
lines(index(x),x$lwr,col="purple",lwd=3,lty=3)
lines(index(x),x$upr,col="purple",lwd=3,lty=3)
abline(lm(Ad(GSPC)~index(GSPC)),col='light blue',lwd=3)

###------------------- plotForecastErrors function  -------------------------------###
plotForecastErrors <- function(forecasterrors)
{
  # make a red histogram of the forecast errors:
  mybinsize <- IQR(forecasterrors)/4
  mymin <- min(forecasterrors)*3
  mymax <- max(forecasterrors)*3
  mybins <- seq(mymin, mymax, mybinsize)
  hist(forecasterrors, col="red", freq=FALSE, breaks=mybins)
  # freq=FALSE ensures the area under the histogram = 1
  mysd <- sd(forecasterrors)
  # generate normally distributed data with mean 0 and standard deviation mysd
  mynorm <- rnorm(10000, mean=0, sd=mysd)
  myhist <- hist(mynorm, plot=FALSE, breaks=mybins)
  # plot the normal curve as a blue line on top of the histogram of forecast errors:
  points(myhist$mids, myhist$density, type="l", col="blue", lwd=2)
}



###-------------------------------- ARIMA Test -------------------------------###
acf(spx.r, lag.max = 20, plot=T)
pacf(spx.r, lag.max=20, plot=T)
library(tseries)
adf.test(spx.r)				# Augmented Dickey-Fuller unit root
pp.test(spx.r)				# Phillips-Perron unit root
pp.test(spx.p)
po.test(merge(spx.r,aapl.r))		# Phillips-Ouliaris Cointegration
library(forecast)
auto.arima(coredata(spx.r))		# automatically find the p,d,q
spx.arima <- arima(spx.r, order=c(2,0,3))

## forecast
library("forecast")
spx.arima.forecast <- forecast.Arima(spx.arima, h=5)
plot.forecast(spx.arima.forecast)
acf(spx.arima, forecast$residuals, lag.maax = 20)
Box.test(spx.arima.forecast$residuals, lag = 20, type = "Ljung-Box") 	# test autocorrelation of forecast errors
# test if the forecast errors are normally distributed
plot.ts(spx.arima.forecast$residuals) 			# make a time plot of forecast errors
plotForecasErrors(spx.arima.forecast$residuals)		# make a histgram



###-------------------------------- GARCH Test -------------------------------###
acf((spx.r - mean(spx.r))^2)					# conditional heteroscedasticity
spx.garch <- garch(spx.r,trace = F)
spx.res <- spx.garch$res[-1]
acf(spx.res)
acf(spx.res^2)



###-------------------------------- Chart -------------------------------###
### chart, ta is a vector of technical indicators, e.g. TA = "adVol()"
chartSeries(GSPC, type ='candlestick', subset='200902/',theme = 'black',TA = NULL) 		# candleChart
addVol()							# volumn
addMACD(32,50,12)
addBBands()
reChart(subset="2009",theme="white",type="candles")
last(GSPC['2009'],'3 weeks')
periodicity[GSPC)
to.monthly(GSPC)
periodReturn(GSPC, period='yearly', subset='2009::', type='log')
## Bollinger bands
b <- BBands(HLC = HLC[GSPC["2011"], n =20,sd=2)
chartSeries(GSPC, TA = 'addBBands();addBBands(draw="p");addVol()', subset='2011', theme = "white")
### Add SMA
fastMA <- SMA(Cl(spx.p), n=6)
slow <- SMA(Cl(spx.p),n=45)
co <- fastMA > slowMA
x <- which(co[2006-03-10::2007-03-10'])[1]
chartSeries(spx.p, name="SPX', subset='2006-03-10::2007-03-10', TA = 'addSMA(n=6,col="red"); addSMA(n=24,col='blue')')
text(x=x,y=spx.p[x,"Low"], "Crossover\nbar",col="white",pos=2)
spx.sma14 <- SMA(spx.r, n = 14)




###-------------------------------- TTR -------------------------------###
### For n=5, EMA[1:4] = NA, EMA[5] = mean(y[1:5])
### Then EMA[6] = a*y[6]+(1-a)*EMA[5], and so on
ema.5 <- EMA(esa[,"Trade"],5)
### Therefore, MACD starts at period 26+9-1 = 34
macd <- MACD(esa[,"Trade"],12,26,9,maType="EMA")












































