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

using System.Windows.Input;
using RDotNet;
using OxyPlot;
using OxyPlot.Series;

using BackTestWindow.Infrastructure;

namespace BackTestWindow.UI
{
    class PlotWindowViewModel : ObservableObject
    {
        private REngine _engine;
        CharacterVector _response;
        public PlotWindowViewModel(REngine engine)
        {
            _engine = engine;
            _response = _engine.Evaluate("require(quantmod)").AsCharacter();      // load quantmod
        }

        #region binding
        string _symbol = "GOOGL";
        public string Symbol { get { return _symbol; } set { _symbol = value; RaisePropertyChanged("Symbol"); } }
        string _fromdate = DateTime.Today.AddYears(-1).ToString("yyyy-MM-dd");
        public string FromDate { get { return _fromdate; } set { _fromdate = value; RaisePropertyChanged("FromDate"); } }
        string _todate = DateTime.Today.ToString("yyyy-MM-dd");
        public string ToDate { get { return _todate; } set { _todate = value; RaisePropertyChanged("ToDate"); } }
        PlotModel _myplot;
        public PlotModel MyPlot { get { return _myplot; } set { _myplot = value; RaisePropertyChanged("MyPlot"); } }
        #endregion

        #region command
        void LoadPlot()
        {
            PlotModel plot = new PlotModel()
            {
                LegendSymbolLength = 24.0,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.TopCenter,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black
            };

            var x_axis = new OxyPlot.Axes.DateTimeAxis { Position = OxyPlot.Axes.AxisPosition.Bottom, MajorStep=50, Title = "Date", StringFormat = "yyyyMMdd", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot };
            x_axis.TextColor = OxyColors.White; x_axis.TitleColor = OxyColors.White;
            plot.Axes.Add(x_axis);
            var y_axis = new OxyPlot.Axes.LinearAxis { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Price", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot };
            y_axis.TextColor = OxyColors.White; y_axis.TitleColor = OxyColors.White;
            plot.Axes.Add(y_axis);

            try
            {
                CharacterVector symbol = _engine.CreateCharacterVector(new[] {_symbol});
                CharacterVector from = _engine.CreateCharacterVector(new[] {_fromdate});
                CharacterVector to = _engine.CreateCharacterVector(new[] {_todate});
                _engine.SetSymbol("symbol",symbol);
                _engine.SetSymbol("from",from);
                _engine.SetSymbol("to", to);
                _engine.Evaluate("getSymbols(symbol, from=from, to=to)");

                NumericMatrix bars = _engine.GetSymbol(_symbol).AsNumericMatrix();
                // 1970-01-01 = 0
                DateTime rootdate = new DateTime(1970, 1, 1);
                IntegerVector dates = _engine.Evaluate("index(" + _symbol + ")").AsInteger();

                CandleStickSeries candleStickSeries = new CandleStickSeries
                {
                    Title = _symbol,
                    CandleWidth = 5,
                    Color = OxyColors.DarkGray,
                    IncreasingFill = OxyColors.DarkGreen,
                    DecreasingFill = OxyColors.Red,
                    TrackerFormatString = "Date: {1:yyyyMMdd}\nHigh: {2:0.00}\nLow: {3:0.00}\nOpen: {4:0.00}\nClose: {5:0.00}"
                };

                // add to bars
                for (int i = 0; i < dates.Count(); i++)
                {
                    DateTime d = rootdate.AddDays(dates[i]);
                    int dint = TradingBase.Util.ToIntDate(d);
                    candleStickSeries.Items.Add(new HighLowItem(OxyPlot.Axes.DateTimeAxis.ToDouble(d), bars[i, 1], bars[i, 2], bars[i, 0], bars[i, 3]));
                }

                plot.Series.Add(candleStickSeries);
                MyPlot = plot;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        bool CanLoadPlot()
        {
            return !string.IsNullOrEmpty(_symbol);
        }

        public ICommand LoadPlotCmd { get { return new RelayCommand(LoadPlot, CanLoadPlot); } }
        #endregion
    }
}
