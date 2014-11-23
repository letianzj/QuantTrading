###-------------------------------- add Library Path -------------------------------###
.libPaths( c("C:/Users/Letian/Documents/R/Library",.libPaths()) )
library()
## change work directory
getwd()
setwd("C:/QuantTrading/Strategies/R")


###-------------------------------- install package form zip -------------------------------###
### install.packages('H:/R/Packages/zoo_1.7-9.zip', repos = NULL, type = 'source', lib='H:/R/Library')
### library(zoo)
### ls("package:zoo", all = T)


###-------------------------------- Data types -------------------------------###
data(trees)
attach(trees)
## + - * / %% (modulus), ^ (exponentiation)
5%%2
### 1. vectors
a <- c(1,2,5.3,6,-2,4)		# numeric vector
class(a)
unclass(a)
str(a)
which (a^2>2)				# find the index
which(a == max(a))
length(a)
a[a > 4] <- 0			# retrieve
a[c(2,4)]				# 2nd and 4th elements of vector
b <- c("one","two","three")	# character vector
c <- c(TRUE, TRUE, FALSE, TRUE)	# logical vector
x <- rep(1,5)
diff(x)
sign(x)
seq(from = 1, to = 10, by = 2)		# 1 3 5 7 9 sequence
for (i in seq(x))			# use seq to avoid NULL case
ls()
rm(list=ls())
  
### 2. Matrices
### matrix multiplication %*%, transpose t(m)
y <- matrix(1:20, nrow = 5, ncol = 4)		# matrix by default is stored column-wise
dim(y)
y[,4]					# 4th column of matrix
y[,-3]
m <- rbind( c(1,4), c(2,2) )
A = matrix(c(3,1,1,2), nrow=2)
B = solve(A); A%*%B		# inverse
t(A)   # transpose
v = eigen(A)			# eigen
A%*%v$vectors[,1];  v$values[1]*v$vectors[,1] 		# first eigen, A*v = lambda*v
sqrtA <- v$vectors %*% diag(sqrt(v$values)) %*% t(v$vectors)	# square root of A
sqrtA %*% sqrtA			# it is A

### 3. Data frames -- tables
## constructed column-wise
d <- c(1,2,3,4)
e <- c("red", "white", "red", NA)
f <- c(TRUE, TRUE, FALSE, TRUE)
mydata <- data.frame(d,e,f)
names(mydata) <- c("ID", "Color", "Passed")		# variable names
mydata[2:3]		# columns 2 and 3
mydata[c("ID","Passed")]
mydata$ID

### 4. Lists -- struct
w <- list(name="Fred", mynumbers=a, mymatrix=y,age=5.3)
w$name				# retrieve

### 5. Factors -- enum
gender <- c(rep("male",20), rep("female",30))
gender <- factor(gender)
summary(gender)
incomes <- c( rep(2.5,20), rep(1.5,30) )
incmean <- tapply(incomes, gender, mean)



###-------------------------------- String and Date -------------------------------###
## concatenate string
u <- paste("abc", "de", "f")
strsplit(u," ")
v <- paste("abc", "de", "f", sep="")
## change mode type
z <- 0.9
digits <- as.character(z)
d <- as.integer(digits)
is.na(z)


###-------------------------------- Flow control -------------------------------###
## if ... else
x = 0.5
if (x > 0)  { y = sqrt(x) } else { y = -sqrt(-x) }
## for loop; with keyword next and break
x = rnorm(10); s = 0
for (i in 1:length(x)) { s = s+x[i] }
## while
x = 0
while (x <= s) { x = x+1 }
## function
square = function(x) x*x
square(1:10)
## sqrt
root = function(x) {
	rold = 0
	rnew = 1
	for (i in 1:10) {
		if (rnew == rold)
			break
		rold = rnew
		rnew = 0.5 * (rnew + x/rnew)
	}
	rnew
}
root(2)


###-------------------------------- Apply Functions -------------------------------###
### 1. apply -- applying a function to margins of an array or matrix
m <- matrix(c(1:10, 11:20), nrow = 10, ncol = 2)
apply(m, 1, mean)			# mean of the rows
apply(m, 2, mean)			# mean of the columns
## divide all values by 2, it equals m[,1:2]/2
apply(m, 1:2, function(x) x/2)

