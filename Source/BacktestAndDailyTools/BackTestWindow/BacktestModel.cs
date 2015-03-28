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

using System.ComponentModel;
using System.Data;
using System.IO;

using TradingBase;
using RDotNet;

namespace BackTestWindow
{
    class BacktestModel
    {
        string _rootpath = Util.GetRootPath();
        private ConfigManager _config;
        bool _showticks;
        string _dps = "N2";
        bool _ussebidaskfill = false;
        bool _adjustincomingticksize = false;

        // Only one strategy, doesn't support multiple strategies
        private IdTracker _idtracker = new IdTracker(0);
        private StrategyBase _strategy;
        private string _strategyname = "";
        string _dllname = "";
        List<string> _tickfiles = new List<string>();

        // run backtest
        // historical tick date
        protected int _date = 0;
        // historical tick time
        protected int _time = 0;

        private REngine _rengine;       // somehow rengine can't be re-initialized afte dispose

        #region BacktestEngine params
        // backtest engine or optimization engine
        BacktestEngine _backtestengine;
        OptimizeEngine _optimengine;
        bool _createnewengine = true;
        PerformanceEvaluator _performevaluator;
        Dictionary<string, Position> _positionlist = new Dictionary<string, Position>();
        List<Trade> _tradelist = new List<Trade>();
        #endregion

        #region OptimEngine params
        public decimal StartAt { get; set; }
        public decimal FinishAt { get; set; }
        public decimal StepSize { get; set; }

        List<string> _objlist = OptimizeEngine.GetDecideable();
        public List<string> ObjList { get { return _objlist; } }
        public string SelectedObj {get; set;}

        List<string> _optimvariablelist;
        public List<string> OptimizableList {get {return _optimvariablelist;}}
        public string SelectedVariableToBeOptimized { get; set; }

        public List<string> OptimResultsList { get; set; }
        private Dictionary<string, PerformanceEvaluator> _optimresultsdict;
        #endregion

        #region datatables
        DataTable _ticktable = new DataTable("TickTable");
        DataTable _indicatortable = new DataTable("IndicatorTable");
        DataTable _positiontable = new DataTable("PositionTable");
        DataTable _ordertable = new DataTable("OrderTable");
        DataTable _filltable = new DataTable("FillTable");
        DataTable _resultstable = new DataTable("ResultsTable");
        DataTable _optimresultstable = new DataTable("OptimResultsTable");

        public DataTable TickTable { get { return _ticktable; } }
        public DataTable IndicatorTable { get { return _indicatortable; } }
        public DataTable PositionTable { get { return _positiontable; } }
        public DataTable OrderTable { get { return _ordertable; } }
        public DataTable FillTable { get { return _filltable; } }
        public DataTable ResultTable { get { return _resultstable; } }
        public DataTable OptimResultsTable { get { return _optimresultstable; } }

        private void initdatatables()
        {
            _ticktable.Columns.Add("Date");
            _ticktable.Columns.Add("Time");
            _ticktable.Columns.Add("Symbol");
            _ticktable.Columns.Add("Trade");
            _ticktable.Columns.Add("TSize");
            _ticktable.Columns.Add("BSize");
            _ticktable.Columns.Add("Bid");
            _ticktable.Columns.Add("Ask");
            _ticktable.Columns.Add("ASize");
            
            // indicator table
            _indicatortable.Columns.Add("SID");
            _indicatortable.Columns.Add("SName");
            _indicatortable.Columns.Add("Indicators");

            _positiontable.Columns.Add("Time");
            _positiontable.Columns.Add("Symbol");
            _positiontable.Columns.Add("Size");
            _positiontable.Columns.Add("AvgPrice");
            _positiontable.Columns.Add("Profit");
            _positiontable.Columns.Add("Points");

            _ordertable.Columns.Add("Time");
            _ordertable.Columns.Add("Symbol");
            _ordertable.Columns.Add("Size");
            _ordertable.Columns.Add("Type");
            _ordertable.Columns.Add("Price");
            _ordertable.Columns.Add("Id");

            _filltable.Columns.Add("FillTime");
            _filltable.Columns.Add("Symbol");
            _filltable.Columns.Add("FillSize");
            _filltable.Columns.Add("FillPrice");
            _filltable.Columns.Add("Id");

            _resultstable.Columns.Add("Statistics");
            _resultstable.Columns.Add("Result");

            _optimresultstable.Columns.Add("Statistics");
            _optimresultstable.Columns.Add("Result");
        }

        private void InitIndicatorTable()
        {

        }
        #endregion

