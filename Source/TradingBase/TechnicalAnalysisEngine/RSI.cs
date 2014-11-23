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
        /// Calculates Relative Strength Index (RSI) indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="period">Number of periods</param>
        /// <returns>Object containing operation results</returns>
        public static RSIResult RSI(IEnumerable<double> input, int period)
        {
            double startGain = 0;
            double startLoss = 0;

            List<double> output = new List<double>();
            List<GainLoss> gainLossList = new List<GainLoss>();

            var inputList = input.ToList();
            for (int i = 0; i < period; i++)
            {
                if (i == 0)
                {
                    continue;
                }
                double gain = 0;
                double loss = 0;

                CalculateGainLoss(inputList[i], inputList[i - 1], out gain, out loss);

                startGain += gain;
                startLoss += loss;
            }

            startGain = startGain / period;
            startLoss = startLoss / period;

            gainLossList.Add(new GainLoss() { AverageGain = startGain, AverageLoss = startLoss });

            var skippedPeriodList = inputList.Skip(period).ToList();

            for (int i = 1; i < skippedPeriodList.Count; i++)
            {
                double currentGain = 0;
                double currentLoss = 0;

                CalculateGainLoss(skippedPeriodList[i], skippedPeriodList[i - 1], out currentGain, out currentLoss);

                double averageGain = (gainLossList[i - 1].AverageGain * (period - 1) + currentGain) / period;
                double averageLoss = (gainLossList[i - 1].AverageLoss * (period - 1) + currentLoss) / period;

                gainLossList.Add(new GainLoss() { AverageGain = averageGain, AverageLoss = averageLoss });
            }

            foreach (var item in gainLossList)
            {
                double result = 100 - 100 / (1 + item.AverageGain / item.AverageLoss);
                output.Add(result);
            }

            var rsiResult = new RSIResult()
            {
                StartIndexOffset = period - 1,
                Values = output
            };

            return rsiResult;
        }

        private class GainLoss
        {
            public double AverageGain { get; set; }
            public double AverageLoss { get; set; }
        }

        private static void CalculateGainLoss(double current, double previous, out double gain, out double loss)
        {
            double result = current - previous;

            if (result > 0)
            {
                gain = result;
                loss = 0;
            }
            else if (result < 0)
            {
                gain = 0;
                loss = Math.Abs(result);
            }
            else
            {
                gain = 0;
                loss = 0;
            }
        }
    }
}
