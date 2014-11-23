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
    /// <summary>
    /// list of accepted special order instructions
    /// </summary>
    [Serializable()] 
    public enum TimeInForce
    {
        Invalid = -2,
        None = -1,
        /// <summary>
        /// day order
        /// </summary>
        DAY = 0,
        /// <summary>
        /// good till canceled
        /// </summary>
        GTC = 1,
        /// <summary>
        /// market on close
        /// </summary>
        MOC = 2,
        /// <summary>
        /// opening order
        /// </summary>
        OPG = 4,
        /// <summary>
        /// immediate or cancel
        /// </summary>
        IOC = 8,
        /// <summary>
        /// pegged to mid-market
        /// </summary>
        PEG2MID = 32,
        /// <summary>
        /// pegged to market
        /// </summary>
        PEG2MKT = 64,
        /// <summary>
        /// pegged to primary
        /// </summary>
        PEG2PRI = 128,
        /// <summary>
        /// pegged to best
        /// </summary>
        PEG2BST = 256,
        /// <summary>
        /// hidden
        /// </summary>
        HIDDEN = 512,
    }
}
