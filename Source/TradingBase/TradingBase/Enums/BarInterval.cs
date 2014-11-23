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
using System.Threading.Tasks;

namespace TradingBase
{
    public enum BarInterval
    {
        /// <summary>
        /// custom volume bars
        /// </summary>
        CustomVol = -3,
        /// <summary>
        /// custom tick bars
        /// </summary>
        CustomTicks = -2,
        /// <summary>
        /// custom interval length
        /// </summary>
        CustomTime = -1,
        /// <summary>
        /// One-second intervals
        /// </summary>
        Second = 1,
        /// <summary>
        /// One-minute intervals
        /// </summary>
        Minute = 60,
        /// <summary>
        /// Five-minute interval
        /// </summary>
        FiveMin = 300,
        /// <summary>
        /// FifteenMinute intervals
        /// </summary>
        FifteenMin = 900,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        ThirtyMin = 1800,
        /// <summary>
        /// Hour-long intervals
        /// </summary>
        Hour = 3600,
        /// <summary>
        /// Day-long intervals
        /// </summary>
        Day = 86400,
    }
}
