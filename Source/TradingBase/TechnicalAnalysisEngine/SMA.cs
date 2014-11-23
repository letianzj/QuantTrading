using System;
using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TechnicalAnalysisEngine;

namespace TechnicalAnalysisEngine
{
    public sealed partial class AnalysisEngine
    {
        /// <summary>
        /// Calculates Simple Moving Average (SMA) indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="periods">Number of periods</param>
        /// <param name="returnImmatureValues">Determines whether immature values should be taken into account</param>
        /// <returns>Object containing operation results</returns>
        public static SMAResult SMA(IEnumerable<double> input, int periods, bool returnImmatureValues)
        {
            var returnValues = new List<double>();

            if (returnImmatureValues)
            {
                for (int i = 0; i < input.Count(); i++)
                {
                    var resultValue = input
                        .Skip(i + 1 > periods ? i + 1 - periods : 0)
                        .Take(i >= periods ? periods : i + 1)
                        .Average();

                    returnValues.Add(resultValue);
                }
            }
            else
            {
                var copyInputValues = input.ToList();

                for (int i = 0; i < input.Count() - periods + 1; i++)
                {
                    var resultValue = copyInputValues.GetRange(i, periods).Average();

                    returnValues.Add(resultValue);
                }
            }

            var result = new SMAResult()
            {
                Values = returnValues,
                StartIndexOffset = periods - 1
            };

            return result;
        }

        /// <summary>
        /// Calculates Simple Moving Average (SMA) indicator
        /// </summary>
        /// <param name="input">Input signal</param>
        /// <param name="periods">Number of periods</param>
        /// <returns>Object containing operation results</returns>
        public static SMAResult SMA(IEnumerable<double> input, int periods)
        {
            return SMA(input, periods, false);
        }
    }
}

