using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechnicalAnalysisEngine;

namespace TechnicalAnalysisEngine
{
    /// <summary>
    /// Provides methods for technical analysis
    /// </summary>
    public sealed partial class AnalysisEngine
    {
        /// <summary>
        /// Calculates Bollinger Bands indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="periods">Number of periods</param>
        /// <param name="standardDeviations">Number of standard deviations</param>
        /// <param name="calculateBandwidth">Determines whether the bandwidth line should be calculated</param>
        /// <param name="calculatePercentB">Determines whether the percent B line should be calculated</param>
        /// <returns>Object containing operation results</returns>
        public static BollingerBandsResult BollingerBands(IEnumerable<double> input, int periods, int standardDeviations, bool calculateBandwidth, bool calculatePercentB)
        {
            List<double> upperBand = new List<double>();
            List<double> lowerBand = new List<double>();
            List<double> middleBand = null;
            List<double> bandwidth = null;
            List<double> percentB = null;

            if (calculateBandwidth)
            {
                bandwidth = new List<double>();
            }

            if (calculatePercentB)
            {
                percentB = new List<double>();
            }

            var middleBandSMA = SMA(input, periods);
            middleBand = middleBandSMA.Values;
            
            var stdevList = new List<double>();
            var inputHelperList = input.ToList();

            for (int i = 0; i < input.Count() - periods + 1; i++)
            {
                stdevList.Add(Helpers.StandardDeviation(inputHelperList.GetRange(i, periods)));
            }

            for (int i = 0; i < middleBand.Count; i++)
            {
                var middleValue = middleBand.ElementAt(i);
                var stdev = stdevList.ElementAt(i);

                var upperBandValue = middleValue + stdev * standardDeviations;
                var lowerBandValue = middleValue - stdev * standardDeviations;
                upperBand.Add(upperBandValue);
                lowerBand.Add(lowerBandValue);

                if (bandwidth != null)
                {
                    bandwidth.Add(upperBandValue - lowerBandValue);
                }

                if (percentB != null)
                {
                    var price = input.ElementAt(i + periods - 1);
                    percentB.Add((price - lowerBandValue) / (upperBandValue - lowerBandValue));
                }
            }

            var result = new BollingerBandsResult()
            {
                Bandwidth = bandwidth,
                LowerBand = lowerBand,
                MiddleBand = middleBand,
                PercentB = percentB,
                StartIndexOffset = periods - 1,
                UpperBand = upperBand
            };

            return result;
        }

        /// <summary>
        /// Calculates Bollinger Bands indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="periods">Number of periods</param>
        /// <param name="standardDeviations">Number of standard deviations</param>
        /// <returns>Object containing operation results</returns>
        public static BollingerBandsResult BollingerBands(IEnumerable<double> input, int periods, int standardDeviations)
        {
            return BollingerBands(input, periods, standardDeviations, false, false);
        }
    }
}
