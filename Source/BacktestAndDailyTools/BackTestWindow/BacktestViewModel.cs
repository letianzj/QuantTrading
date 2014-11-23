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
using System.Windows.Input;
using System.Data;
using System.Collections.ObjectModel;
using BackTestWindow.Infrastructure;

namespace BackTestWindow
{
    class BacktestViewModel : ObservableObject
    {
        BacktestModel _model;

        public BacktestViewModel()
        {
            _model = new BacktestModel();
            _model.DebugDelegate += _model_DebugDelegate;
            _model.MainStatusDelegate += _model_StatusDelegate;
            _model.TickFileStatusDelegate += _model_TickFileStatusDelegate;
            _model.DllStatusDelegate += _model_DllStatusDelegate;

            _model.OptimizerStatusDelegate += _model_OptimizerStatusDelegate;
        }

        #region binding properties
        public DataTable TickTable { get { return _model.TickTable; } }
        public DataTable IndicatorTable { get { return _model.IndicatorTable; } }
        public DataTable PositionTable { get { return _model.PositionTable; } }
        public DataTable OrderTable { get { return _model.OrderTable; } }
        public DataTable FillTable { get { return _model.FillTable; } }
        public DataTable ResultTable { get { return _model.ResultTable; } }
        public DataTable OptimumResultsTable { get { return _model.OptimResultsTable; } }

        public string _mainstatus;
        public string _dllstatus;
        public string _tickfilestatus;
        public string MainStatus
        {
            get { return _mainstatus; }
            set
            {
                _mainstatus = value;
                RaisePropertyChanged("MainStatus");
            }
        }

        public string DLLStatus
        {
            get { return _dllstatus; }
            set
            {
                _dllstatus = value;
                RaisePropertyChanged("DLLStatus");
            }
        }

        public string TickFileStatus
        {
            get { return _tickfilestatus; }
            set
            {
                _tickfilestatus = value;
                RaisePropertyChanged("TickFileStatus");
            }
        }

        public decimal StartAt
        {
            get { return _model.StartAt; }
            set
            {
                _model.StartAt = value;
                //RaisePropertyChanged("StartAt");
            }
        }

        public decimal FinishAt
        {
            get { return _model.FinishAt; }
            set
            {
                _model.FinishAt = value;
                //RaisePropertyChanged("FinishAt");
            }
        }

        public decimal StepSize
        {
            get { return _model.StepSize; }
            set
            {
                _model.StepSize = value;
                //RaisePropertyChanged("StepSize");
            }
        }

        public List<string> OptimVariables
        {
            get { return _model.OptimizableList; }
        }

        public string SelectedVariable { get; set; }

        public List<string> OptimObjs
        {
            get { return _model.ObjList; }
        }

        public string SelectedObj { get; set; }

        public List<string> OptimumResultsList
        {
            get { return _model.OptimResultsList;}
        }

        void _model_OptimizerStatusDelegate(string obj)
        {
            switch(obj)
            {
                case "OptimVariable":
                    RaisePropertyChanged("OptimVariables");
                    break;
                case "OptimObj":
                    RaisePropertyChanged("OptimObjs");
                    break;
                case "OptimResults":
                    RaisePropertyChanged("OptimumResultsList");
                    break;
                default:
                    return;
            }
        }

        // select optimum result listbox
        int _selectedindex;
        public int SelectedOptimumResult
        {
            get { return _selectedindex; }
            set
            {
                if (_selectedindex == value)
                {
                    return;
                }
                // At this point _selectedIndex is the old selected item's index

                _selectedindex = value;

                // At this point _selectedIndex is the new selected item's index
                _model.FillOptimumResultTable(_selectedindex);
            }
        }

        private ObservableCollection<string> _messagelist = new ObservableCollection<string>();
        /// <summary>
        /// Message Listbox
        /// </summary>
        public ObservableCollection<string> MessageList
        {
            get
            {
                return _messagelist;
            }
        }
        #endregion

        #region commands
        void ReqHistData()
        {
            // _model.RequestHistoricalData();
        }

        bool CanReqHistData()
        {
            // historical data needs connection
            // return !_isconnected;
            return false;
        }

        void PlotHistData()
        {
            _model.PlotHistoricalData();
        }

        bool CanPlotHistData()
        {
            // historical data needs connection
            return true;
        }

        void LoadTickData()
        {
            _model.LoadTickData();
        }
        bool CanLoadTickData()
        {
            return true;
        }

        public ICommand LoadTickDataCmd { get { return new RelayCommand(LoadTickData, CanLoadTickData); } }

        void LoadStrategyDLL()
        {
            _model.LoadStrategy();
        }
        bool CanLoadStrategyDLL()
        {
            return true;
        }

        public ICommand LoadStrategyDLLCmd { get { return new RelayCommand(LoadStrategyDLL, CanLoadStrategyDLL); } }

        void ResetStrategy()
        {
            _model.Reset(true);
        }
        bool CanResetStrategy()
        {
            return true;
        }

        void ConvertHistData()
        {
        }
        bool CanConvertHistData()
        {
            return false;
        }

        public ICommand ReqHistDataCmd { get { return new RelayCommand(ReqHistData, CanReqHistData); } }
        public ICommand PlotHistDataCmd { get { return new RelayCommand(PlotHistData, CanPlotHistData); } }
        public ICommand ResetStrategyCmd { get { return new RelayCommand(ResetStrategy, CanResetStrategy); } }

        public ICommand ConvertHistDataCmd { get { return new RelayCommand(ConvertHistData, CanConvertHistData); } }
        
        void StartOptimization()
        {
            // get combobox values
            _model.SelectedObj = SelectedObj;
            _model.SelectedVariableToBeOptimized = SelectedVariable;

            _model.Optimize();
        }
        bool CanStartOptimization()
        {
            return !(string.IsNullOrWhiteSpace(SelectedObj) || string.IsNullOrWhiteSpace(SelectedVariable));
        }

        public ICommand StartOptimizationCmd { get { return new RelayCommand(StartOptimization, CanStartOptimization); } }

        public void PlayTo(int timedurationinseconds)
        {
            _model.PlayTo(timedurationinseconds);
        }
        #endregion

        #region events
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _model.SaveConfig();
        }

        void _model_DebugDelegate(string obj)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                _messagelist.Add(obj);
            });
        }

        void _model_StatusDelegate(string obj)
        {
            MainStatus = obj;
        }

        void _model_DllStatusDelegate(string obj)
        {
            DLLStatus = obj;
        }

        void _model_TickFileStatusDelegate(string obj)
        {
            TickFileStatus = obj;
        }
        #endregion
    }
}
