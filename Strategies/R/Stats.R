###-------------------------------- dlm  -------------------------------###
### Y_t = F_t * theta_t + v_t 			v_t ~ N(0,V_t)
### theta_t = G_t * theta_(t-1) + w_t		w_t ~ N(0,W_t)
### theta_0 ~ N(m0,C0)

###------------------------1. Random Walk plus noise / local level model -------------------------------###
### pp. 42, pp. 95. , it limits to ARIMA(0,1,1)
### staty model has, for large t, essentially the same forecast function as the simple exponential smoothing
### Y_t = mu_t + v_t					v_t ~ N(0,V)
### mu_t = mu_(t-1) + w_t				w_t ~ N(0,W)
### 1.1 Fibonacci in book pairs trading pp 63
### 0,1,1,2,3,5,8,13,21,34,55,89,...
library(dlm)
y <- c(5,3,4,6,9,7,3,1,5,10,12,8,6)
### state variance = observation variance = start variance
rw.dlm <- dlm(m0 = 2, C0 = 0.1, FF = 1, V = 0.1, GG = 1, W = 0.1)
y.filtered <- dlmFilter(y,rw.pairs)
### m0 = E[theta0] = 2 = y0
### m1 = 2/3*y1 + 1/3*y1 = 4
### m2 = 5/8*y2 + 2/8*y1 + 1/8*y0 = 3.375
### m3 = 13/21*y3 + 5/21 * y2 + 2/21 * y1 + 1/21 *y0 = 3.761905
y.filtered$m
y.smooth <- dlmSmooth(y.filtered)
y.forecast <- dlmForecast(y.filtered, nAhead = 12, sampleNew = 10)
### model checking
qqnorm(residuals(y.filtered,sd=FALSE))
qqline(residuals(y.filtered,sd=FALSE))
tsdiag(y.filtered)
### 1.2 EWA
lambda <- 0.8
ewa.dlm <- dlm(m0 = 2, C0 = (1-lambda)*0.1, FF = 1, V = (1/lambda-1)*0.1, GG = 1, W = lambda*0.1)

###------------------------2. linear growth model  -------------------------------###
### pp.42. pp. 96, it limits to ARIMA(0,2,2)
### Y_t = mu_t + v_t 			v_t ~ N(0,V)
### mu_t = mu_(t-1) + beta_(t-1) + w_(t,1) 		w_(t,1) ~ N(0, sigma_mu^2)		local level
### beta_t = beta_(t-1) + w_(t,2)				w_(t,2) ~ N(0, sigma_beta^2)		local growth rate
lg <- dlm(FF = matrix(c(1,0),nr=1),  V = 1.4,
		GG = matrix(c(1,0,1,1),nr=2),
		W = diag(c(0,0.2)),
		m0 = rep(0,2),
		C0 = 10*diag(2) )


###------------------------3. linear regression  -------------------------------###
### pp.43
x <- rnorm(100)
dlr <- dlm(FF = matrix(c(1,0),nr=1),
		V = 1.3,
		GG = diag(2),
		W = diag(c(0.4,0.2)),
		m0 = rep(0,2), C0 = 10*diag(2),
		JFF=matrix(c(0,1),nr=1),
		X=x)

###------------------------4. ARIMA  -------------------------------###