/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Reflection;

namespace TradingBase
{
    /// <summary>
    /// Performance Evaluation based on a series of trades
    /// The last trade 
    /// </summary>
    public class PerformanceEvaluator : PerformanceMatrix
    {
        public PerformanceEvaluator() : this(.01m, 2.01m) { }

        public PerformanceEvaluator(decimal riskfreerate, decimal commission)
        {
            RiskFreeRate = riskfreerate;
            CommissionPerUnit = commission;
            PerSymbol = true;
        }

        public decimal RiskFreeRate { get { return _riskfreerate; } set { _riskfreerate = value; } }
        public decimal CommissionPerUnit { get { return _commissionperunit; } set { _commissionperunit = value; } }
        /// <summary>
        /// Generate statistics per symbol
        /// </summary>
        public bool PerSymbol { get; set; }

        List<Trade> _fills = new List<Trade>();
        Dictionary<string, Position> _positions = new Dictionary<string, Position>();

        /// <summary>
        /// If not initialized, position is empty by default
        /// </summary>
        public void InitializePositions(List<Position> pos=null)
        {
            try
            {
                _positions.Clear();
                if (pos != null)
                {
                    foreach (var p in pos)
                        _positions.Add(p.FullSymbol, p);
                }
            }
            catch (Exception e)
            {
                Debug("Can't intializae positions in evaluation report.");
                Debug(e.Message);
            }

        }
        /// <summary>
        /// Generate reports
        /// call after InitializePosition
        /// </summary>
        public void GenerateReports(List<Trade> trades)
        {
            try
            {
                _fills.Clear();
                _fills.AddRange(trades);

                List<decimal> moneyinuse = new List<decimal>();                   // money in use
                List<decimal> tradepnl = new List<decimal>();
                List<int> days = new List<int>();                           // hostorical trading days when the trades happened
                Dictionary<string, int> tradecount = new Dictionary<string, int>();         // symbol --> trade count
                List<decimal> negret = new List<decimal>(_fills.Count);

                int consecWinners = 0;
                int consecLosers = 0;
                List<long> exitscounted = new List<long>();
                decimal winpl = 0;
                decimal losepl = 0;
                
                foreach (Trade trade in _fills)
                {
                    if (tradecount.ContainsKey(trade.FullSymbol))
                        tradecount[trade.FullSymbol]++;
                    else
                        tradecount.Add(trade.FullSymbol, 1);

                    if (!days.Contains(trade.TradeDate))
                        days.Add(trade.TradeDate);

                    int usizebefore = 0;
                    decimal closedpnlfromthistrade = 0;
                    if (_positions.ContainsKey(trade.FullSymbol))
                    {
                        usizebefore = _positions[trade.FullSymbol].UnsignedSize;
                        closedpnlfromthistrade = _positions[trade.FullSymbol].Adjust(trade);                           // closed pnl

                    }
                    else
                    {
                        // add the trade to position
                        _positions.Add(trade.FullSymbol, new Position(trade));
                        usizebefore = 0;
                        closedpnlfromthistrade = 0;
                    }
                         
                    bool isroundturn = (usizebefore != 0) && (_positions[trade.FullSymbol].UnsignedSize == 0);      // end at exact 0

                    bool isclosing = _positions[trade.FullSymbol].UnsignedSize < usizebefore;
                    
                    // calculate MIU and store on array
                    decimal miu = Calc.Sum(Calc.MoneyInUse(_positions));
                    if (miu != 0)
                        moneyinuse.Add(miu);
                    // if we closed something, update return
                    if (isclosing)
                    {
                        // get p&l for portfolio
                        decimal pl = Calc.Sum(Calc.AbsoluteReturn(_positions));         // with one param, AbsoluteReturn returns closed pnl
                        // count return
                        tradepnl.Add(pl);
                        // get pct return for portfolio
                        decimal pctret = moneyinuse[moneyinuse.Count - 1] == 0 ? 0 : pl / moneyinuse[moneyinuse.Count - 1];
                        // if it is below our zero, count it as negative return
                        if (pctret < 0)
                            negret.Add(pl);
                    }
                    if (isroundturn)            // # of RoundTurns = RoundWinners + RoundLosers
                    {
                        RoundTurns++;
                        if (closedpnlfromthistrade >= 0)
                            RoundWinners++;
                        else if (closedpnlfromthistrade < 0)
                            RoundLosers++;

                    }

                    Trades++;
                    SharesTraded += Math.Abs(trade.TradeSize);
                    Commissions += CalculateIBCommissions(trade);
                    GrossPL += closedpnlfromthistrade;



                    if ((closedpnlfromthistrade > 0) && !exitscounted.Contains(trade.Id))
                    {
                        if (trade.Side)
                        {
                            SellWins++;
                            SellPL += closedpnlfromthistrade;
                        }
                        else
                        {
                            BuyWins++;
                            BuyPL += closedpnlfromthistrade;
                        }
                        if (trade.Id != 0)
                            exitscounted.Add(trade.Id);
                        Winners++;
                        consecWinners++;
                        consecLosers = 0;
                    }
                    else if ((closedpnlfromthistrade < 0) && !exitscounted.Contains(trade.Id))
                    {
                        if (trade.Side)
                        {
                            SellLosers++;
                            SellPL += closedpnlfromthistrade;
                        }
                        else
                        {
                            BuyLosers++;
                            BuyPL += closedpnlfromthistrade;
                        }
                        if (trade.Id != 0)
                            exitscounted.Add(trade.Id);
                        Losers++;
                        consecLosers++;
                        consecWinners = 0;
                    }
                    if (closedpnlfromthistrade > 0)
                        winpl += closedpnlfromthistrade;
                    else if (closedpnlfromthistrade < 0)
                        losepl += closedpnlfromthistrade;

                    if (consecWinners > ConsecWin) ConsecWin = consecWinners;
                    if (consecLosers > ConsecLose) ConsecLose = consecLosers;
                    if ((_positions[trade.FullSymbol].Size == 0) && (closedpnlfromthistrade == 0)) Flats++;
                    if (closedpnlfromthistrade > MaxWin) MaxWin = closedpnlfromthistrade;
                    if (closedpnlfromthistrade < MaxLoss) MaxLoss = closedpnlfromthistrade;
                    if (_positions[trade.FullSymbol].OpenPL > MaxOpenWin) MaxOpenWin = _positions[trade.FullSymbol].OpenPL;
                    if (_positions[trade.FullSymbol].OpenPL < MaxOpenLoss) MaxOpenLoss = _positions[trade.FullSymbol].OpenPL;

                }   // end of loop over trades

                if (Trades != 0)
                {
                    AvgPerTrade = Math.Round((losepl + winpl) / Trades, 2);
                    AvgLoser = Losers == 0 ? 0 : Math.Round(losepl / Losers, 2);
                    AvgWin = Winners == 0 ? 0 : Math.Round(winpl / Winners, 2);
                    MoneyInUse = Math.Round(Calc.Max(moneyinuse.ToArray()), 2);
                    MaxPL = Math.Round(Calc.Max(tradepnl.ToArray()), 2);
                    MinPL = Math.Round(Calc.Min(tradepnl.ToArray()), 2);
                    MaxDD = Calc.MaxDDPct(_fills);
                    SymbolCount = _positions.Count;
                    DaysTraded = days.Count;
                    GrossPerDay = Math.Round(GrossPL / days.Count, 2);
                    GrossPerSymbol = Math.Round(GrossPL / _positions.Count, 2);
                    if (PerSymbol)
                    {
                        foreach (KeyValuePair<string, Position> item in _positions)
                        {
                            PerSymbolStats.Add(item.Value.FullSymbol, tradecount[item.Value.FullSymbol] + " for " + item.Value.ClosedPL.ToString("C2"));
                        }
                    }
                }
                else
                {
                    MoneyInUse = 0;
                    MaxPL = 0;
                    MinPL = 0;
                    MaxDD = 0;
                    GrossPerDay = 0;
                    GrossPerSymbol = 0;
                }

                // ratio measures
                try
                {
                    SharpeRatio = tradepnl.Count < 2 ? 0 : Math.Round(Calc.SharpeRatio(tradepnl[tradepnl.Count - 1], Calc.StdDev(tradepnl.ToArray()), (RiskFreeRate * MoneyInUse * DaysTraded/252)), 3);
                }
                catch (Exception ex)
                {
                    Debug("sharpe error: " + ex.Message);
                }

                try
                {
                    if (tradepnl.Count == 0)
                        SortinoRatio = 0;
                    else if (negret.Count == 1)
                        SortinoRatio = 0;
                    else if ((negret.Count == 0) && (tradepnl[tradepnl.Count - 1] == 0))
                        SortinoRatio = 0;
                    else if ((negret.Count == 0) && (tradepnl[tradepnl.Count - 1] > 0))
                        SortinoRatio = 0;
                    //SortinoRatio = decimal.MaxValue;
                    else if ((negret.Count == 0) && (tradepnl[tradepnl.Count - 1] < 0))
                        SortinoRatio = 0;
                    //SortinoRatio = decimal.MinValue;
                    else
                        SortinoRatio = Math.Round(Calc.SortinoRatio(tradepnl[tradepnl.Count - 1], Calc.StdDev(negret.ToArray()), (RiskFreeRate * MoneyInUse)), 3);
                }
                catch (Exception ex)
                {
                    Debug("sortino error: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug("error in generating performance report" + ex.Message);
            }
        }

        public void FillGrid(DataTable table)
        {
            table.BeginLoadData();
            table.Clear();

            Type t = typeof(PerformanceMatrix);
            FieldInfo[] fis = t.GetFields();
            foreach (FieldInfo fi in fis)
            {
                string format = null;
                if (fi.FieldType == typeof(Decimal)) format = "{0:N2}";
                table.Rows.Add(new string[] { fi.Name, (format != null) ? string.Format(format, fi.GetValue(this)) : fi.GetValue(this).ToString() });
            }
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                if (pi.Name == "PerSymbolStats")
                    continue;
                string format = null;
                if (pi.PropertyType == typeof(Decimal)) format = "{0:N2}";
                table.Rows.Add(new string[] { pi.Name, (format != null) ? string.Format(format, pi.GetValue(this, null)) : pi.GetValue(this, null).ToString() });
            }

            foreach (KeyValuePair<string,string> kv in this.PerSymbolStats)
            {                
                table.Rows.Add(kv.Key, kv.Value);
            }

            table.EndLoadData();
        }

        private decimal CalculateIBCommissions(Trade t)
        {
            if (t.FullSymbol.Contains("STK"))
            {
                return (decimal)Math.Max(0.005 * Math.Abs(t.TradeSize), 1);
            }
            else if (t.FullSymbol.Contains("FUT"))
            {
                return 2.01m * Math.Abs(t.TradeSize);
            }
            else if (t.FullSymbol.Contains("OPT"))
            {
                return Math.Max(0.70m * Math.Abs(t.TradeSize), 1);
            }
            else if (t.FullSymbol.Contains("CASH"))
            {
                return Math.Max(0.000002m * (t.TradePrice * t.TradeSize), 2);
            }
            else
            {
                return 0;
            }
        }
        #region Auxiliary
        public event Action<string> SendDebugDelegate;

        void Debug(string msg)
        {
            if (SendDebugDelegate != null)
                SendDebugDelegate(msg);
        }
        #endregion
    }
}
