using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnicalAnalysisEngine
{
    /// <summary>
    /// Contains calculation results for MFI indicator
    /// </summary>
    public class MFIResult
    {
        public List<double> Values { get; set; }

        /// <summary>
        /// Represents the index of input signal at which the indicator starts
        /// </summary>
        public int StartIndexOffset { get; set; }
    }
}
