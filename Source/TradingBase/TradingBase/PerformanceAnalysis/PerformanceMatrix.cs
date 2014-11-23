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

namespace TradingBase
{
    public class PerformanceMatrix
    {
        // symbol --> stats
        Dictionary<string, string> _persymstats = new Dictionary<string, string>();
        public Dictionary<string, string> PerSymbolStats { get { return _persymstats; } set { _persymstats = value; } }

        public decimal GrossPL { get; set; }
        public decimal NetPL { get { return GrossPL - Commissions; } }
        public decimal BuyPL { get; set; }
        public decimal SellPL { get; set; }
        public int Winners { get; set; }
        public int BuyWins { get; set; }
        public int SellWins { get; set; }
        public int SellLosers { get; set; }
        public int BuyLosers { get; set; }
        public int Losers { get; set; }
        public int Flats { get; set; }
        public decimal AvgPerTrade { get; set; }
        public decimal AvgWin { get; set; }
        public decimal AvgLoser { get; set; }
        public decimal MoneyInUse { get; set; }
        public decimal MaxPL { get; set; }
        public decimal MinPL { get; set; }
        public decimal MaxDD { get; set; }
        public decimal MaxWin { get; set; }
        public decimal MaxLoss { get; set; }
        public decimal MaxOpenWin { get; set; }
        public decimal MaxOpenLoss { get; set; }
        /// <summary>
        /// actual shares traded; stock has multiplier 1
        /// </summary>
        public int SharesTraded { get; set; }
        public int RoundTurns { get; set; }
        public int RoundWinners { get; set; }
        public int RoundLosers { get; set; }
        public int Trades { get; set; }
        public int SymbolCount { get; set; }
        public int DaysTraded { get; set; }
        public decimal GrossPerDay { get; set; }
        public decimal GrossPerSymbol { get; set; }
        public decimal SharpeRatio { get; set; }
        public decimal SortinoRatio { get; set; }
        public int ConsecWin { get; set; }
        public int ConsecLose { get; set; }
        public string WinSeqProbEffHyp { get { return v2s(Math.Min(100, ((decimal)Math.Pow(1 / 2.0, ConsecWin) * (Trades - Flats - ConsecWin + 1)) * 100)) + @" %"; } }
        public string LoseSeqProbEffHyp { get { return v2s(Math.Min(100, ((decimal)Math.Pow(1 / 2.0, ConsecLose) * (Trades - Flats - ConsecLose + 1)) * 100)) + @" %"; } }
        public decimal Commissions { get; set; }
        public string WLRatio { get { return v2s((Losers == 0) ? 0 : (Winners / Losers)); } }
        public string GrossMargin { get { return v2s((GrossPL == 0) ? 0 : ((GrossPL - Commissions) / GrossPL)); } }

 
        protected decimal _riskfreerate;
        protected decimal _commissionperunit;       // obsolete
        protected string v2s(decimal v) { return v.ToString("N2"); }
    }
}
