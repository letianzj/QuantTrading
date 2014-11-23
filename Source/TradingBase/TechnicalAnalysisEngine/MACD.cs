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
        /// Calculates Moving Average Convergence-Divergence (MACD) indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="fastPeriod">Number of periods for fast moving average</param>
        /// <param name="slowPeriod">Number of periods for slow moving average</param>
        /// <param name="signalPeriod">Number of periods for signal line</param>
        /// <returns>Object containing operation results</returns>
        public static MACDResult MACD(IEnumerable<double> input, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            var MACD = new List<double>();

            int MACDoffsetIndex = slowPeriod - 1;
            int signalOffsetIndex = slowPeriod + signalPeriod - 2;

            var fastEMA = EMA(input, fastPeriod);
            var slowEMA = EMA(input, slowPeriod);

            fastEMA.Values.RemoveRange(0, slowPeriod - fastPeriod);

            for (int i = 0; i < fastEMA.Values.Count(); i++)
            {
                MACD.Add(fastEMA.Values[i] - slowEMA.Values[i]);
            }

            var signal = EMA(MACD, signalPeriod);

            var result = new MACDResult()
            {
                MACD = MACD,
                MACDStartIndexOffset = MACDoffsetIndex,
                Signal = signal.Values,
                SignalStartIndexOffset = signalOffsetIndex
            };

            return result;
        }


        /// <summary>
        /// Calculates Moving Average Convergence-Divergence (MACD) indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="fastPeriod">Number of periods for fast moving average</param>
        /// <param name="slowPeriod">Number of periods for slow moving average</param>
        /// <param name="signalPeriod">Number of periods for signal line</param>
        /// <param name="calculateHistogram">Determines whether histogram should be calculated</param>
        /// <returns>Object containing operation results</returns>
        public static MACDResult MACD(IEnumerable<double> inputValues, int fastPeriod, int slowPeriod, int signalPeriod, bool calculateHistogram)
        {
            var result = AnalysisEngine.MACD(inputValues, fastPeriod, slowPeriod, signalPeriod);
            var histogram = new List<double>();

            var macdCopy = result.MACD.ToList();

            macdCopy.RemoveRange(0, signalPeriod - 1);

            for (int i = 0; i < macdCopy.Count; i++)
            {
                histogram.Add(macdCopy[i] - macdCopy[i]);
            }

            result.HistogramStartIndexOffset = result.SignalStartIndexOffset;
            result.Histogram = histogram;

            return result;
        }

    }
}
