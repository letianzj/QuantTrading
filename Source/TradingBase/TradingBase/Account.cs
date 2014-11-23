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
    /// Brokerage account
    /// </summary>
    [Serializable]
    public class Account
    {
        public Account() { }
        public Account(string AccountID) { _id = AccountID; }
        public Account(string AccountID, string Description) { _id = AccountID; _desc = Description; }
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool isValid { get { return (ID != null) && (ID != ""); } }
        string _id = "";
        string _desc = "";
        bool _execute = true;
        bool _notify = true;
        /// <summary>
        /// Gets the ID of this account.
        /// </summary>
        /// <value>The ID.</value>
        public string ID { get { return _id; } }
        /// <summary>
        /// Gets or sets the description for this account.
        /// </summary>
        public string Desc { get { return _desc; } set { _desc = value; } }
        public bool Notify { get { return _notify; } set { _notify = value; } }
        public bool Execute { get { return _execute; } set { _execute = value; } }
        public override string ToString()
        {
            return ID;
        }
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Account o = (Account)obj;
            return Equals(o);
        }
        public bool Equals(Account a)
        {
            return this._id.Equals(a.ID);
        }
    }
}
