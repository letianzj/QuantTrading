using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnicalAnalysisEngine
{
    /// <summary>
    /// Contains calculation results for Moving Average Envelopes indicator
    /// </summary>
    public class MovingAverageEnvelopesResult
    {
        public List<double> MovingAverage { get; set; }
        public List<double> UpperEnvelope { get; set; }
        public List<double> LowerEnvelope { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the indicator starts
        /// </summary>
        public int StartIndexOffset { get; set; }
    }
}
