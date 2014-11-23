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
using System.Threading.Tasks;

namespace TradingBase
{
    public interface BarIntervalData
    {
        int Last();
        int Count();
        void Reset();
        bool IsRecentNew();
        List<decimal> Open();
        List<decimal> Close();
        List<decimal> High();
        List<decimal> Low();
        List<long> Vol();
        List<int> Date();
        List<int> BarOrder();
        /// <summary>
        /// string symbol, int interval
        /// </summary>
        event Action<string, int>  NewBarHandler;
        Bar GetBar(int index, string symbol);
        Bar GetBar(string symbol);
        void NewPoint(string symbol, decimal p, int time, int date, int size);
        void NewTick(Tick k);
        void AddBar(Bar b);
    }
}
