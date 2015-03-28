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
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.PubSubEvents;
using Modules.Framework.Services;
using Modules.Framework.Interfaces;
using Modules.Framework.Events;
using Modules.StrategyManager.Model;
using Modules.StrategyManager.Services;
using TradingBase;

namespace Modules.StrategyManager.ViewModel
{
    /// <summary>
    /// StrategyGridViewModel doesn't track the registration of strategies,
    /// so each strategy should track its security of interest and orders it placed.
    /// </summary>
    public class StrategyGridViewModel : BindableBase
    {
        GlobalIdService _globalIdService;
        IConfigManager _configManagerService;
        ILoggerFacade _logger;
        IEventAggregator _eventAggregator;
        StrategyQuoteFeedService _quotefeedservice;

        public  strategyItemList _strategyitemlist = new strategyItemList();
        Dictionary<int, int> _sid2s = new Dictionary<int, int>();             // strategy id to index in _strategyitemlist

        public StrategyGridViewModel()
        {
            _globalIdService = ServiceLocator.Current.GetInstance<IGlobalIdService>() as GlobalIdService;
            _configManagerService = ServiceLocator.Current.GetInstance<IConfigManager>() as ConfigManager;
            _logger = ServiceLocator.Current.GetInstance<ILoggerFacade>();
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            _quotefeedservice = ServiceLocator.Current.GetInstance<IStrategyQuoteFeedService>() as StrategyQuoteFeedService;
            _quotefeedservice.QuoteFeeViewModel = this;

            _eventAggregator.GetEvent<InitialPositionEvent>().Subscribe(ClientGotInitialPosition);
            _eventAggregator.GetEvent<HistBarEvent>().Subscribe(ClientGotHistBar);
            //_eventaggregator.GetEvent<SendOrderEvent>().Subscribe(ClientGotOrder);
            _eventAggregator.GetEvent<OrderConfirmationEvent>().Subscribe(ClientGotOrder);
            _eventAggregator.GetEvent<OrderCancelConfirmationEvent>().Subscribe(ClientGotOrderCancelConfirmation);
            _eventAggregator.GetEvent<OrderFillEvent>().Subscribe(ClientGotOrderFilled);

            LoadStrategies();
        }

        public strategyItemList StrategyItemList
        {
            get { return this._strategyitemlist; }
            set { SetProperty(ref this._strategyitemlist, value); }
        }

