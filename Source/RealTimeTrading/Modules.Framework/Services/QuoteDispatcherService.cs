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
using System.Threading;
using TradingBase;
using System.Collections.Concurrent;

using System.Reactive.Linq;
using System.Reactive;

namespace Modules.Framework.Services
{
    public class QuoteDispatcherService : IDisposable
    {
        private ILoggerFacade _logger;
        IQuoteUpdateService _quoteviewupdateservice;
        IStrategyQuoteFeedService _feedstrategyquoteservice;
        TickArchiver _tickarchiveservice;

        BlockingCollection<Tick> _tickqueue;

        CancellationTokenSource _tokensource;
        CancellationToken _token;
        Task _dispatcherTask;
        bool _isStarted = false;
        int sampletime_;

        private Action<MyEventArgs<Tick>> TickReceived;

        public QuoteDispatcherService(IQuoteUpdateService quoteviewupdateservvice, IStrategyQuoteFeedService feedstrategyquoteservice, TickArchiver tickarchiveservice, ILoggerFacade logger, BlockingCollection<Tick> tickqueue)
        {
            this._quoteviewupdateservice = quoteviewupdateservvice;
            this._feedstrategyquoteservice = feedstrategyquoteservice;
            this._tickarchiveservice = tickarchiveservice;
            this._tickqueue = tickqueue;
            this._logger = logger;
            ConfigManager conf = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IConfigManager>() as ConfigManager;
            sampletime_ = conf.TickSampleTime;

            IObservable<MyEventArgs<Tick>> tickObservable = System.Reactive.Linq.Observable.FromEvent<MyEventArgs<Tick>>(
                        h => TickReceived += h, h => TickReceived -= h);
            
            // only select trade
            // .BufferWithTime(TimeSpan.FromMilliseconds(_od))
            // .Where(x => x.Count > 0)
            // .Subscribe(DataReceived, LogError);
            tickObservable.Where(e => e.Value.IsTrade).GroupBy(e => e.Value.FullSymbol)
                .Subscribe(group => group.Sample(TimeSpan.FromSeconds(sampletime_))
                    .Subscribe(QuoteDispatcher));
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _tokensource = new CancellationTokenSource();
                _token = _tokensource.Token;
                _feedstrategyquoteservice.Start();
                _dispatcherTask = Task.Factory.StartNew(() => DispatchQuotes(_token), _token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                _isStarted = true;
            }
        }

        public void Stop()
        {
            if (_isStarted)
            {
                _tokensource.Cancel();
                _feedstrategyquoteservice.Stop();
                _isStarted = false;
                _dispatcherTask = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private void DispatchQuotes(CancellationToken token)
        {
            while(true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                //****************** Trigger event, which calls quote dispatcher **************//
                Tick k = _tickqueue.Take();
                TickReceived(new MyEventArgs<Tick>(k));         // fire event
            }
        }

        /// <summary>
        /// Quote Dispatcher
        /// </summary>
        /// <param name="e"></param>
        private void QuoteDispatcher(MyEventArgs<Tick> e)
        {
            // 1. strategies
            _feedstrategyquoteservice.FeedStrategies(e.Value);
            // 2. tick archive
            _tickarchiveservice.newTick(e.Value);
            // 3. Views
            _quoteviewupdateservice.UpdateViews(e.Value);
        }
    }
}
