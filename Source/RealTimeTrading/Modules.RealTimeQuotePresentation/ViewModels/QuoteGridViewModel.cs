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

using Modules.Framework.Interfaces;
using Modules.RealTimeQuotePresentation.Model;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Modules.RealTimeQuotePresentation.Services;
using Modules.Framework.Events;
using TradingBase;

namespace Modules.RealTimeQuotePresentation.ViewModels
{
    public class QuoteGridViewModel : BindableBase
    {
        private QuoteGridList _gridList;
        private EventAggregator _eventaggregator;
        private QuoteUpdateService _quoteupdateservice;

        public QuoteGridViewModel()
        {
            
            this._gridList = new QuoteGridList();
            _quoteupdateservice = ServiceLocator.Current.GetInstance<IQuoteUpdateService>() as QuoteUpdateService;
            _eventaggregator = ServiceLocator.Current.GetInstance<EventAggregator>();
            _quoteupdateservice.GridViewModel = this;
        }

        public QuoteGridList QuoteGridList
        {
            get { return this._gridList; }
            set { SetProperty(ref this._gridList, value); }
        }

        int _selectedsymbolindex = -1;
        public int SelectedSymbolIndex
        {
            get { return _selectedsymbolindex; }
            set
            {
                if (value == _selectedsymbolindex)
                    return;

                // index changed
                _selectedsymbolindex = value;
                _eventaggregator.GetEvent<SymbolGridSelectionChangedEvent>().Publish(QuoteGridList[_selectedsymbolindex].Symbol);
            }
        }

        public void UpdateRow(Tick k)
        {
            // bool exist = GridViewModel.QuoteGridList.Any(x => x.Symbol == k.FullSymbol);
            var item = QuoteGridList.FirstOrDefault(i => i.Symbol == k.FullSymbol);

            if (item != null)
            {
                if (k.HasBid)
                {
                    item.BidPair = new QuoteGridLine.QuotePair(item.BidPair._newvalue, k.BidPrice);    
                    item.BidSize = k.BidSize;
                }
                if (k.HasAsk)
                {
                    item.AskPair = new QuoteGridLine.QuotePair(item.AskPair._newvalue, k.AskPrice);
                    item.AskSize = k.AskSize;
                }

                if (k.IsTrade)
                {
                    // decimal old_trade = (decimal)item.TradePair._newvalue;
                    item.TradePair = new QuoteGridLine.QuotePair(item.TradePair._newvalue, k.TradePrice);
                    item.Size = k.TradeSize;
                    
                    //item.TradeColor = k.TradePrice > old_trade ? System.Windows.Media.Colors.Green
                    //   : (k.TradePrice == old_trade ? System.Windows.Media.Colors.White : System.Windows.Media.Colors.Red);

                    if (item.PreClose != 0m)
                    {
                        item.Change = k.TradePrice - item.PreClose;
                        item.ChangePercentage = String.Format("{0:P3}.", k.TradePrice / item.PreClose - 1);
                    }
                }
            }
        }

        public void UpdatePreclose(string sym, decimal preclose)
        {
            var item = QuoteGridList.FirstOrDefault(i => i.Symbol == sym);
            if (item != null)
            {
                item.PreClose = preclose;
                if (item.PreClose != 0m)
                {
                    item.Change = item.TradePair._newvalue - item.PreClose;
                    item.ChangePercentage = String.Format("{0:P3}.", item.TradePair._newvalue / item.PreClose - 1);
                }   
            }
        }
    }
}
