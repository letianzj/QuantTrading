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

        public QuoteDispatcherService(IQuoteUpdateService quoteviewupdateservvice, IStrategyQuoteFeedService feedstrategyquoteservice, TickArchiver tickarchiveservice, ILoggerFacade logger, BlockingCollection<Tick> tickqueue)
        {
            this._quoteviewupdateservice = quoteviewupdateservvice;
            this._feedstrategyquoteservice = feedstrategyquoteservice;
            this._tickarchiveservice = tickarchiveservice;
            this._tickqueue = tickqueue;
            this._logger = logger;


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

                //********************  quote dispatcher **************//
                Tick k = _tickqueue.Take();
                // 1. strategies
                _feedstrategyquoteservice.FeedStrategies(k);
                // 2. tick archive
                _tickarchiveservice.newTick(k);
                // 3. Views
                _quoteviewupdateservice.UpdateViews(k);
            }
        }
    }
}
