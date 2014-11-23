/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Reflection;

namespace TradingBase
{
    /// <summary>
    /// An optimize engine
    /// virtual functions from BacktestEngine: Reset(), Play(), UnbindEvents()
    /// </summary>
    public class OptimizeEngine : BacktestEngine
    {

        public OptimizeEngine(StrategyBase s, List<string> tickfiles) : base(s, tickfiles) { }

        // variable name; choose one to optimize
        public string OptimizeName = string.Empty;
        // the performance measure to be optimized
        public string OptimizeDecisionsName = string.Empty;
        public bool isHigherDecisionValuesBetter = true;

        public decimal StartAt = 0;
        public decimal StopAt = 0;
        public decimal Advance = 1;
        private decimal Current = 0;

        private int _id = -1;
        private bool _countup = true;
        public bool isNextAvail { get { return _countup ? Current + Advance < StopAt : Current + Advance > StopAt; } }
        public int OptimizeRemain { get { return OptimizeCount - _id - 1; } }
        public int OptimizeCount { get { return (int)(Math.Abs(StartAt - StopAt) / Advance); } }
        public decimal NextParam
        {
            get
            {
                if (_id < 0)
                {
                    Current = StartAt;
                    if (StartAt > StopAt)
                    {
                        _countup = false;
                        Advance = Math.Abs(Advance) * -1;
                    }
                    else
                        Advance = Math.Abs(Advance);
                }
                else
                    Current += Advance;
                _id++;
                return Current;
            }
        }
  
        // (parameter : obj) --> performanceresult
        // Access this upon play completion
        private Dictionary<string, PerformanceEvaluator> _resultsdict = new Dictionary<string, PerformanceEvaluator>();
        public Dictionary<string, PerformanceEvaluator> ResultsDict { get { return _resultsdict; } }

