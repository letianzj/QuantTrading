using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnicalAnalysisEngine
{
    /// <summary>
    /// Contains calculation results for Bollinger Bands indicator
    /// </summary>
    public class BollingerBandsResult
    {
        public List<double> UpperBand { get; set; }
        public List<double> MiddleBand { get; set; }
        public List<double> LowerBand { get; set; }
        public List<double> Bandwidth { get; set; }
        public List<double> PercentB { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the indicator starts
        /// </summary>
        public int StartIndexOffset { get; set; }
    }
}
