using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnicalAnalysisEngine
{
    /// <summary>
    /// Contains calculation results for Stochastic indicator
    /// </summary>
    public class StochasticResult
    {
        public List<double> DLine { get; set; }
        public List<double> KLine { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the %D line starts
        /// </summary>
        public int DStartIndexOffset { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the %K line starts
        /// </summary>
        public int KStartIndexOffset { get; set; }
    }
}
