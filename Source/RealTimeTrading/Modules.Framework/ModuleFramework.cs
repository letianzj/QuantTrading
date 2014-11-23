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

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.Logging;
using System.Threading;
using log4net;
using TradingBase;

using System.Collections.Concurrent;
using Microsoft.Practices.Prism.PubSubEvents;
using Modules.Framework.Interfaces;
using Modules.Framework.Services;
using Modules.Framework.Events;

namespace Modules.Framework
{
    [Module(ModuleName="ModuleFramework")]
    [ModuleDependency("ModuleRealTimeQuoteGrid")]
    [ModuleDependency("ModuleOrderAndPositions")]
    [ModuleDependency("ModuleStrategyManager")]
    [ModuleDependency("ModuleOrderTicket")]
    public class ModuleFramework : IModule, IDisposable
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly ILoggerFacade _logger;
        private readonly IEventAggregator _eventAggregator;

        private BrokerService _brokerservice;
        private TickArchiver _tickarchiveservice;
        private QuoteDispatcherService _quotedispatcherservice;
        private ConfigManager _configmanagerservice;
        private IQuoteUpdateService _quoteupdateservice;
        private IStrategyQuoteFeedService _feedstrategyservice;
        private IGlobalIdService _globalidservice;

        private BlockingCollection<Tick> _tickqueue;
        Basket _basket;
        bool _isconnected = false;
        bool _hasconnected = false;
        DateTime _today, _preday;

        public ModuleFramework(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, 
            IQuoteUpdateService quoteserviceupdate, IStrategyQuoteFeedService feedstrategyservice, ILoggerFacade logger)
        {
            this._container = container;
            this._regionManager = regionManager;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            this._quoteupdateservice = quoteserviceupdate;
            this._feedstrategyservice = feedstrategyservice;
            this._configmanagerservice = ServiceLocator.Current.GetInstance<IConfigManager>() as ConfigManager;
            this._globalidservice = ServiceLocator.Current.GetInstance<IGlobalIdService>();
        }

        public void Initialize()
        {
            _today = DateTime.Today;
            _preday = PreviousBusinessDay();

            _eventAggregator.GetEvent<ApplicationExitEvent>().Subscribe((o) => Dispose(), true);
            _eventAggregator.GetEvent<ConnectDisconnectEvent>().Subscribe(Start);
            _eventAggregator.GetEvent<HistBarEvent>().Subscribe(ProcessPreviousClose);

            RegisterViewsAndServices();

            _logger.Log("Module ModuleFramework Loaded", Category.Info, Priority.None);

            Start(true);
        }

        private void RegisterViewsAndServices()
        {
            // 0. intialize tickqueue and basket
            _tickqueue = new BlockingCollection<Tick>(_configmanagerservice.TickQueueCapacity);
            _basket = Basket.DeserializeFromXML(Util.GetRootPath() + _configmanagerservice.SettingPath + "basket.xml");

            // 1. BrokerService, no need to register
            _brokerservice = new BrokerService(_configmanagerservice, _eventAggregator, _logger, _globalidservice, _tickqueue, _basket);

            // 2. Tick arichive service, no need to register
            _tickarchiveservice = new TickArchiver(Util.GetRootPath() + _configmanagerservice.TickPath + _configmanagerservice.DefaultBroker);
            
            // 3. QuuoteDispatcher Service, no need to register
            _quotedispatcherservice = new QuoteDispatcherService(_quoteupdateservice, _feedstrategyservice, _tickarchiveservice, _logger, _tickqueue);
        }

        private void Start(bool toconnect)
        {
            // either not connected and ask for connection
            if ( (!_isconnected) && (toconnect) )
            {
                _isconnected = toconnect;

                // 0. Initalize sec basket
                List<decimal> closeprices = new List<decimal>();
                for (int i = 0; i < _basket.Count; i++)
                {
                    closeprices.Add(0.0m);
                }
                _quoteupdateservice.InitTickerAndPreClose(_basket.Securities.ToArray(), closeprices.ToArray());

                // 1. brokerservice and initialize globalidservice
                _brokerservice.Start();
                _globalidservice.SetInitialStrategyId(0);

                // 3. QuoteDispatcherService
                _quotedispatcherservice.Start();

                // 4. Request yesterday's close in a separate thread, to avoid IB hist request limit
                if (!_hasconnected)
                {
                    _hasconnected = true;
                    Task.Factory.StartNew(() =>
                    {
                        string broker = _configmanagerservice.DefaultBroker;
                        for (int i = 0; i < _basket.Count; i++)
                        {
                            if (broker == "IB")
                                Thread.Sleep(10000);     // wait ten sec
                            else
                                Thread.Sleep(1000);      // wait one sec

                            BarRequest br = new BarRequest(_basket[i], 86400, Util.ToIntDate(_preday), 0, Util.ToIntDate(_today), 0, broker);
                            _brokerservice.ReqHistoricalData(br);
                        }
                    });
                }
            }
            //  or connected and ask for disconnection
            else if ((_isconnected) && (!toconnect))
            {
                _isconnected = toconnect;
                Stop();
            }
            else
            {
                _logger.Log("connection/disconnection order messed up", Category.Info, Priority.High);
            }
        }

        private void ProcessPreviousClose(Bar b)
        {
            _quoteupdateservice.InitTickerAndPreClose(new string[] { b.FullSymbol }, new decimal[] { b.Close });
        }

        public void Dispose()
        {
            Stop();
            _tickarchiveservice.Stop();
        }

        private void Stop()
        {
            _brokerservice.Stop();
            _quotedispatcherservice.Stop();
        }

        private DateTime PreviousBusinessDay()
        {
            DateTime date = DateTime.Today;
            do
            {
                date = date.AddDays(-1);
            }
            while ((date.DayOfWeek == DayOfWeek.Saturday) || (date.DayOfWeek == DayOfWeek.Sunday) || _configmanagerservice.Holidays.Contains(Util.ToIntDate(date).ToString()));

            return date;
        }
    }
}