        public bool isValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(OptimizeName) && !string.IsNullOrWhiteSpace(OptimizeDecisionsName)
                    && _strategy != null
                    && (StartAt != StopAt) && (Advance < (Math.Abs(StartAt - StopAt)));
            }
        }

        /// <summary>
        /// Start the loop over selected parameter
        /// </summary>
        public void Start()
        {
            //ensure everything is ready to start the optimization
            if (!isValid)
            {
                Debug("Invalid optimization, Must configure this optimization completely before re-starting.");
                Status("Optimization not configured.");
                return;
            }

            if (_isbusy)
                return;

            Debug("Starting optimization, Queueing up all " + this.OptimizeCount + " combinations...");
            Debug("All combinations queued, Starting Optimizer Threads");

            _playthread.RunWorkerAsync();
            Status("Optimizaton started with " + OptimizeCount + " combinations.");
        }

        protected override void Play(object sender, DoWorkEventArgs e)
        {
            _isbusy = true;
            _resultsdict.Clear();
            while (isNextAvail)
            {
                decimal value = NextParam;
                int id = _id;

                if (ChangeValue(ref _strategy, value))
                {
                    //OptimizerEngine ge = new OptimizerEngine(s, hsim);
                    //Thread.SpinWait(1000);
                    //debugControl1.GotDebug("Starting a Optimizer instance");
                    Debug("Started Optimizer Engine: " + id);
                    Go();

                    List<Trade> trades = _simbroker.GetTradeList();
                    PerformanceEvaluator result = new PerformanceEvaluator();
                    //result.InitializePositions();
                    result.GenerateReports(trades);

                    decimal rv = 0;
                    GetResult(result, out rv);

                    _resultsdict.Add(value.ToString("N3") + " : " + rv.ToString("N3"), result);

                    Debug("optimize " + id + " finished.  Used: " + value + " -> " + OptimizeDecisionsName + ": " + rv + " trades: " + trades.Count + " SimsRemaining: " + OptimizeRemain);
                    progress(_id, OptimizeCount);
                }
                else
                    Debug("Unable to start Optimizer engine: " + id + " with: " + value);

            }
            Debug("All optimization runs complete, computing optimum...");
            // unbind
            UnbindEvents();
            _isbusy = false;
        }

        /// <summary>
        /// Do each loop
        /// </summary>
        private void Go()
        {
            _simbroker.Reset();
            if (_strategy != null)
            {
                _strategy.Reset();
                _strategy.IsActive = true;      // force true in case it was turned off by last run
            }
                
            if (_histsim != null)
            {
                _histsim.Reset();
                _histsim.SetTickFiles(_tickfiles.ToArray());
                _histsim.PlayTo(MultiSim.ENDSIM);
            }
            else
                Debug("No simulation defined on Optimizer engine.");
        }

        int lastpct = -1;
        void progress(int cur, int max)
        {
            int pct = (int)(((double)cur * 100) / max);
            progress(pct);
        }
        void progress(int pct)
        {
            if (pct <= lastpct)
                return;
            if (pct > 100)
                pct = 100;
            lastpct = pct;
            SendPlayProgress(pct);
        }

        #region reflection
        /// <summary>
        /// change parameter of a strategy
        /// </summary>
        bool ChangeValue(ref StrategyBase s, decimal v)
        {
            try
            {
                var t = s.GetType();
                foreach (var pi in t.GetProperties())
                    if (pi.Name == OptimizeName)
                    {
                        if (pi.PropertyType == typeof(int))
                        {
                            pi.SetValue(s, (int)v, null);
                        }
                        else
                            pi.SetValue(s, v, null);
                        return true;
                    }
            }
            catch (Exception ex)
            {
                Debug("error setting parameter " + OptimizeName + " on strategy: " + s.FullName + " to: " + v + " err: " + ex.Message + ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Get performance measure as optimization objective
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDecideable()
        {
            List<string> dr = new List<string>();
            // Type r = typeof(PerformanceMatrix);
            PerformanceMatrix r = new PerformanceMatrix();
            foreach (var pi in r.GetType().GetProperties())
            {
                if (!pi.GetAccessors()[0].IsPublic)
                    continue;
                var pt = pi.PropertyType;
                if ((pt == typeof(decimal)) || (pt == typeof(int)))
                    dr.Add(pi.Name);
            }
            foreach (var pi in r.GetType().GetFields())
            {
                if (!pi.IsPublic)
                    continue;
                var pt = pi.FieldType;
                if ((pt == typeof(decimal)) || (pt == typeof(int)))
                    dr.Add(pi.Name);
            }
            var tmp = dr.ToArray();
            Array.Sort(tmp);
            return new List<string>(tmp);
        }

        PropertyInfo _resultproperty = null;
        /// <summary>
        /// Get the optimized objective from a permformance result
        /// </summary>
        bool GetResult(PerformanceMatrix r, out decimal result)
        {
            result = 0;
            if (_resultproperty == null)
            {
                foreach (var pr in r.GetType().GetProperties())
                {
                    if (pr.Name == OptimizeDecisionsName)
                    {
                        _resultproperty = pr;
                        break;
                    }
                }
            }
            try
            {
                var o = _resultproperty.GetValue(r, null);
                result = (decimal)o;
                return true;
            }
            catch (Exception ex)
            {
                Debug("error getting result from OptimizeDecision name: " + OptimizeDecisionsName + " err: " + ex.Message + ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Get optimizable parameters of a strategy
        /// </summary>
        /// <returns></returns>
        public static List<string> GetOptimizeable(string dllname, string strategyname)
        {
            List<string> op = new List<string>();

            var s = Util.GetSingleStrategyFromDLL(strategyname, dllname);
            // ensure valid
            if (!s.IsActive)
                return op;
            // get all the public global properties
            foreach (var pi in s.GetType().GetProperties())
            {
                var pt = pi.PropertyType;
                if ((pt == typeof(int)) || (pt == typeof(decimal)))
                {
                    if (pi.CanWrite && pi.GetAccessors(false)[0].IsPublic)
                    {
                        op.Add(pi.Name);
                    }
                }
            }
            var tmp = op.ToArray();
            Array.Sort(tmp);
            return new List<string>(tmp);
        }

        public decimal GetMinAdvance(string name)
        {
            // ensure valid
            if ((_strategy == null) || (!_strategy.IsActive))
                return 0;
            // get all the public global properties
            foreach (var pi in _strategy.GetType().GetProperties())
            {
                if (pi.Name != name)
                    continue;
                var pt = pi.PropertyType;
                if (pt == typeof(int))
                    return 1;
                else return .01m;
            }
            return 0;
        }

        public bool isNameInt(string name)
        {
            // ensure valid
            if ((_strategy == null) || (!_strategy.IsActive))
                return false;
            // get all the public global properties
            foreach (var pi in _strategy.GetType().GetProperties())
            {
                if (pi.Name != name)
                    continue;
                var pt = pi.PropertyType;
                if (pt == typeof(int))
                    return true;
                return false;
            }
            return false;
        }

        public static object DeepClone(object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }
        #endregion
    }
}
