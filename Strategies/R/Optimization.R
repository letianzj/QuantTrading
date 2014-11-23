## 1. optimize					Single variable optimization over an interval
## 2. optim						General optimization based on Nelder-Mead, quasi-Newton, and conjugate gradient
## 3. linear programming
## 4. quadratic programming			libary(quadprog); args(solve.QP)
## 5. Differential Evolution Algorithm	args(DEoptim)
## 6. Portfolio Optimization


## 1. optimize		interval = c(1,5)
f <- function(x) {
	abs(x-3.5) + (x-2)^2
}
op <- optimize(f = f, interval = c(1,5))

## 2. optim			method = c("Nelder-Mead", "BFGS", "CG", "L-BFGS-B", "SANN"), lower = -Inf, upper = Inf
##				gr = NULL, hessian = FALSE
## Given samples of x ~ N(mu, sigma^2), use MLE to estimate the parameters
xvec <- c(2, 5, 3, 7, -3, -2, 0)		# sample
fn <- function(theta) {
	sum( 0.5*(xvec-theta[1])^2/theta[2] + 0.5* log(theta[2]) )
}
optim(theta <- c(0,1), fn, hessian=TRUE)
nlm(fn, theta <- c(0,2), hessian=TRUE)
mean(xvec)			# this checks out with estimate[1]
sum( (xvec -mean(xvec))^2 )/7		# this also checks out with estimate[2]


## 5. Differential Evolution Algorithm
G2 = function( x ) {
	if( x[1] >=0 & x[1] <=10 & x[2] >=0 & x[2] <= 10 &
		x[1] * x[2] >= 0.75 & x[1] + x[2] <= 15 ) {
			s <- cos(x[1])^4+cos(x[2])^4
			p <- 2*cos(x[1])^2*cos(x[2])^2
			r <- sqrt(x[1]^2+2*x[2]^2)
			f <- -abs((s-p)/r)	
	} else {
			f <- 0
	}
	return(f)
}

lower = c(0,0)
upper = c(10,10)
res = DEoptim(G2, lower, upper,control = list(NP = 20, itermax = 100, strategy=1, trace=F))


## 6. Portfolio Optimization
