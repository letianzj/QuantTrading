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
    public struct BarRequest
    {
        /// <summary>
        /// client making request
        /// </summary>
        public string Client;
        public int StartDate;
        public int EndDate;
        public int StartTime;
        public int EndTime;
        public int CustomInterval;
        public string FullSymbol;
        public int Interval;
        public long ID;
        public DateTime StartDateTime { get { return Util.ToDateTime(StartDate, StartTime); } }
        public DateTime EndDateTime { get { return Util.ToDateTime(EndDate, EndTime); } }
        public BarRequest(string fullsymbol, int interval, int startdate, int starttime, int enddate, int endtime, string client)
        {
            Client = client;
            FullSymbol = fullsymbol;
            Interval = interval;
            StartDate = startdate;
            StartTime = starttime;
            EndDate = enddate;
            EndTime = endtime;
            ID = 0;
            CustomInterval = 0;
        }

        public override string ToString()
        {
            return FullSymbol + " " + Interval + " " + StartDateTime + "->" + EndDateTime;
        }
    }
}