### 2. lapply -- apply to list, return is also a list
# create a list with 2 elements
l <- list(a = 1:10, b = 11:20)
# take mean and sum of the values in each element
lapply(l, mean)
lapply(l, sum)

### 3. sapply -- apply to list, return vector instead of list
# mean of values using sapply
l.mean <- sapply(l, mean) 			# return a numeric vector
l.mean[['a']]

### 4. tapply -- apply to data frame according to factors
attach(iris)
# mean petal length by species
tapply(iris$Petal.Length, Species, mean)

state <-c("NY","NJ","NJ","NY")
statef<-factor(state)
levels(statef)
incomes<-c(100,200,50,70)
incmeans<-tapply(incomes, statef,mean)


### 5. rollapply -- rolling apply to time series
z <- zoo(11:15, as.Date(31:35))
rollapply(z, width = 3, mean, align="left")

### 6. cumsum, cumprod, cummin, cummax
cumsum(1:10)
cumprod(1:10)


###-------------------------------- Filter Functions -------------------------------###
ma5 <- filter(spx.p, sides = 2, rep(1,5)/5)

###-------------------------------- Optimization -------------------------------###
### round, ceiling, floor
round(1.5); round(1.49)			# 2,1
round(-1.5); round(-1.49)		# -2, -1
ceiling(1.5); ceiling(-1.5)		# 2, -1
floor(1.5); floor(-1.5)			# 1, -2

### optimize
f <- function(x) {
	abs(x-3.5)+(x-2)^2
}
op <- optimize(f=f, interval = c(1,5))



###-------------------------------- Simulation -------------------------------###
### d: PDF
### p: CDF
### q: Inverse/quantile
### r: Random
x <- runif(10)
mean(x); median(x); var(x); sd(x)
### Draw sample
sample(1:6, 10, replace = T)		# roll a dice 10 times
sample( c("H","T"), 10, replace = TRUE)	# toss a coin
### 1. Uniform
x <- runif(100,0,2)
hist(x, probability = TRUE, col = gray(.9), main = "uniform on [0,2]")
curve(dunif(x,0,2), add = T)

### 2. Normal
x <- rnorm(100, mean = 0, sd = 1)
hist(x, probability = TRUE, main = "normal mu = , sigma = 1")
curve(dnorm(x), add = T)
### Normal PDF
x <- seq(from = -5, to = 5, by = 0.01)
y <- dnorm(x)
plot(x = x, y=y, type='l', col = "seagreen", lwd=2, 
	xlab="quantile", ylab="density\ny=dnorm(x)")
grid(col="darkgrey",lwd=2)
title(main="Probability Density Function(PDF)")
### central limit theorem
results = c()
mu = 0; sigma=1
for (i in 1:200) {
	X = rnorm(100, mu, sigma)
	results[i] = (mean(X) - mu)/(sigma/sqrt(100))
}
hist(results, prob=T)
## qq plot
x <- rnorm(100,0,1)
qqnorm(x, main = 'normal(0,1)')
qqline(x)
## empirical vs theretical density line
x <- rnorm(1000, 5, 100)
hist(x, prob+T, col = "red")
lines(density(x), lwd=2)
x <- seq(-4,4,length=100)
y <- dnorm(x, mu,sigma)
lines(x,y,lwd=2,col="blue")
## Normality test
x <- rnorm(100, 5,9)
shapiro.test(x)
ks.test(x,"pnorm", mean=mean(x),sd=sd(x))
library("tseries")
jarque.bera.test(x)

### 3. Binomial
x <- rbinom(100, n, p)
hist(x, prob=T)
xvals <- 0:n
points(xvals, dbinom(xvals,n,p),type='h',lwd=3)
y=(x-n*p)/sqrt(n*p*(1-p))
hist(y,prob=T)

### 4. Exponential
x <- rexp(100,1/2500)
hist(x,prob=T,col=gray(0.9), main="exponential mean=2500")
curve(dexp(x,1/2500), add=T)



###-------------------------------- ARIMA Time Series -------------------------------###
### 1. White Noise
w <- rnorm(1000)
plot(w, type = "l")
x <- seq(-3, 3, length=1000)
hist(w,prob=T,breaks=10)
points(x,dnorm(x),type="l")
lines(density(x),col='red')
acf(w)

### 2. Random Walk
x <- w <- rnorm(1000)
for (t in 2:1000) x[t] <- x[t-1]+w[t]
plot(x, type="l")
acf(x)
pacf(x)
acf(diff(x))

