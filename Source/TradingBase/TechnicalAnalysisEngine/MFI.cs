using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechnicalAnalysisEngine;

namespace TechnicalAnalysisEngine
{
    public sealed partial class AnalysisEngine
    {
        private class MFIMoneyFlow
        {
            public double PositiveMoneyFlow { get; set; }
            public double NegativeMoneyFlow { get; set; }
        }

        /// <summary>
        /// Calculates Money Flow Index (MFI) indicator
        /// </summary>
        /// <param name="highs">Signal representing price highs</param>
        /// <param name="lows">Signal representing price lows</param>
        /// <param name="closes">Signal representing closing prices</param>
        /// <param name="periods">Number of periods</param>
        /// <param name="volume">Signal representing daily volumes</param>
        /// <returns>Object containing operation results</returns>
        public static MFIResult MFI(IEnumerable<double> highs, IEnumerable<double> lows, IEnumerable<double> closes, IEnumerable<int> volume, int periods)
        {
            double lastTypicalPrice = (highs.ElementAt(0) + lows.ElementAt(0) + closes.ElementAt(0)) / 3;

            var moneyFlowList = new List<MFIMoneyFlow>();

            for (int i = 1; i < highs.Count(); i++)
            {
                double typicalPrice = (highs.ElementAt(i) + lows.ElementAt(i) + closes.ElementAt(i)) / 3;
                bool up = typicalPrice > lastTypicalPrice;
                lastTypicalPrice = typicalPrice;

                double rawMoneyFlow = typicalPrice * volume.ElementAt(i);

                var moneyFlow = new MFIMoneyFlow();
                
                if (up)
                {
                    moneyFlow.PositiveMoneyFlow = rawMoneyFlow;
                }
                else
                {
                    moneyFlow.NegativeMoneyFlow = rawMoneyFlow;
                }

                moneyFlowList.Add(moneyFlow);
            }

            var moneyFlowIndexList = new List<double>();

            for (int i = 0; i < moneyFlowList.Count - periods + 1; i++)
            {
                var range = moneyFlowList.GetRange(i, periods);

                double positiveMoneyFlow = range.Sum(x => x.PositiveMoneyFlow);
                double negativeMoneyFlow = range.Sum(x => x.NegativeMoneyFlow);
                double moneyFlowRatio = positiveMoneyFlow / negativeMoneyFlow;
                double moneyFlowIndex = 100 - 100 / (1 + moneyFlowRatio);

                moneyFlowIndexList.Add(moneyFlowIndex);
           }

            var result = new MFIResult()
            {
                Values = moneyFlowIndexList,
                StartIndexOffset = periods
            };

            return result;
        }
    }
}
