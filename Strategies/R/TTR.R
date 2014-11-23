library(quantmod)
## lower case y accepts 00-99. Use upper case here.
from.dat <- as.Date("01/01/2006", format="%m/%d/%Y") 
to.dat <- as.Date("12/31/2010", format="%m/%d/%Y") 
getSymbols("^GSPC", src="yahoo", from = from.dat, to = to.dat)
candleChart(GSPC, subset='200902/',theme='black')
addBBands()
addMACD(30,50,12)
# the object time window
x<-GSPC['200902/200903']$GSPC.Close

########### Moving Averge ###########
## SMA
SMA(x,n=10)
## EMA, from the beginning, ratio or smoothing factor = 2/(N+1)
EMA(x,n=1,ratio=2/11)
## DEMA
DEMA(x,n=1,ratio=0.6,v=0.8)