        public BacktestModel()
        {
            LoadConfig();
            _dps = "N" + _config.DecimalPlace;
            _ussebidaskfill = _config.UseBidAskFill;
            _adjustincomingticksize = _config.AdjustIncomingTick;
            _showticks = _config.ShowTicks;

            initdatatables();

            REngine.SetEnvironmentVariables();
            // There are several options to initialize the engine, but by default the following suffice:
            _rengine = REngine.GetInstance();
        }

        void Reset() { Reset(true); }
        public void Reset(bool reloadstrategy)
        {
            try
            {
                
                _tradelist.Clear();
                _positionlist.Clear();

                if (_createnewengine == false)     // already have an egine
                {
                    if (_backtestengine != null)        // could be optimize engine
                    {
                        try
                        {
                            _backtestengine.Stop();
                            _backtestengine.Reset();
                        }
                        catch { }
                    }  
                }

                _ticktable.Clear();
                _indicatortable.Clear();
                _positiontable.Clear();
                _ordertable.Clear();
                _filltable.Clear();
                _resultstable.Clear();

                if (reloadstrategy)
                {
                    _strategy = null;
                    _strategyname = "";
                    _dllname = "";
                    DllStatus("");

                    if (_createnewengine == false)     // already have an egine
                        UnbindBacktestEngine(ref _backtestengine);
                    _createnewengine = true;
                }
            }
            catch (Exception ex)
            {
                Status("An error occured, try again.");
                Debug("reset error: " + ex.Message + ex.StackTrace);
            }
        }

        void CreateNewEngine(string type)
        {
            // CheckPrerequisite() is alrready done at playto
            switch (type)
            {
                case "Backtest":
                    {
                        // Init inidcator datatable
                        InitIndicatorTable();
                        _backtestengine = new BacktestEngine(_strategy, _tickfiles);
                        BindBacktestEngine(ref _backtestengine);
                        break;
                    }
                case "Optimize":
                    {
                        Reset(false);
                        _optimengine = new OptimizeEngine(_strategy, _tickfiles);
                        break;
                    }
                default:
                    return;
            }
        }

        bool CheckPrerequisite()
        {
            if ((_backtestengine != null) && (_backtestengine.IsBusy)) { Status("Still playing, please wait..."); return false; }
            if (_tickfiles.Count == 0) { Status("Add study data."); return false; }
            if ((_strategyname == "") || (_dllname == "")) { Status("Add strategy."); return false; }

            Status("Click on desired play duration to begin.");
            return true;
        }

        #region File IO
        public void SaveConfig()
        {
            //ConfigManager.Serialize(_config, _rootpath+_config.SettingPath + "mainsettings.xml");
        }

        public void LoadConfig()
        {
            _config = ConfigManager.Deserialize(_rootpath + "\\Config\\mainsettings.xml");
        }

        public bool LoadStrategy()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            openFileDialog1.InitialDirectory = _rootpath+_config.StrategyPath;
            openFileDialog1.Filter = "dll files (*.dll)|*.dll|All files (*.*)|*.*";
            openFileDialog1.Title = "Select strategy dll";
            openFileDialog1.RestoreDirectory = true;

            Nullable<bool> result = openFileDialog1.ShowDialog();

            if (result == true)
            {
                string f = openFileDialog1.FileName;

                if (Util.isStrategyFile(f))
                {
                    // Select specifc strategy in the dll
                    BackTestWindow.UI.StrategySelectorWindow strategyselector = new BackTestWindow.UI.StrategySelectorWindow(f);
                    Nullable<bool> sresult = strategyselector.ShowDialog();

                    if (sresult == true)
                    {
                        Reset();

                        // load first strategy, ignore multi-selection
                        _dllname = f;
                        _strategyname = strategyselector.StrategySelected[0];

                        _strategy = Util.GetSingleStrategyFromDLL(_strategyname, _dllname);
                        _strategy.Reset();
                        _strategy.SetIdTracker(_idtracker);

                        // set strategy parameters
                        BackTestWindow.UI.StrategySetterWindow strategysetter = new BackTestWindow.UI.StrategySetterWindow(_strategy);
                        sresult = strategysetter.ShowDialog();      // ignore sresult. User can assume default parameters.

                        // status
                        DllStatus(_strategyname);

                        // set up optimizable variable list
                        _optimvariablelist = OptimizeEngine.GetOptimizeable(_dllname, _strategyname);
                        OptimizerStatusUpdates("OptimVariable");
                    }
                }
            }

            bool success = CheckPrerequisite();
            return success;
        }

        public bool LoadTickData()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();

            openFileDialog1.InitialDirectory = _rootpath+_config.TickPath;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.Title = "Select tick data files";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;

            Nullable<bool> result = openFileDialog1.ShowDialog();
            if (result == true)
            {
                // clear current epffiles
                _tickfiles.Clear();

                string[] fs = openFileDialog1.FileNames;

                foreach (string f in fs)
                {
                    if (Util.isTickFile(f))
                        _tickfiles.Add(f);
                }
            }

