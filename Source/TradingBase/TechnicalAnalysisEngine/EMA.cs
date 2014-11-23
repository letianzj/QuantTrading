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
        /// Calculates Exponential Moving Average (EMA) indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="period">Number of periods</param>
        /// <param name="uisngavg">Start with Average or start with first element</param>
        /// <returns>Object containing operation results</returns>
        public static EMAResult EMA(IEnumerable<double> input, int period, bool usingavg=true)
        {
            var returnValues = new List<double>();

            double multiplier = (2.0 / (period + 1));

            int start = usingavg ? period : 1;

            if ( input.Count() >= start)
            {
                double initialEMA = usingavg ? input.Take(period).Average() : input.First();

                returnValues.Add(initialEMA);

                var copyInputValues = input.ToList();

                for (int i = start; i < copyInputValues.Count; i++)
                {
                    var resultValue = (copyInputValues[i] - returnValues.Last()) * multiplier + returnValues.Last();

                    returnValues.Add(resultValue);
                }
            }

            var result = new EMAResult()
            {
                Values = returnValues,
                StartIndexOffset = start - 1
            };

            return result;
        }
    }
}
