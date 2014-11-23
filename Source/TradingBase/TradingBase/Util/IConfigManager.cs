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
    public interface IConfigManager
    {
        /// <summary>
        /// Config file path
        /// </summary>
        string SettingPath { get; set; }
        /// <summary>
        /// Tick file path
        /// </summary>
        string TickPath { get; set; }
        /// <summary>
        /// historical bar file file
        /// </summary>
        string HistPath { get; set; }
        /// <summary>
        /// log file path
        /// </summary>
        string LogPath { get; set; }
        /// <summary>
        /// strategy dll path
        /// </summary>
        string StrategyPath { get; set; }
        string RWorkspacePath { get; set; }
        string DefaultBroker { get; set; }
        /// <summary>
        /// default account (not currently used)
        /// </summary>
        string DefaultAccount { get; set; }
        string MyAccount { get; set; }
        /// <summary>
        /// IB Api IP address
        /// </summary>
        string Host { get; set; }
        /// <summary>
        ///  IB Api Port
        /// </summary>
        int Port { get; set; }
        string User { get; set; }
        string Password { get; set; }
        int GoogleRefreshInMilliseconds { get; set; }
        string GmailFrom { get; set; }
        string GmailPass { get; set; }
        string EmailTo { get; set; }
        string RunVersion { get; set; }
        string DllName { get; set; }
        int TickQueueCapacity { get; set; }
        int DailyOrderCapacity { get; set; }
        bool EnableOversellProtect { get; set; }
        bool OversellSplitInsteadofRoundDown { get; set; }
        bool SaveIndicator { get; set; }
        bool DisableStrategyOnException { get; set; }
        bool RealTimePlot { get; set; }
        bool PlotRegularTradingHours { get; set; }


        //************************ Backtest additional setting ******************************//
        int DecimalPlace { get; set; }
        bool UseBidAskFill { get; set; }
        bool AdjustIncomingTick { get; set; }
        bool ShowTicks { get; set; }

        //************************ DailyPreMarket additional setting ******************************//
        string DailyBucket { get; set; }
        string DailyPairs { get; set; }
        string DailyResultPath { get; set; }


        //*************************************** Other ***************************************//
        List<string> Strategies { get; set; }
        List<string> Holidays { get; set; }
    }
}
