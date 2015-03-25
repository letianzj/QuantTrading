


cd C:\Users\Zimin\Documents\Matlab;

clear all;
close all;
clc;
format short g;
addpath(strcat(pwd,'\functions'));
addpath(strcat(pwd,'\data'));
addpath(strcat(pwd,'\Mbook_Paolo'));


tday=datestr(datenum(date),'mm/dd/yyyy');
tday1={tday};

%symbol={'azo'};
symbol={'xiv','spy','fb','gld','tlt','lvs','googl','txn',...
    'pcln','qqq','cvs','lmt','wlk','PFE','dish','ddd','iwm',...
    'dia','eem','BP','CVX','yhoo','kmb'...
    'amzn','lnkd','pm','sbux','AZO',...
    'wfc','axp','AAPL','kors','fslr','tsla'...
    'jnj','low','hd','rost','fex','cop','ko','qihu','exc'};

%yearstart=2007;
yearstart=2010;
start_date=strcat('11/30/',num2str(yearstart));
start_date={start_date};

weekday_today=weekday(date);
c=clock;
if or(or(c(1,4)>19,c(1,4)<9),or(weekday_today==7,weekday_today==1)) %sat or sunday
  add=0;
else
  add=1;
end

add=0;

%filename='TestEvents.xlsx';
filename='TestEvents_50_xlsx.xlsx';
%filename2='\Etrade_input.xlsm';
ifplot=0;
ifwritefile=1;
%viewsize=919; % fit with the existing graphics
viewsize=50;   %% aslo used to calculate the recent performance


[i,nsymb]=size(symbol);