        private void LoadStrategies()
        {
            string root = Util.GetRootPath();
            foreach (string s in _configManagerService.Strategies)
            {
                string[] ss = s.Split('.');

                var r = Util.GetSingleStrategyFromDLL(s, root + @"\Strategies\" + _configManagerService.RunVersion + "\\" + ss[0] + ".dll");
                r.ID = _globalIdService.GetNextStrategyId();
                r.Reset();
                BindStrategy(ref r);
                r.SetIdTracker(_globalIdService);           // Set Ordr ID tracker
                r.IsActive = false;

                StrategyItem sr = new StrategyItem(r);
                _strategyitemlist.Add(sr);
                _sid2s.Add(r.ID, _strategyitemlist.Count - 1);
            }
        }

        void SendDebug(string msg)
        {
            _logger.Log(msg, Category.Info, Priority.None);
        }

        void BindStrategy(ref StrategyBase tmp)
        {
            // handle all the outgoing events from the strategy
            tmp.SendDebugEvent += _strategy_GotDebug;
            tmp.SendOrderEvent += _strategy_SendOrder;
            tmp.SendCancelEvent += _strategy_CancelOrderSource;
            tmp.SendBasketEvent += _strategy_SendBasket;
            tmp.SendReqHistBarEvent += _strategy_SendReqHistBar;
            tmp.SendChartLabelEvent += _strategy_SendChartLabel;
            tmp.SendIndicatorsEvent += _strategy_SendIndicators;
        }

        void UnBindStrategy(ref StrategyBase tmp)
        {
            // handle all the outgoing events from the strategy
            tmp.SendDebugEvent -= _strategy_GotDebug;
            tmp.SendOrderEvent -= _strategy_SendOrder;
            tmp.SendCancelEvent -= _strategy_CancelOrderSource;
            tmp.SendBasketEvent -= _strategy_SendBasket;
            tmp.SendReqHistBarEvent -= _strategy_SendReqHistBar;
            tmp.SendChartLabelEvent -= _strategy_SendChartLabel;
            tmp.SendIndicatorsEvent -= _strategy_SendIndicators;
        }

        #region send out commands
        /// <summary>
        /// Create order from StrategyBase (as oppose to manual order)
        /// </summary>
        /// <param name="o"></param>
        void _strategy_SendOrder(Order o, int sid)
        {
            if (!_sid2s.ContainsKey(sid))
            {
                SendDebug("Ignoring order from strategy with invalid id: " + sid + " index not found. order: " + o.ToString());
                return;
            }
            if (!_strategyitemlist[_sid2s[sid]].isSActive)
            {
                SendDebug("Ignoring order from disabled/inactive strategy: " + _strategyitemlist[_sid2s[sid]].SName + " order: " + o.ToString());
                return;
            }

            // set account on order
            if (o.Account == string.Empty)
                o.Account = _globalIdService.Account;
            if (string.IsNullOrEmpty(o.FullSymbol))
                throw (new Exception("Order is missing underlying fullsymbol"));

            // assign order id before send out
            if (o.Id == 0)
                o.Id = _globalIdService.GetNextOrderId();
            // send order and get error message
            try
            {
                o.OrderStatus = OrderStatus.PendingSubmit;
                _eventAggregator.GetEvent<SendOrderEvent>().Publish(o);
            }
            catch (Exception ex)
            {
                SendDebug("Place order error: " + ex.Message);
            }
        }

        void _strategy_SendBasket(Basket b, int id)
        {
            SendDebug("Basket subscription not implemented yet. StrategyBase should use the basket from basket.xml");
        }

        void _strategy_SendReqHistBar(BarRequest br)
        {
            _eventAggregator.GetEvent<SendHistDataRequestEvent>().Publish(br);
        }

        void _strategy_SendIndicators(int idx, string param)
        {
            _logger.Log("rid=" + idx + ", name=" + _strategyitemlist[_sid2s[idx]].SName + ", indicator=" + param, 
                Category.Info, Priority.Medium);
        }

        void _strategy_SendChartLabel(decimal price, int bar, string label, System.Drawing.Color c)
        {
            SendDebug("sendchart is not supported at this time.");
        }

        void _strategy_CancelOrderSource(long orderid, int sid)
        {
            if (!_sid2s.ContainsKey(sid))
            {
                SendDebug("strategy id not found for this strategy who wants to cancel order: " + sid);
                return;
            }

            if (!_strategyitemlist[_sid2s[sid]].isSActive)
            {
                SendDebug("Ignoring cancel from disabled strategy: " + _strategyitemlist[_sid2s[sid]].SName + " orderid: " + orderid);
                return;
            }

            _eventAggregator.GetEvent<CancelOrderEvent>().Publish(orderid);
        }

        void _strategy_GotDebug(string msg)
        {
            // display to screen
            SendDebug(msg);
        }
        #endregion

        #region incoming commands
        private void ClientGotInitialPosition(Position obj)
        {
            foreach (StrategyItem si in _strategyitemlist)
            {
                si.S.GotPosition(obj);
            }
        }

        /// <summary>
        /// Previous day daily bar
        /// </summary>
        private void ClientGotHistBar(Bar br)
        {
            foreach (StrategyItem si in _strategyitemlist)
            {
                si.S.GotHistoricalBar(br);
            }
        }

        private void ClientGotOrder(Order o)
        {
            foreach (StrategyItem si in _strategyitemlist)
            {
                si.S.GotOrder(o);
            }
        }

        private void ClientGotOrderCancelConfirmation(long oid)
        {
            foreach (StrategyItem si in _strategyitemlist)
            {
                si.S.GotOrderCancel(oid);
            }
        }

        private void ClientGotOrderFilled(Trade k)
        {
            foreach (StrategyItem si in _strategyitemlist)
            {
                si.S.GotFill(k);
            }
        }
        #endregion
    }
}