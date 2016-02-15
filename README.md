QuantTrading
============

## Project Description
A real-time quantitative trading/backtesting platform in C#, supporting IB (full brokerage) and Google Finance (quote only). It adds R support through R.NET.

## Project Summary
Currently it supports Interactive Brokers(IB) and GoogleFinance(GOOG); switch from config\mainconfg.xml.

Run program RealTimeTrading (QTShell) on top of TWS to manually trade, or

Design and derive a strategy from StrategyBase class. Backtest it in BackTestWindow Program, and then load it seamlessly into RealTimeTrading by configuring the Config/mainconfig.xml. For details, see examples in project ClassicStrategies.

DailyPreMarket and HistoricalDataDownloader are console based in order to be set up in Windows task scheduler.

### Keywords
<b>Quantitative</b>: Econometrics, Time Series, Technical Analysis, Statistical Arbitrage, Kalman Filter, Machine Learning.

<b>Technical</b>: WCF, WPF, MVVM, Rx, Prism, Concurrency, TPL, LINQ

### Installation
(1) Download and upzip; no installation needed. Compile to run or try Programs\Release\QTShell.exe.

(2) Currently it supports IB(IB) and GoogleFinance(GOOG); switch from config\mainconfg.xml.

(3) Portfolio is retrieved from config\basket.xml.

(4) The only personal information needed is google email account in config\mainconfig.xml if email notices on trading activities are desirable. Otherwise leave it blank.

(5) Some functions need statistical package R (required) and RStudio (recommended).

(6) Due to IB historical data request restriction, it may take a while for yesterday's close and change to be fully initialized (or should we load yesterday's prices along with basket file instead of requesting them dynamically?)

7) Log folder holds log files; Real time data is recorded in TickData folder. Historical bar requests are directed into HistData folder.

8) Strategy dlls are placed in the Strategy folder, which can be loaded into RealTimeTrading or BacktestWindow.

(9) It's compiled in VS 2013. If you encounter a false complaint of missing Oxyplot or R.Net, simply build again and it should get pass. 

(10) Don't forget to check out Git Repository for latest development. Enjoy !

### Figures
![alt tag](https://letianquant.files.wordpress.com/2014/10/goog1.png)

![alt tag](https://letianquant.files.wordpress.com/2015/03/multithreading.png)