if add==1
   addTodayPrice=zeros(nsymb,1);
   nn=5+nsymb-1;
   str1=strcat('c5:c',num2str(nn));
   addTodayPrice=xlsread(strcat(pwd,'\',filename),'RT_Quotes',str1);

end 


winsize=8*ones(nsymb,2);   %defaut [8,8]

%from TestEvents_calibrate.xlsx
%update 9/16/14
%update 10/9/14
%update 11/3/14
%update 11/18/14
%update 11/25/14
%update dec/7/14
%update feb/24/15


winsize=[  %%%% 50 days holdout
10	3
10	5
10	6
9	4
9	5
6	6
3	3
9	4
3	3
10	4
10	11
5	11
5	5
4	8
8	11
4	3
7	5
7	7
10	4
8	6
8	10
7	4
8	11
6	10
3	5
7	5
4	6
11	3
7	6
10	5
4	11
10	4
8	7
10	7
5	11
9	11
4	8
5	4
10	4
7	6
11	4
9	6
7	11
];

%winsize=[  80-20 split 
%10	3
%10	5
% 3	8
% 8	8
% 4	8
% 6	6
% 7	5
% 7	11
% 3	3
% 10	4
% 6	8
% 5	11
% 5	5
% 10	5
% 6	11
% 8	5
% 7	10
% 7	7
% 6	3
% 7	6
% 8	10
% 6	8
% 6	10
% 6	10
% 4	7
% 7	5
% 4	7
% 11	3
% 7	6
% 7	11
% 4	11
% 10	4
% 8	7
% 8	7
% 6	8
% 9	11
% 4	8
% 7   9
% 10  11
% 7  6
% 7  8
% 9  6
% ];


%winsize=[10,6; %xiv
%         10,5; %spy
%         5,7;  %fb
%         9,4;  %gld
%         9,6; %tlt
%         6,6; %lvs
%         9,5; %googl
%         9,4;  %txn
%         9,4; %pcln
%         10,4; %qqq
%         8,6; %cvs
%         8,8; %lmt
%         5,5; %wlk
%         10,5; %pfe
%         8,6; %dish
%         4,5; %ddd
%         7,5; %iwm
%         7,7; %dia
%         7,7; %eem
%         7,6; %bp
%         8,10; %CVX
%         8,8; %YHOO
%         8,6; %kmb
%         6,6; %amzn
%         4,7; %lnkd
%         5,5; %pm
%         4,7; %sbux
%         11,3; %AZO
%         7,6; %wfc
%         7,5; %AXP
%         4,7; %aapl
%         10,4]; %kors
%         %7,7; %eem
      
     

data = getYahooDailyData(symbol,start_date, tday1, 'mm/dd/yyyy');

for asset=1:nsymb
    
XX=getfield(data, symbol{asset});
[datasize,colnum]=size(XX);

outsample=viewsize;  %%%round(datasize*0.2);   %%%%  80-20 split used in para selection %%%%%%%%%


hist_date=datestr(XX.Date,'mm/dd/yyyy');
hist_close=XX.AdjClose;
hist_volume=XX.Volume;
lastvol=hist_volume(end);

 if add==1 
      datasize=datasize+1;
      hist_date=[hist_date;tday]; %add today's date
      hist_close=[hist_close;addTodayPrice(asset)];
      hist_volume=[hist_volume;lastvol]; %use yesterday's
  end; % add today's approximate close
  
  
   for j=1:datasize
       hist_date1{j,1}=hist_date(j,:);
   end

  
  
  
x=[hist_close,hist_volume];
[x,xmin,xmax]=rangeNorm1(x);   %normalized to [0,1]


zma=zeros(datasize,2);

for j=1:2
    
for i=winsize(asset,j):datasize
    start=i-winsize(asset,j)+1;
    y=x(start:i,j);
    zma(i,j)=ZMA(winsize(asset,j), y);
end 
end



% -------------------------
n= datasize;
m=4;

x1=hist_close;
x1p=[x1(1);x1(1:n-1)];
r1=(x1-x1p)./x1p;
r2=[r1(2:n);0];

p2=prctile(r1,2);
p5=prctile(r1,5);
p92=prctile(r1,92);
p97=prctile(r1,97);

zma_diff=zeros(datasize,2);
zma_diff=[zeros(1,2);diff(zma)];

zma_diff(:,2)=[zma_diff(1,2);zma_diff(1:n-1,2)]; 
%using yesterday's volume state since volume is settled at close


%zma_diff=[zeros(1,2);diff(zma)];

s1=and(zma_diff(:,1)>0,zma_diff(:,2)>0); %rr
s2=and(zma_diff(:,1)>0,zma_diff(:,2)<=0); %rf
s3=and(zma_diff(:,1)<=0,zma_diff(:,2)>0); %fr
s4=and(zma_diff(:,1)<=0,zma_diff(:,2)<=0); %ff


% reconstructe the path
state=zeros(n,1);
peventdays=zeros(n,1);
veventdays=zeros(n,1);

for i=1:n
    if s1(i)==1
        state(i)=1;
        peventdays(i)=1;
        veventdays(i)=1;
        
    elseif s2(i)==1
        state(i)=2;
        peventdays(i)=1;
        veventdays(i)=-1;
    elseif s3(i)==1
        state(i)=3;
        peventdays(i)=-1;
        veventdays(i)=1;
    else
        state(i)=4;
        peventdays(i)=-1;
        veventdays(i)=-1;
    end
end

avg=zeros(m,1);
count=zeros(m,1);
stddev=zeros(m,1);
sharpe=zeros(m,1);



insample=1:(n-outsample);

avg(1)=mean(r2(s1(insample)));
avg(2)=mean(r2(s2(insample)));
avg(3)=mean(r2(s3(insample)));
avg(4)=mean(r2(s4(insample)));

count(1)=sum(s1(insample));
count(2)=sum(s2(insample));
count(3)=sum(s3(insample));
count(4)=sum(s4(insample));

sumcount=sum(count);

stddev(1)=std(r2(s1(insample)));
stddev(2)=std(r2(s2(insample)));
stddev(3)=std(r2(s3(insample)));
stddev(4)=std(r2(s4(insample)));


for i=1:m
    breath=sqrt(250*count(i)/sumcount);
    sharpe(i)=breath*abs(avg(i))/stddev(i);
end

%avg=[1;-1;1;-1];  investment belief

strength=sum(sharpe .* (sharpe>0.4));

buyorder=zeros(n,1);

buyorder=sign(avg(state)).* (sharpe(state)>0.4);
%buyorder=sign(avg(state));

x2p=[0;buyorder(1:n-1)];

pnl=(x1-x1p).*x2p;
cpnl=cumsum(pnl);
%cpnlp=cumsum(pnl./x1p);
peventdays=cumsum(peventdays);
veventdays=cumsum(veventdays);

z=[peventdays,veventdays];
z_relative=rangeNorm(z);

winingSize=sum(pnl.*(pnl>0))/datasize
losingSize=sum(pnl.*(pnl<=0))/datasize

wl_ratio=-winingSize/losingSize
winingCount=sum(pnl>0)
winingProb=winingCount/datasize

if ifplot==1
figure;
%plot(rangeNorm(x),'--r');
hold on

plot(zma);
plot(x(:,1),'r');

title(symbol{asset});

figure;
plot(x1,'r');
hold on;
plot(cpnl);

plot(buyorder*5,'g');

title(symbol{asset});

figure;
plot(z_relative(:,1),'r');  %peventdays
hold on;
plot(z_relative(:,2));  %veventdays
title(symbol{asset});

figure;
plot(veventdays,peventdays,'r');
hold on
plot(veventdays(end),peventdays(end),'o');


title(symbol{asset});

end %ifplot


disp('---------------------------------------------------')
%l=n-100; % 4 weeks
%see=[x1(l:n),rangeNorm(zma(l:n,:)),buyorder(l:n), state(l:n), pnl(l:n),cpnl(l:n)];

%see

disp('---------------------------------------------------')
drawdown=zeros(n,1);
ddduration=zeros(n,1);

[maxDD,maxDDD,maxDD_pct,drawdown,ddduration]=Calc_drawdown(cpnl);

ret_strat=pnl./x1p;

sharpe_strat_in=mean(ret_strat(insample))/std(ret_strat(insample))*15.81
sharpe_long_in=mean(r1(insample))/std(r1(insample))*15.81

sharpe_strat=mean(ret_strat)/std(ret_strat)*15.81
sharpe_long=mean(r1)/std(r1)*15.81

out=(n-viewsize+1):n;   %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

sharpe_strat_out=mean(ret_strat(out))/std(ret_strat(out))*15.81
sharpe_long_out=mean(r1(out))/std(r1(out))*15.81

volatility=std(ret_strat(out))*15.81; % strat volatility


out=(n-20+1):n;   %%%%%%%%%%%%%%%%%%% last four weeks performance
forecast_ret=abs(avg(state(out-1)));
forecast_cum=cumsum(forecast_ret);
realized_ret=ret_strat(out);
realized_cum=cumsum(realized_ret);

IC=corr2(forecast_cum,realized_cum);   %Information Coefficient for alpha selection
IC1=corr2(forecast_ret,realized_ret);

ICs=[IC1,IC];

if ifwritefile==1

disp('---------------------------------------------------')

D=[hist_close,hist_volume,drawdown,ddduration,zma,state,buyorder,...
    pnl,cpnl,peventdays,veventdays,z_relative];

temp=datasize;
if datasize >viewsize 
    start=datasize-viewsize+1;
    D=D(start:datasize,:);
    hist_date1=hist_date1(start:datasize,1);
    temp=viewsize;
end


see1=[temp,maxDD,maxDDD,maxDD_pct,mean(ret_strat)*250,sharpe_strat,mean(r1)*250,sharpe_long, ...
    sharpe_strat_out,sharpe_long_out,p2,p5,p92,p97,volatility,winingSize,losingSize,winingProb,wl_ratio,asset]

symbol(asset)
see2=[avg,stddev,sharpe,count]
limit=[xmin;xmax];

xlswrite(filename,see1,symbol{asset},'A1');
xlswrite(filename,winsize(asset,:),symbol{asset},'A2');
xlswrite(filename,strength,symbol{asset},'c2');
xlswrite(filename,see2,symbol{asset},'A3');
xlswrite(filename,hist_date1,symbol{asset},'A8');
xlswrite(filename,D,symbol{asset},'B8');
xlswrite(filename,limit,symbol{asset},'p3');
xlswrite(filename,ICs,symbol{asset},'T5');



end %writefile

end
tilefigs;
    








   


