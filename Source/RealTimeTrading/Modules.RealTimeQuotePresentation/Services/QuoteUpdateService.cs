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

using System.Windows.Threading;
using Modules.Framework.Interfaces;
using Modules.RealTimeQuotePresentation.ViewModels;
using TradingBase;

namespace Modules.RealTimeQuotePresentation.Services
{
    public class QuoteUpdateService : IQuoteUpdateService
    {
        private List<string> _basket = new List<string>();

        public void UpdateViews(Tick k)
        {
            if ((GridViewModel != null) && k.IsValid)
            {
                // _uidispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => 
                {
                    GridViewModel.UpdateRow(k);
                }), DispatcherPriority.Background);

                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    PlotViewModel.GotTick(k);
                }),DispatcherPriority.Background);
            }
        }

        public void InitTickerAndPreClose(string[] symbol, decimal[] preclose)
        {
            for (int i = 0; i < symbol.Count(); i ++ )
            {
                int idx = _basket.IndexOf(symbol[i]);
                if (idx != -1)      // exist
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        GridViewModel.UpdatePreclose(symbol[i], preclose[i]);
                    });
                }
                else    // not found
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        GridViewModel.QuoteGridList.Add(new Model.QuoteGridLine(symbol[i], 0, 0m, 0m, 0, 0, preclose[i], 0m, 0m));
                    });

                    PlotViewModel.SetLineSeries(new string[] { symbol[i] });
                    _basket.Add(symbol[i]);
                }
            }
        }

        public QuoteGridViewModel GridViewModel { get; set; }
        public QuotePlotViewModel PlotViewModel { get; set; }
    }
}
