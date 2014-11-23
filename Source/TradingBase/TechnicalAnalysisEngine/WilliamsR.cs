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
        /// Calculates Williams %R indicator
        /// </summary>
        /// <param name="highs">Signal representing price highs</param>
        /// <param name="lows">Signal representing price lows</param>
        /// <param name="closes">Signal representing closing prices</param>
        /// <param name="periods">Number of periods</param>
        /// <returns>Object containing operation results</returns>
        public static WilliamsRResult WilliamsR(IEnumerable<double> highs, IEnumerable<double> lows, IEnumerable<double> closes, int periods)
        {
            var outputValues = new List<double>();

            for (int i = periods - 1; i < highs.Count(); i++)
            {
                double highestHigh = highs.Skip(i - periods + 1).Take(periods).Max();
                double lowestLow = lows.Skip(i - periods + 1).Take(periods).Min();

                double currentClose = closes.ElementAt(i);

                double value = ((highestHigh - currentClose) / (highestHigh - lowestLow)) * -100;
                outputValues.Add(value);
            }

            var result = new WilliamsRResult()
            {
                StartIndexOffset = periods - 1,
                Values = outputValues
            };

            return result;
        }
    }
}
