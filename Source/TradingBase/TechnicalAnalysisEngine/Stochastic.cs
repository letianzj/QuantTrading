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
        /// Calculates Stochastic indicator
        /// KDJ is derived from Stoch.
        /// K = Dline
        /// D = Slow-Dline; dfault perido = 3
        /// J = 3D-2K
        /// </summary>
        /// <param name="highs">Signal representing price highs</param>
        /// <param name="lows">Signal representing price lows</param>
        /// <param name="closes">Signal representing closing prices</param>
        /// <param name="kPeriods">Number of periods for %K line; default=5</param>
        /// <param name="dPeriods">Number of periods for %D line; default=3</param>
        /// <returns>Object containing operation results</returns>
        public static StochasticResult Stochastic(IEnumerable<double> highs, IEnumerable<double> lows, IEnumerable<double> closes, int kPeriods, int dPeriods)
        {
            int startKIndex = kPeriods - 1;
            int startDIndex = startKIndex + 2;

            var outputKLine = new List<double>();
            var outputDLine = new List<double>();

            for (int i = kPeriods - 1; i < highs.Count(); i++)
            {
                double highestHigh = highs.Skip(i + 1 - kPeriods)
                        .Take(kPeriods)
                        .Max();

                double lowestLow = lows.Skip(i + 1 - kPeriods)
                        .Take(kPeriods)
                        .Min();

                double currentClose = closes.ElementAt(i);

                double k = (currentClose - lowestLow) / (highestHigh - lowestLow) * 100;
                outputKLine.Add(k);
            }

            if (outputDLine != null)
            {
                var dLineSMA = SMA(outputKLine, dPeriods);

                foreach (var item in dLineSMA.Values)
                {
                    outputDLine.Add(item);
                }
            }

            var result = new StochasticResult()
            {
                KLine = outputKLine,
                DLine = outputDLine,
                DStartIndexOffset = startDIndex,
                KStartIndexOffset = startKIndex
            };

            return result;
        }
    }
}