            // setup tickdata status as the name of last tick file
            if (_tickfiles.Count > 0)
            {
                string lastf = Path.GetFileNameWithoutExtension(_tickfiles[_tickfiles.Count - 1]);
                TickFileStatus(lastf);
            }

            bool success = CheckPrerequisite();
            return success;
        }
        #endregion

        #region Historical Simulation
        public void PlayTo(int timespaninseconds)
        {
            if (!CheckPrerequisite())
                return;

            if (_createnewengine)
            {
                // Create new engine and bind it
                CreateNewEngine("Backtest");
                _backtestengine.UseBidAskFill = _ussebidaskfill;
                _backtestengine.AdjustIncomingTickSize = _adjustincomingticksize;
                _createnewengine = false;
            }

            _backtestengine.Start(timespaninseconds);
        }

        public void Optimize()
        {
            if (!CheckPrerequisite())
                return;

            // create an optimization engine
            CreateNewEngine("Optimize");
            _optimengine.UseBidAskFill = _ussebidaskfill;
            _optimengine.AdjustIncomingTickSize = _adjustincomingticksize;

            // bind
            _optimengine.SendEngineDebugEvent += _backtestengine_SendEngineDebugEvent;
            _optimengine.SendEngineStatusEvent += _backtestengine_SendEngineStatusEvent;
            _optimengine.SendPlayProgressEvent += _optimengine_SendPlayProgressEvent;
            _optimengine.SendPlayCompleteEvent += _optimengine_SendPlayCompleteEvent;

            // reset to do optimization loop
            _optimengine.StartAt = StartAt;
            _optimengine.StopAt = FinishAt;
            _optimengine.Advance = StepSize;

            _optimengine.OptimizeName = SelectedVariableToBeOptimized;
            _optimengine.OptimizeDecisionsName = SelectedObj;
            _optimengine.isHigherDecisionValuesBetter = true;

            _optimengine.Start();
        }

        void BindBacktestEngine(ref BacktestEngine _engine)
        {
            _engine.SendEngineDebugEvent += _backtestengine_SendEngineDebugEvent;
            _engine.SendEngineStatusEvent += _backtestengine_SendEngineStatusEvent;
            _engine.GotFillEvent += _backtestengine_GotFillEvent;
            _engine.GotIndicatorEvent += _backtestengine_GotIndicatorEvent;
            _engine.GotOrderEvent += _backtestengine_GotOrderEvent;
            _engine.GotTickEvent += _backtestengine_GotTickEvent;

            _engine.SendPlayCompleteEvent += _backtestengine_SendPlayCompleteEvent;
            _engine.SendPlayProgressEvent += _backtestengine_SendPlayProgressEvent;
        }

        void UnbindBacktestEngine(ref BacktestEngine _engine)
        {
            try
            {
                _engine.SendEngineDebugEvent -= _backtestengine_SendEngineDebugEvent;
                _engine.SendEngineStatusEvent -= _backtestengine_SendEngineStatusEvent;
                _engine.GotFillEvent -= _backtestengine_GotFillEvent;
                _engine.GotIndicatorEvent -= _backtestengine_GotIndicatorEvent;
                _engine.GotOrderEvent -= _backtestengine_GotOrderEvent;
                _engine.GotTickEvent -= _backtestengine_GotTickEvent;

                _engine.SendPlayCompleteEvent -= _backtestengine_SendPlayCompleteEvent;
                _engine.SendPlayProgressEvent -= _backtestengine_SendPlayProgressEvent;
            }
            catch { }
        }

        void _backtestengine_SendEngineDebugEvent(string obj)
        {
            Debug(obj);
        }

        void _backtestengine_SendEngineStatusEvent(string obj)
        {
            Status(obj);
        }

        void _backtestengine_GotFillEvent(Trade t)
        {
            _tradelist.Add(t);

            Position mypos = new Position(t);
            decimal closepnl = 0;
            decimal closepoint = 0;
            if (!_positionlist.TryGetValue(t.FullSymbol, out mypos))
            {
                mypos = new Position(t);
                _positionlist.Add(t.FullSymbol, mypos);
            }
            else
            {
                closepoint = Calc.ClosePT(mypos, t);
                closepnl = mypos.Adjust(t);
                _positionlist[t.FullSymbol] = mypos;
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _positiontable.Rows.Add(t.TradeTime.ToString(), mypos.FullSymbol, mypos.Size, mypos.AvgPrice.ToString(_dps), closepnl.ToString("C2"), closepoint.ToString(_dps));
                _filltable.Rows.Add(t.TradeTime.ToString(), t.FullSymbol, t.TradeSize, t.TradePrice.ToString(_dps), t.Id);
            });
        }

        void _backtestengine_GotTickEvent(Tick t)
        {
            if (_showticks)
            {
                _date = t.Date;
                _time = t.Time;

                // don't display ticks for unmatched exchanges
                string trade = "";
                string bid = "";
                string ask = "";
                string ts = "";
                string bs = "";
                string os = "";

                if (t.IsIndex)
                {
                    trade = t.TradePrice.ToString(_dps);
                }
                else if (t.IsTrade)
                {
                    trade = t.TradePrice.ToString(_dps);
                    ts = t.TradeSize.ToString();
                }
                if (t.HasBid)
                {
                    bs = t.BidSize.ToString();
                    bid = t.BidSize.ToString(_dps);
                }
                if (t.HasAsk)
                {
                    ask = t.AskPrice.ToString(_dps);
                    os = t.AskSize.ToString();
                }

                // add tick to grid
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    _ticktable.Rows.Add(new string[] { t.Date.ToString(), _time.ToString(), t.FullSymbol, trade, ts, bs, bid, ask, os });
                });
                
            }
        }

        void _backtestengine_GotOrderEvent(Order o)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _ordertable.Rows.Add(o.OrderTime, o.FullSymbol, o.OrderSize, (o.IsMarket ? "Mkt" : (o.IsLimit ? "Lmt" : "Stp")), o.IsStop ? o.StopPrice : (o.IsTrail ? o.TrailPrice : 0.0m), o.Id);
            });
        }

        void _backtestengine_GotIndicatorEvent(string param)
        {
            string[] msg = param.Split(';');
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _indicatortable.Rows.Add(msg[0],msg[1],msg[2]);
            });
        }

        void _backtestengine_SendPlayProgressEvent(int obj)
        {
            
        }

        void _optimengine_SendPlayProgressEvent(int obj)
        {

        }

        void _backtestengine_SendPlayCompleteEvent(int obj)
        {
            _performevaluator = new PerformanceEvaluator();
            //_performevaluator.InitializePositions();
            _performevaluator.GenerateReports(_tradelist);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _performevaluator.FillGrid(_resultstable);
            });
        }

        void _optimengine_SendPlayCompleteEvent(int obj)
        {
            // get results
            _optimresultsdict = _optimengine.ResultsDict;

            // update listbox
            OptimResultsList = _optimresultsdict.Select(x => x.Key).ToList();
            OptimizerStatusUpdates("OptimResults");

            // unbind
            _optimengine.SendEngineDebugEvent -= _backtestengine_SendEngineDebugEvent;
            _optimengine.SendEngineStatusEvent -= _backtestengine_SendEngineStatusEvent;
            _optimengine.SendPlayProgressEvent -= _optimengine_SendPlayProgressEvent;
            _optimengine.SendPlayCompleteEvent -= _optimengine_SendPlayCompleteEvent;

            _optimengine = null;
            _createnewengine = true;        // New backtest/optimize engine are available
        }

        // update datatable
        public void FillOptimumResultTable(int selectedindex)
        {  
            _optimresultstable.Clear();
            string key = OptimResultsList[selectedindex];

            _optimresultsdict[key].FillGrid(_optimresultstable);
        }
        #endregion

        #region outgoing delegates
        public event Action<string> DebugDelegate;
        void Debug(string s)
        {
            if (DebugDelegate != null)
                DebugDelegate(s);
        }

        public event Action<string> MainStatusDelegate;
        void Status(string s)
        {
            if (MainStatusDelegate != null)
                MainStatusDelegate(s);
        }
        public event Action<string> DllStatusDelegate;
        void DllStatus(string s)
        {
            if (DllStatusDelegate != null)
                DllStatusDelegate(s);
        }
        public event Action<string> TickFileStatusDelegate;
        void TickFileStatus(string s)
        {
            if (TickFileStatusDelegate != null)
                TickFileStatusDelegate(s);
        }

        public event Action<string> OptimizerStatusDelegate;
        void OptimizerStatusUpdates(string s)
        {
            if (OptimizerStatusDelegate != null)
                OptimizerStatusDelegate(s);
        }
        #endregion

        #region other
        public void PlotHistoricalData()
        {
            BackTestWindow.UI.PlotWindow plotwindow = new BackTestWindow.UI.PlotWindow(_rengine);
            plotwindow.Show();
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    // you should always dispose of the REngine properly.
                    // After disposing of the engine, you cannot reinjitialize nor reuse it
                    _rengine.Close();
                    _rengine.Dispose();
                    _rengine = null;
                }
                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;
            }
        }

        ~BacktestModel()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
