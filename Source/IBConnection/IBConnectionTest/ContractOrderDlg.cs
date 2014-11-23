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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using IBApi;

namespace IBConnectionTest
{
    public partial class ContractOrderDlg : Form
    {
        public Contract _contract;
        public Order _order;

        public ContractOrderDlg()
        {
            InitializeComponent();

            tbRequestId.Text = "0";

            // Contract Description
            tbContractId.Text = "0";
            tbSymbol.Text = "SPY";
            tbType.Text = "STK";
            tbExpiry.Text = "";
            tbStrike.Text = "0.0";
            tbRight.Text = "";      
            tbMultiplier.Text = "";
            tbExchange.Text = "SMART";
            tbPrimaryExchange.Text = "";//"ISLAND";
            tbCurrency.Text = "USD";           
            tbLocalSymbol.Text = "";
            tbIncludeExpired.Text = "0";
            tbSecIdType.Text = "";
            tbSecId.Text = "";

            // Market Data
            tbGenericTickTags.Text = "100,101,104,105,106,107,165,221,225,233,236,258,293,294,295,318";
            cbSnapshot.Checked = false;

            // Market Depth
            tbNumOfRows.Text = "20";

            // Exercise Option
            tbAction.Text = "1";
            tbNumOfContracts.Text = "1";
            tbOverride.Text = "0";
            
            // Order Description
            tbAction.Text = "BUY";
            tbTotalOrderSize.Text = "10";
            tbOrderType.Text = "LMT";
            tbOrderPrice.Text = "40.0";
            tbAuxPrice.Text = "0.0";
            tbGoodAfterTime.Text = "";
            tbGoodTillTime.Text = "";
            
            // Historical Data
            tbEndDate.Text = DateTime.Today.ToString("yyyyMMdd HH:mm:ss");
            tbDuration.Text = "6";
            tbBarSize.Text = "60";
            tbWhatToShow.Text = "TRADES";
            tbRTH.Text = "1";
            tbDateFormat.Text = "1";

            // Market Data Type
        }

        public Contract getContract()
        {
            Contract contract = new Contract();
            // Only Symbol, SecType, and Exchagne are necessary
            contract.Symbol = tbSymbol.Text;
            contract.SecType = tbType.Text;
            contract.Exchange = tbExchange.Text;

            contract.ConId = Int32.Parse(tbContractId.Text, CultureInfo.InvariantCulture);
            contract.Expiry = tbExpiry.Text;
            contract.Strike = Double.Parse(tbStrike.Text, CultureInfo.InvariantCulture);
            contract.Right = tbRight.Text;
            contract.Multiplier = tbMultiplier.Text;
            contract.PrimaryExch = tbPrimaryExchange.Text;
            contract.Currency = tbCurrency.Text;
            contract.LocalSymbol = tbLocalSymbol.Text;
            contract.IncludeExpired = Int32.Parse(tbIncludeExpired.Text, CultureInfo.InvariantCulture) == 0 
                ? false : true;
            contract.SecIdType = tbSecIdType.Text;
            contract.SecId = tbSecId.Text;

            return contract;
        }

        public Order getOrder()
        {
            Order order = new Order();

            order.Action = tbOrderAction.Text;
            order.TotalQuantity = Int32.Parse(tbTotalOrderSize.Text, CultureInfo.InvariantCulture);
            order.OrderType = tbOrderType.Text;
            double limit = Double.Parse(tbOrderPrice.Text, CultureInfo.InvariantCulture);
            if (limit != 0.0)
            {
                order.LmtPrice = limit;
            }
            double stop = Double.Parse(tbAuxPrice.Text, CultureInfo.InvariantCulture);
            if (stop != 0.0)
            {
                order.AuxPrice = stop;
            }

            return order;
        }
    }
}
