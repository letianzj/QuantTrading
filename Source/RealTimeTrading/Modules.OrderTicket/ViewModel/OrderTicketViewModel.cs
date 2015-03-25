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

using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Modules.Framework.Events;
using Modules.Framework.Interfaces;
using Modules.Framework.Services;
using TradingBase;

namespace Modules.OrderTicket.ViewModel
{
    public class OrderTicketViewModel : BindableBase
    {
        EventAggregator _eventaggregator = ServiceLocator.Current.GetInstance<EventAggregator>();
        IGlobalIdService _globalIdService;

        public OrderTicketViewModel()
        {
            _globalIdService = ServiceLocator.Current.GetInstance<IGlobalIdService>();
            this.PlaceOrderCmd = new DelegateCommand<object>(this.OnPlaceOrder);
            this.CancelOrderCmd = new DelegateCommand<object>(this.OnCancelOrder);
            this.GenerateReportCmd = new DelegateCommand<object>(this.OnGenerateReport);
            this.ConnectDisconnectCmd = new DelegateCommand<object>(this.OnConnectDisconnect);

            _eventaggregator.GetEvent<SymbolGridSelectionChangedEvent>().Subscribe(QuoteRowChangedHandler, ThreadOption.UIThread);
            _eventaggregator.GetEvent<OrderGridSelectionChangedEvent>().Subscribe(OrderRowChangedHandler, ThreadOption.UIThread);
        }

        private void QuoteRowChangedHandler(string symbol)
        {
            FullSymbol = symbol;
        }

        private void OrderRowChangedHandler(long oid)
        {
            SelectedOrderId = oid;
        }

        #region binding
        string _fullsymbol;
        public string FullSymbol
        {
            get { return _fullsymbol; }
            set
            {
                SetProperty(ref _fullsymbol, value);
                if (_fullsymbol.Contains("STK"))
                    SizeIncrement = 100;
                else
                    SizeIncrement = 1;
            }
        }

        long _selectedorderid;
        public long SelectedOrderId
        {
            get { return _selectedorderid; }
            set {SetProperty(ref _selectedorderid, value);}
        }

        int _selectedbuysell = 0;
        public int SelectedBuySell
        {
            get { return _selectedbuysell; }
            set
            {
                SetProperty(ref _selectedbuysell, value);
            }
        }

        int _selectedordertype = 0;
        public int SelectedOrderType
        {
            get { return _selectedordertype; }
            set
            {
                SetProperty(ref _selectedordertype, value);
            }
        }

        decimal _price;
        public decimal Price
        {
            get { return _price; }
            set
            {
                SetProperty(ref _price, value);
            }
        }

        decimal _auxprice;
        public decimal AuxPrice
        {
            get { return _auxprice; }
            set
            {
                SetProperty(ref _auxprice, value);
            }
        }

        int _size;
        public int Size
        {
            get { return _size; }
            set
            {
                SetProperty(ref _size, value);
            }
        }

        int _incsize;
        public int SizeIncrement
        {
            get { return _incsize; }
            set
            {
                SetProperty(ref _incsize, value);
            }
        }

        string _connectdisconnectbuttontext = "Disconnect";
        public string ConnectDisconnectButtonText
        {
            get { return _connectdisconnectbuttontext; }
            set
            {
                SetProperty(ref _connectdisconnectbuttontext, value);
            }
        }

        #endregion

        #region commands
        public System.Windows.Input.ICommand PlaceOrderCmd { get; private set; }
        public System.Windows.Input.ICommand CancelOrderCmd { get; private set; }

        public System.Windows.Input.ICommand GenerateReportCmd { get; private set; }

        public System.Windows.Input.ICommand ConnectDisconnectCmd { get; private set; }

        private void OnPlaceOrder(object obj) 
        {
            Order _workingorder = null;

            switch (_selectedordertype)
            {
                case 0:
                    _workingorder = new MarketOrder(_fullsymbol, _selectedbuysell == 0 ? _size : -_size, _globalIdService.GetNextOrderId());
                    //_workingorder.StopPrice = 0;
                    //_workingorder.LimitPrice = 0;
                    break;
                case 1:
                    _workingorder = new LimitOrder(_fullsymbol, _selectedbuysell == 0 ? _size : -_size, _price, _globalIdService.GetNextOrderId());
                    //_workingorder.StopPrice = 0;
                    //_workingorder.LimitPrice = _price;
                    break;
                case 2:
                    _workingorder = new StopOrder(_fullsymbol, _selectedbuysell == 0 ? _size : -_size, _price, _globalIdService.GetNextOrderId());
                    //_workingorder.StopPrice = _price;
                    //_workingorder.LimitPrice = 0;
                    break;
                case 3:
                    _workingorder = new StopLimitOrder(_fullsymbol, _selectedbuysell == 0 ? _size : -_size, _price, _auxprice,_globalIdService.GetNextOrderId());
                    break;
                case 4:
                    _workingorder = new TrailingStopOrder(_fullsymbol, _selectedbuysell == 0 ? _size : -_size, _auxprice, _globalIdService.GetNextOrderId());
                    break;
                case 5:
                    _workingorder = new TrailingStopLimitOrder(_fullsymbol, _selectedbuysell == 0 ? _size : -_size, _price, _auxprice, _globalIdService.GetNextOrderId());
                    break;
            }

            _workingorder.OrderStatus = OrderStatus.PendingSubmit;
            _eventaggregator.GetEvent<SendOrderEvent>().Publish(_workingorder);
        }
        //private bool CanPlaceOrder() { return (Size != 0); }

        private void OnCancelOrder(object obj)
        {
            _eventaggregator.GetEvent<CancelOrderEvent>().Publish(_selectedorderid);
        }

        private void OnGenerateReport(object obj)
        {
            _eventaggregator.GetEvent<GenerateReportEvent>().Publish(Util.ToIntTime(DateTime.Now));
        }

        private void OnConnectDisconnect(object obj)
        {
            // To disconnect
            if (_connectdisconnectbuttontext == "Disconnect")
            {
                ConnectDisconnectButtonText = "Connect";
                _eventaggregator.GetEvent<ConnectDisconnectEvent>().Publish(false);
            }
            else if (_connectdisconnectbuttontext == "Connect")
            {
                ConnectDisconnectButtonText = "Disconnect";
                _eventaggregator.GetEvent<ConnectDisconnectEvent>().Publish(true);
            }
        }

        #endregion
    }
}
