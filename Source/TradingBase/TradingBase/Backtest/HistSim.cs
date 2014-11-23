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
    public interface HistSim
    {
        event Action<Tick> GotTickHandler;
        event Action<string> GotDebugHandler;

        /// <summary>
        /// reset simulation
        /// </summary>
        void Reset();
        /// <summary>
        /// start simulation and run to specified date/time
        /// </summary>
        /// <param name="ftime"></param>
        void PlayTo(long ftime);
        /// <summary>
        /// stop simulation
        /// </summary>
        void Stop();

        /// <summary>
        /// Total ticks available for processing, based on provided filter or tick files.
        /// </summary>
        int TicksPresent { get; }
        /// <summary>
        /// Ticks processed in this simulation run.
        /// </summary>
        int TicksProcessed { get; }
        /// <summary>
        /// Gets next tick in the simulation
        /// </summary>
        long NextTickTime { get; }
    }
}
