function  zma=ZMA(winsize, y)

% y is an array taking value t-winsize, t-winsize+1,....t


x=ones(winsize,1);
x=cumsum(x);

ys=zeros(winsize,1);

ys=cumsum(y);

st= var(x);
temp=cov(x,ys);

zma=temp(1,2)/st;



