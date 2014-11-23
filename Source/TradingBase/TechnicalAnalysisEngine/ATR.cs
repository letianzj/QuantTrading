using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechnicalAnalysisEngine;

namespace TechnicalAnalysisEngine
{
    public sealed partial class AnalysisEngine
    {
        /// <summary>
        /// Calculates Average True Range indicator
        /// </summary>
        /// <param name="highs">Signal representing price highs</param>
        /// <param name="lows">Signal representing price lows</param>
        /// <param name="closes">Signal representing closing prices</param>
        /// <param name="periods">Number of periods</param>
        /// <returns>Object containing operation results</returns>
        public static ATRResult ATR(IEnumerable<double> highs, IEnumerable<double> lows, IEnumerable<double> closes, int periods)
        {
            // calculate True Range first
            var trueRangeList = new List<double>();
            
            for (int i = 0; i < highs.Count(); i++)
            {
                double currentHigh = highs.ElementAt(i);
                double currentLow = lows.ElementAt(i);
                double previousClose = closes.ElementAtOrDefault(i - 1);

                double highMinusLow = currentHigh - currentLow;
                double highMinusPrevClose = i != 0 ? Math.Abs(currentHigh - previousClose) : 0;
                double lowMinusPrevClose = i != 0 ? Math.Abs(currentLow - previousClose) : 0;

                double trueRangeValue = highMinusLow;

                if (highMinusPrevClose > trueRangeValue)
                {
                    trueRangeValue = highMinusPrevClose;
                }
                if (lowMinusPrevClose > trueRangeValue)
                {
                    trueRangeValue = lowMinusPrevClose;
                }

                trueRangeList.Add(trueRangeValue);
            }

            // calculate Average True Range
            var atrList = new List<double>();

            double initialATR = trueRangeList.Take(periods).Average();
            atrList.Add(initialATR);

            for (int i = periods; i < trueRangeList.Count; i++)
            {
                double currentATR = (atrList[i - periods] * (periods - 1) + trueRangeList[i]) / periods;
                atrList.Add(currentATR);
            }

            var result = new ATRResult()
            {
                StartIndexOffset = periods - 1,
                Values = atrList
            };

            return result;
        }

    }
}
