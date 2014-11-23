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

using System.Threading;
using System.Collections.Concurrent;
using Modules.Framework.Interfaces;
using Modules.StrategyManager.Model;
using Modules.StrategyManager.ViewModel;
using TradingBase;

namespace Modules.StrategyManager.Services
{
    public class StrategyQuoteFeedService : IStrategyQuoteFeedService
    {
        private int _numProcs = Environment.ProcessorCount;
        private ConcurrentDictionary<string, Tick> _tickdictionary;
        public StrategyGridViewModel QuoteFeeViewModel { get; set; }

        private CancellationTokenSource _tokensource;
        private CancellationToken _cancellationtoken;
        
        public StrategyQuoteFeedService()
        {
            // Capacity = 150 ticks
            _tickdictionary = new ConcurrentDictionary<string,Tick>(_numProcs*2, 150);
        }

        /// <summary>
        /// Feed tick to stategies, no matter it's active or not; in case it will turned back to active.
        /// </summary>
        /// <param name="k"></param>
        public void FeedStrategies(Tick t)
        {
            _tickdictionary.AddOrUpdate(t.FullSymbol, t, (key,oldvalue) => t);
        }

        public void Start()
        {
            _tokensource = new CancellationTokenSource();
            _cancellationtoken = _tokensource.Token;

            var task = Task.Factory.StartNew(() => StrategyLoop(_cancellationtoken), _cancellationtoken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            /*
            Task.Factory.StartNew(() =>
            {
                foreach (StrategyItem si in QuoteFeeViewModel.StrategyItemList)
                {
                    si.S.GotTick(k);
                }
            });
            */
        }

        public void Stop()
        {
            _tokensource.Cancel();
        }

        private void StrategyLoop(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                System.Threading.Tasks.Parallel.ForEach<StrategyItem>(QuoteFeeViewModel.StrategyItemList, si =>
                {
                    foreach(string sym in si.S.Symbols)
                    {
                        Tick k;
                        if (_tickdictionary.TryGetValue(sym, out k))
                        {
                            si.S.GotTick(k);
                        }
                    }
                });
            }
        }
    }
}
