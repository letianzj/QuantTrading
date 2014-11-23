VanillaCall <- function(S0, sigma, T)
{
  ### parameters
  K <- 100
  r <- 0.01

  steps <- 1000
  nSim <- 10000
  V <- rep(0, nSim)
  qV <- V

  for (i in 1:nSim)
  {
    rn <- rnorm(steps, 0, sqrt(T/steps))     # dw
    rn <- rn * sigma               # sigma * dw
    dlnS <- (r-0.5*sigma^2)*(T/steps) + rn        # dln(S) = (r-0.5*sigma^2)*dt + sigma*dw
    dlnS[1] <- log(S0)
    csumlnS <- cumsum(dlnS)
    S <- exp(csumlnS)
    quadraticV <- diff(S)
    quadraticV <- quadraticV^2
    V[i] <- exp(-r)*max(S[steps]-K,0)
    qV[i] <- sum(quadraticV)
  }

  d1 <- (log(S0/K) + (r+sigma^2/2)*T)/(sigma*sqrt(T))
  d2 <- (log(S0/K) + (r-sigma^2/2)*T)/(sigma*sqrt(T))
  p <- S0*pnorm(d1) - K*exp(-r*T)*pnorm(d2)
  delta <- pnorm(d1)
  gamma <- dnorm(d1)/S0/sigma/sqrt(T)
  vega <- S0*dnorm(d1)*sqrt(T)
  theta <- -S0*dnorm(d1)*sigma/2/sqrt(T)-r*K*exp(-r*T)*pnorm(d2)
  rho <- K*T*exp(-r*T)*pnorm(d2)
  ret <- c(mean(V),p,delta,gamma,vega,theta,rho,mean(qV))
  names(ret) <- c("simprice","price","delta","gamma","vega","theta","rho","qudraticvariation")
  return (ret)
}