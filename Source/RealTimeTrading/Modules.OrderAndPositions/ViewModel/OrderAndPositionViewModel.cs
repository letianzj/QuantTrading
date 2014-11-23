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

using System.Collections.ObjectModel;
using Modules.Framework.Interfaces;
using Modules.OrderAndPositions.Model;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Logging;
using Modules.Framework.Events;
using TradingBase;

namespace Modules.OrderAndPositions.ViewModel
{
    public class OrderAndPositionViewModel : BindableBase
    {
        private OrderTable _ordertable = new OrderTable();
        private PositionTable _positiontable = new PositionTable();
        private FillTable _filltable = new FillTable();
        //private ResultTable _resulttable = new ResultTable();
        System.Data.DataTable _resultstable = new System.Data.DataTable("ResultsTable");

        private EventAggregator _eventaggregator;
        private readonly ILoggerFacade _logger;
        private ConfigManager _configmanager;

        PositionTracker _positiontracker;
        OrderTracker _ordertracker;
        List<Trade> _tradelist = new List<Trade>();
        PerformanceEvaluator _performevaluator;

        public OrderAndPositionViewModel()
        {
            _eventaggregator = ServiceLocator.Current.GetInstance<EventAggregator>();
            _logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            _configmanager = ServiceLocator.Current.GetInstance<IConfigManager>() as ConfigManager;

            _ordertracker = new OrderTracker(_configmanager.DailyOrderCapacity);
            _ordertracker.SendDebugEvent += OnDebug;
            _positiontracker = new PositionTracker(_configmanager.DailyOrderCapacity);

            _eventaggregator.GetEvent<InitialPositionEvent>().Subscribe(ClientGotInitialPosition);
            //_eventaggregator.GetEvent<SendOrderEvent>().Subscribe(ClientGotOrder);
            _eventaggregator.GetEvent<OrderConfirmationEvent>().Subscribe(ClientGotOrder);
            _eventaggregator.GetEvent<OrderCancelConfirmationEvent>().Subscribe(ClientGotOrderCancelConfirmation);
            _eventaggregator.GetEvent<OrderFillEvent>().Subscribe(ClientGotOrderFilled);
            _eventaggregator.GetEvent<GenerateReportEvent>().Subscribe(GeneratePerformanceReport);

            _resultstable.Columns.Add("Statistics");
            _resultstable.Columns.Add("Result");
        }

