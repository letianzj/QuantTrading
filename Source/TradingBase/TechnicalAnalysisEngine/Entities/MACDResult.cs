using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnicalAnalysisEngine
{
    /// <summary>
    /// Contains calculation results for MACD indicator
    /// </summary>
    public class MACDResult
    {
        public List<double> MACD { get; set; }
        public List<double> Signal { get; set; }
        public List<double> Histogram { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the MACD line starts
        /// </summary>
        public int MACDStartIndexOffset { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the signal starts
        /// </summary>
        public int SignalStartIndexOffset { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the histogram starts
        /// </summary>
        public int HistogramStartIndexOffset { get; set; }
    }
}
