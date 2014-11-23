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

using System.Xml.Serialization;

namespace TradingBase
{
    /// <summary>
    /// Full Symbol = Serialization = Symbol + Security Type + Exchange (+ Multiplier, default is 1)
    /// It's the Full Symbol that is used throughout the program;
    /// Symbol is used only for converting to IB contract
    /// GOOGL STK SMART      (Stock)
    /// GOOGL_140920P00535000 OPT SMART 100 (Option)
    /// ESM4 FUT GLOBLEX 50   (Futures)
    /// EWQ4_C1730 FOP GLOBEX 50 (Futures Option)
    /// EUR.USD CASH IDEALPRO (spot FOREX)
    /// 6BU1 FUT GLOBELX (GBP.USD Sep 2011 FX Futures)
    /// </summary>
    public class Security
    {
        public Security(string fullsymbol)
        {
            string[] str = fullsymbol.Split(' ');

            _symbol = str[0];
            _securitytype = str[1];
            _exchange = str[2];
            _right = getoptionpart(_symbol);

            _multiplier = 1;
            if (str.Length > 3 && str[3] != "")
                _multiplier = Int32.Parse(str[3]);          
        }

        public Security(string symbol, string securitytype, string exchange="", int multiplier=1)
        {
            _symbol = symbol;
            _exchange = exchange;
            _securitytype = securitytype;
            _multiplier = multiplier;

            _right = getoptionpart(_symbol);
        }

        string _symbol = "";
        string _securitytype = "";
        string _exchange = "";
        int _multiplier = 1;                // default is 1
        string _right = "";                 // "C" or "P" or ""
        int _expiry = Util.ToIntDate(DateTime.Today);             // not implemented yet
        decimal _strike = 0m;               // not implemented yet        

        [XmlIgnore]
        public string FullSymbol { get { return ToString(); } }
        public string Symbol { get { return _symbol; } set { _symbol = value; } }        
        public string SecurityType { get { return _securitytype; } set { _securitytype = value; } }
        public string Exchange { get { return _exchange; } set { _exchange = value; } }
        public int Multiplier { get { return _multiplier; } set { _multiplier = value;} }
        public int Expiry { get { return _expiry; } set { _expiry = value; } }
        public decimal Strike { get { return _strike; } set { _strike = value; } }

        public bool IsValid { get { return _symbol != ""; } }
        public bool IsCall { get { return _right == "Call"; } }
        public bool IsPut { get { return _right == "Put"; } }
        public bool HasSecType { get { return _securitytype != ""; } }
        public bool HasDest { get { return _exchange != ""; } }

        public override string ToString()
        {
            return Serialize(this);
        }

        public static string Serialize(Security sec)
        {
            List<string> str = new List<string>();
            str.Add(sec.Symbol);
            str.Add(sec.SecurityType);
            str.Add(sec.Exchange);
            if (sec.Multiplier != 1)
                str.Add(sec.Multiplier.ToString());

            return string.Join(" ", str.ToArray());
        }

        public static Security Deserialize(string fullsymbol)
        {
            return new Security(fullsymbol);
        }

        /// <summary>
        /// The multiplier should be the last part of full symbol. Default is 1
        /// </summary>
        public static int GetMultiplierFromFullSymbol(string fullsymbol)
        {
            int multiplier = 1;

            string[] str = fullsymbol.Split(' ');
            
            //if (str.Length > 3)
            //{
                bool success = Int32.TryParse(str[str.Length-1], out multiplier);
                if (!success)
                    multiplier = 1;
            //}
            return multiplier;
        }

        private string getoptionpart(string symbol)
        {
            string right = "";

            string[] str = symbol.Split('_');
            if (str.Length > 1)         // Option part
            {
                if (str[1].ToUpper().Contains("C"))
                    right = "Call";
                else if (str[1].ToUpper().Contains("P"))
                    right = "Put";
            }

            return right;
        }
    }
}