        private void ClientGotInitialPosition(Position obj)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (_positiontracker.IsTracked(obj.FullSymbol))
                {
                    int pos = PositionTable.Select(row => row.Symbol).ToList().IndexOf(obj.FullSymbol); // should exist
                    PositionTable[pos].AvgPrice = obj.AvgPrice;
                    PositionTable[pos].Size = obj.Size;
                    PositionTable[pos].ClosePL = obj.ClosedPL;
                    PositionTable[pos].OpenPL = obj.OpenPL;
                }
                else
                {
                    int count = PositionTable.Count;
                    // ?? A first chance exception system notsupportedexception presentationframework dll ??
                    PositionTable.Add(new PositionEntry(count, obj.FullSymbol, obj.AvgPrice, obj.Size, obj.ClosedPL, obj.OpenPL));
                }
            });

            _positiontracker.Adjust(obj);
        }

        private void ClientGotOrder(Order o)
        {
            int pos = OrderTable.Select(row => row.OrderId).ToList().IndexOf(o.Id);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (pos == -1)      // not found
                {
                    OnDebug("Order id " + o.Id.ToString() + " is not found in order table");
                    // it must be previous open order, or placed by tws
                    // add to _ordertracker
                    _ordertracker.GotOrder(o);
                    // update status
                    OrderTable.Add(new OrderEntry(o.Id, o.Account, o.FullSymbol, o.OrderType, o.Price, o.OrderSize, o.OrderTime, EnumDescConverter.GetEnumDescription(o.OrderStatus)));
                }
                else
                {
                    OrderTable[pos].Status = EnumDescConverter.GetEnumDescription(o.OrderStatus);
                }
            });
        }

        private void ClientGotOrderCancelConfirmation(long oid)
        {
            int pos = OrderTable.Select(row => row.OrderId).ToList().IndexOf(oid);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (pos == -1)
                {
                    OnDebug("Order id " + oid.ToString() + " is not found in order table");
                }
                else
                {
                    // order table
                    _ordertracker.GotCancel(oid);
                    OrderStatus status = OrderStatus.Canceled;
                    OrderTable[pos].Status = EnumDescConverter.GetEnumDescription(status);
                }  
            });
        }

        private void ClientGotOrderFilled(Trade k)
        {
            _tradelist.Add(k);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                // order table
                int pos = OrderTable.Select(row => row.OrderId).ToList().IndexOf(k.Id);
                if (pos == -1)
                {
                    OnDebug("Order id " + k.Id.ToString() + " is not found in order table");
                }
                else
                {
                    _ordertracker.GotFill(k);

                    if (_ordertracker[k.Id] == 0)
                    {
                        OrderStatus status = OrderStatus.Filled;
                        OrderTable[pos].Status = EnumDescConverter.GetEnumDescription(status);
                    }
                    else
                    {
                        OrderStatus status = OrderStatus.PartiallyFilled;
                        _ordertable[pos].Status = EnumDescConverter.GetEnumDescription(status);
                    }
                }

                // position table only handles one account
                // but it is guarantteed by order id
                _positiontracker.Adjust(k);
                pos = PositionTable.Select(row => row.Symbol).ToList().IndexOf(k.FullSymbol);
                if (pos == -1)
                {
                    // add new position
                    int count = PositionTable.Count;

                    PositionTable.Add(new PositionEntry(count, k.FullSymbol, _positiontracker[k.FullSymbol].AvgPrice, _positiontracker[k.FullSymbol].Size,
                        _positiontracker[k.FullSymbol].ClosedPL, _positiontracker[k.FullSymbol].OpenPL));
                }
                else
                {
                    // adjust position
                    PositionTable[pos].AvgPrice = _positiontracker[k.FullSymbol].AvgPrice;
                    PositionTable[pos].Size = _positiontracker[k.FullSymbol].Size;
                    PositionTable[pos].ClosePL = _positiontracker[k.FullSymbol].ClosedPL;
                    PositionTable[pos].OpenPL = _positiontracker[k.FullSymbol].OpenPL;
                }

                FillTable.Add(new FillEntry(k.Id, k.TradeTime, k.FullSymbol, k.TradeSize, k.TradePrice));
            });
        }

        private void GeneratePerformanceReport(int time)
        {
            _performevaluator = new PerformanceEvaluator();
            //_performevaluator.InitializePositions();
            _performevaluator.GenerateReports(_tradelist);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _performevaluator.FillGrid(_resultstable);
                TabControlSelectedIndex = 3;
            });
        }

        int _selectedOrderIndex = -1;
        public int SelectedOrderIndex
        {
            get { return _selectedOrderIndex; }
            set
            {
                if (value == _selectedOrderIndex)
                    return;

                // index changed
                _selectedOrderIndex = value;
                _eventaggregator.GetEvent<OrderGridSelectionChangedEvent>().Publish(_ordertable[_selectedOrderIndex].OrderId);
            }
        }

        int _tabcontrolselectedindex = 0;
        public int TabControlSelectedIndex
        {
            get { return _tabcontrolselectedindex; }
            set { SetProperty(ref _tabcontrolselectedindex, value); }
        }

        private void OnDebug(string msg)
        {
            _logger.Log(msg, Category.Info, Priority.None);
        }

        public OrderTable OrderTable
        {
            get { return this._ordertable; }
            set { SetProperty(ref this._ordertable, value); }
        }

        public FillTable FillTable
        {
            get { return this._filltable; }
            set { SetProperty(ref this._filltable, value); }
        }

        public PositionTable PositionTable
        {
            get { return this._positiontable; }
            set { SetProperty(ref this._positiontable, value); }
        }

        public System.Data.DataTable ResultTable { get { return _resultstable; } }
    }
}