### 3. AR(1)
x <- w <- rnorm(1000)
for (t in 2:1000) x[t] <- 0.7*x[t-1]+w[t]
plot(x, type = "l")
acf(x)
pacf(x)
x.ar <- ar(x, method = "mle")

### 4. MA(3)
b <- c(0.8,0.6,0.4)
x <- w <- rnorm(1000)
for (t in 4:1000) {
	for (j in 1:3) x[t] <- x[t] + b[j] * w[t-j]
}
plot(x, type="l")
acf(x)
pacf(x)
x.ma <- arima(x,order=c(0,0,3))

### 5. ARMA(1,1)
x <- arima.sim(n=10000,list(ar=-0.6,ma=0.5))
x.arma <- arima(x,order=c(1,0,1))
coef(x.arma)
acf(resid(x.arma))

### 6. ARIMA(1,1,1)
# x_t = 0.5*x_t-1 +x_t-1 - 0.5*x_t-2 + w_t +0.3w_t-1
# (1-B)(1-0.5B)x_t = (1-0.3B)w_t
x <- w <- rnorm(10000)
for (i in 3:1000) x[i] <- 0.5*x[i-1] + x[i-1] - 0.5*x[i-2] + w[i] + 0.3*w[i-1]
arima(x,order=c(1,1,1))
x <-arima.sim(mode=list(order=c(1,1,1), ar=0.5,ma=0.3), n=1000)
x.arima <- arima(x,order=c(1,1,1))
coef(x.arima)
AIC(x.arima)
predict(x.arima,5)


### 7. GARCH
alpha0 <- 0.1
alpha1 <- 0.4
beta1 <- 0.2
w <- rnorm(10000)
a <- rep(0,10000)
h <- rep(0,10000)
for (i in 2:10000) {
	h[i] <- alpha0 + alpha1*(a[i-1]^2) + beta1*h[i-1]
	a[i] <- w[i]*sqrt(h[i])
}
plot(a, type="l")
acf(a)
acf(a^2)
## fit GARCH
library(tseries)
a.garch <- garch(a, grad = "numerical", trace = FALSE)
confint(a.garch)


### 8. VAR(1)
x <- y <- wx <- wy <- rnorm(1000)
x[1] <- wx[1]
y[1] <- wy[1]
for (i in 2:1000) {
	x[i] <- 0.4*x[i-1] + 0.3*y[i-1] + wx[i]
	y[i] <- 0.2*x[i-1] + 0.1*y[i-1] + wy[i]
}
xy.ar <- ar(cbind(x,y))

### 9. Linear model x = 50 + 3*t + z
z <- w <- rnorm(1000, sd=20)
for (t in 2:1000) z[t] <- 0.8*z[t-1]+w[t]
Time <-1:1000
x <- 50 + 3*Time +z
plot(x, xlab="time", type="l")
x.lm <- lm(x~Time)
coef(x.lm)
sqrt(diag(vcov(x.lm)))
summary(x.lm)
# with autocorrelation in the residuals, OLS is not accurate
acf(resid(x.lm))
pacf(resid(x.lm))
## For a positive serial correlation in the residual series,
## the standard errors of the estimated regression parameters are likely to be underestimated.
### Generalized least square (GLS) can account for autocorrelation in the residual series.
library(nlme)
x.gls <- gls( x~Time, cor = corAR1(0.8 ))
coef(x.gls)
sqrt(diag(vcov(x.gls)))
confint(x.gls)

### 10. GBM
# dlnS = (mu-0.5sigma^2)dt+sigmadW
mu <- 0
sigma <- 0.3
T <- 4
S0 <- 100
w <- rnorm(250*4,sd=1/sqrt(250))
t <- seq(0,4,by=1/250)
dlns <- (mu+0.5*sigma^2)*1/250+sigma*w
lns<-cumsum(dlns)+log(S0)
S <- c(S0,exp(lns))
plot(t,S,type='l')
ES <- S0*exp(mu*t)
lines(t,ES,col='red')

###-------------------------------- State Space Models -------------------------------###


###-------------------------------- I/O -------------------------------###
x <- read.table("c:/x.txt", header=TRUE, sep=",")
x <- read.csv("c:x.csv", header=TRUE)
colnames(x) <- c("date","price","volumn")      # give column names
write(x=m,file="matrix.dat",sep="\t",row.names=F,col.names=F)
