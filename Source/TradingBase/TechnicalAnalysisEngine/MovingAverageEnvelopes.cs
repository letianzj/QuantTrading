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
        /// Calculates Moving Average Envelopes indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="movingAverageType">Type of moving average to use</param>
        /// <param name="periods">Number of periods</param>
        /// <param name="percentage">Percentage for the envelopes (%)</param>
        /// <returns>Object containing operation results</returns>
        public static MovingAverageEnvelopesResult MovingAverageEnvelopes(IEnumerable<double> input, MovingAverageType movingAverageType, int periods, double percentage)
        {
            List<double> movingAverage = null;
            List<double> upperEnvelope = new List<double>();
            List<double> lowerEnvelope = new List<double>();

            int offset = 0;

            if (movingAverageType == MovingAverageType.Simple)
            {
                var sma = SMA(input, periods);

                movingAverage = sma.Values;
                offset = sma.StartIndexOffset;
            }
            else if (movingAverageType == MovingAverageType.Exponential)
            {
                var ema = EMA(input, periods);

                movingAverage = ema.Values;
                offset = ema.StartIndexOffset;
            }

            foreach (var point in movingAverage)
            {
                double upperEnvelopePoint = point + point * percentage / 100;
                double lowerEnvelopePoint = point - point * percentage / 100;

                upperEnvelope.Add(upperEnvelopePoint);
                lowerEnvelope.Add(lowerEnvelopePoint);
            }

            var result = new MovingAverageEnvelopesResult()
            {
                UpperEnvelope = upperEnvelope,
                MovingAverage = movingAverage,
                LowerEnvelope = lowerEnvelope,
                StartIndexOffset = offset
            };

            return result;
        }
    }
}
