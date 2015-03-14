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

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using Modules.Framework.Interfaces;
using Modules.Framework.Events;
using TradingBase;
using System.Collections.Concurrent;

namespace Modules.Framework.Services
{
    public class BrokerService : IDisposable
    {
        private IConfigManager _configmanager;
        private IClient _client;
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IGlobalIdService _globalidservice;

        BlockingCollection<Tick> _tickqueue;
        Basket _basket;

        //private long _nextvalidorderid;
        //private string _account;
        //public long NextValidOrderId { get { return _nextvalidorderid; } }
        //public string Account { get { return _account; } }

        public BrokerService(IConfigManager configmanager, IEventAggregator eventaggregator, ILoggerFacade logger, IGlobalIdService globalidservice, BlockingCollection<Tick> tickqueue, Basket basket)
        {
            this._configmanager = configmanager;
            this._eventAggregator = eventaggregator;
            this._tickqueue = tickqueue;
            this._basket = basket;
            this._logger = logger;
            this._globalidservice = globalidservice;

            // outgoing events
            _eventAggregator.GetEvent<SendOrderEvent>().Subscribe(SendOrder);
            _eventAggregator.GetEvent<CancelOrderEvent>().Subscribe(CancelOrder);
            _eventAggregator.GetEvent<SendHistDataRequestEvent>().Subscribe(ReqHistoricalData);
        }

        public void Start()
        {
            if (_configmanager.DefaultBroker == "IB")
            {
                _client = new IBClient(_configmanager.Host, _configmanager.Port);
            }
            else
            {
                _client = new GoogleClient(_configmanager.GoogleRefreshInMilliseconds);
            }

            BindClient(ref _client);

            if (_client != null)
                _client.Connect();


            System.Threading.Thread.Sleep(new TimeSpan(0, 0, 3));        // wait 3 seconds for IB to be initialized

            _client.RequestMarketData(_basket);
        }

        public void Stop()
        {
            if (_client != null)
            {
                _client.Disconnect();
            }

            UnbindClient(ref _client);
            _client = null;
        }

        public void Dispose()
        {
            Stop();
        }

        #region client incoming messages
        private void BindClient(ref IClient client)
        {
            client.GotTickDelegate += _client_GotTickDelegate;
            client.GotFillDelegate += _client_GotFillDelegate;
            client.GotOrderDelegate += _client_GotOrderDelegate;
            client.GotOrderCancelDelegate += _client_GotOrderCancelDelegate;
            client.GotPositionDelegate += _client_GotPositionDelegate;
            client.GotHistoricalBarDelegate += _client_GotHistoricalBarDelegate;
            client.GotServerInitializedDelegate +=client_GotServerInitializedDelegate;

            client.SendDebugEventDelegate += _client_SendDebugEventDelegate;
        }

        void UnbindClient(ref IClient client)
        {
            try
            {
                client.GotTickDelegate -= _client_GotTickDelegate;
                client.GotFillDelegate -= _client_GotFillDelegate;
                client.GotOrderDelegate -= _client_GotOrderDelegate;
                client.GotOrderCancelDelegate -= _client_GotOrderCancelDelegate;
                client.GotPositionDelegate -= _client_GotPositionDelegate;
                client.GotHistoricalBarDelegate -= _client_GotHistoricalBarDelegate;
                client.GotServerInitializedDelegate -=client_GotServerInitializedDelegate;

                client.SendDebugEventDelegate -= _client_SendDebugEventDelegate;
            }
            catch { }
        }
        
        void _client_GotTickDelegate(Tick k)
        {
            // Enqueue
            // blocking without cancellation
            _tickqueue.Add(k);
        }

        void _client_GotHistoricalBarDelegate(Bar obj)
        {
            _eventAggregator.GetEvent<HistBarEvent>().Publish(obj);
        }

        void _client_GotPositionDelegate(Position obj)
        {
            _eventAggregator.GetEvent<InitialPositionEvent>().Publish(obj);
        }

        void _client_GotOrderDelegate(Order o)
        {
            _eventAggregator.GetEvent<OrderConfirmationEvent>().Publish(o);
        }

        void _client_GotOrderCancelDelegate(long obj)
        {
            _eventAggregator.GetEvent<OrderCancelConfirmationEvent>().Publish(obj);
        }

        void _client_GotFillDelegate(Trade obj)
        {
            _eventAggregator.GetEvent<OrderFillEvent>().Publish(obj);
        }

        object _lockobj = new object();
        void client_GotServerInitializedDelegate(string obj)
        {
            lock(_lockobj)
            {
                string[] msgs = obj.Split(':');

                switch (msgs[0])
                {
                    case "ServerVersion":
                        _logger.Log("Server Version: " + msgs[1], Category.Info, Priority.Medium);
                        break;
                    case "NextValidOrderId":
                        _logger.Log("Next Valid Order Id: " + msgs[1], Category.Info, Priority.Medium);
                        long id = Convert.ToInt64(msgs[1]);
                        _globalidservice.SetInitialOrderId(id - 1);    // it will increase one before retrieve
                        break;
                    case "ServerTime":
                        _logger.Log("Server Time: " + msgs[1], Category.Info, Priority.Medium);
                        break;
                    case "Account":
                        _logger.Log("Account: " + msgs[1], Category.Info, Priority.Medium);
                        _globalidservice.Account = msgs[1];
                        break;
                }
            }  
        }

        void _client_SendDebugEventDelegate(string obj)
        {
            // pass debug info out
            _logger.Log(obj, Category.Info, Priority.Medium);
        }

        #endregion

        #region Outgoing commands
        public void SendOrder(Order o)
        {
            if (_client != null)
                _client.PlaceOrder(o);
        }

        public void CancelOrder(long oid)
        {
            if (_client != null)
                _client.CancelOrder(oid);
        }

        public void ReqHistoricalData(BarRequest br)
        {
            if (_client != null)
                _client.RequestHistoricalData(br, true);
        }
        #endregion
    }
}
