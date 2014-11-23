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

using Modules.Framework.Interfaces;
using Modules.RealTimeQuotePresentation.Model;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.ServiceLocation;
using Modules.RealTimeQuotePresentation.Services;
using OxyPlot;
using OxyPlot.Series;
using TradingBase;

namespace Modules.RealTimeQuotePresentation.ViewModels
{
    public class QuotePlotViewModel : BindableBase
    {
        private PlotModel _tickplot;
        private DateTime _plotxaxisleft, _plotxaxisright;
        private QuoteUpdateService _quoteupdateservice;
        private IConfigManager _configmanager;

        private string _currentsymbol = "";
        private Dictionary<string, LineSeries> _tickseriesdict = new Dictionary<string, LineSeries>();

        public QuotePlotViewModel()
        {
            this._quoteupdateservice = ServiceLocator.Current.GetInstance<IQuoteUpdateService>() as QuoteUpdateService;
            _quoteupdateservice.PlotViewModel = this;
            this._configmanager = ServiceLocator.Current.GetInstance<IConfigManager>();
            

            // initialize tickplot model
            _tickplot = new PlotModel()
            {
                PlotMargins = new OxyThickness(50, 0, 0, 40),
                Background = OxyColors.Transparent,
                //LegendTitle = "Legend",
                LegendTextColor = OxyColors.White,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopRight,
                //LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black
            };

            if (_configmanager.PlotRegularTradingHours)
            {
                _plotxaxisleft = DateTime.Today.Add(new TimeSpan(9, 15, 0));
                _plotxaxisright = DateTime.Today.Add(new TimeSpan(16, 15, 0));
            }
            else
            {
                _plotxaxisleft = DateTime.Today;
                _plotxaxisright = DateTime.Today.AddDays(1);
            }

            var dateAxis = new OxyPlot.Axes.DateTimeAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time", StringFormat = "HH:mm:ss", Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(_plotxaxisleft), Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(_plotxaxisright), MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
            dateAxis.TextColor = OxyColors.White; dateAxis.TitleColor = OxyColors.White;
            _tickplot.Axes.Add(dateAxis);
            var priceAxis = new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Price", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot };
            priceAxis.TextColor = OxyColors.White; priceAxis.TitleColor = OxyColors.White;
            _tickplot.Axes.Add(priceAxis);
        }

        public PlotModel TickPlotModel { get { return _tickplot; } }

        /// <summary>
        /// // add lineseries to dictionary
        /// </summary>
        public void SetLineSeries(string[] basket)
        {
            // add lineseries to dictionary
            for (int i = 0; i < basket.Count(); i++)
            {
                LineSeries tickseries = new LineSeries
                {
                    StrokeThickness = 2,
                    MarkerSize = 3,
                    MarkerStroke = OxyColor.FromRgb(0xA8, 0xD6, 0xFF),
                    MarkerType = MarkerType.None,
                    CanTrackerInterpolatePoints = false,
                    Title = basket[i],
                    Smooth = false
                };
                _tickseriesdict.Add(basket[i], tickseries);
            }
        }

        public void GotTick(Tick k)
        {
            if (_configmanager.RealTimePlot)
            {
                if (_tickseriesdict.ContainsKey(k.FullSymbol) && k.IsTrade)
                {
                    try
                    {
                        _tickseriesdict[k.FullSymbol].Points.Add(new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(DateTime.Now), (double)k.TradePrice));
                    }
                    catch (Exception) {}
                }

            }
        }

        public void UpdatePlotArea(string symbol)
        {
            if (_configmanager.RealTimePlot && _tickseriesdict.ContainsKey(symbol))
            {
                if (_currentsymbol != symbol)
                {
                    //string symbol = _tickseriesdict.Keys.ElementAt(selectedindex);
                    _tickplot.Axes.Clear();
                    var dateAxis = new OxyPlot.Axes.DateTimeAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Time", StringFormat = "HH:mm:ss", Minimum = OxyPlot.Axes.DateTimeAxis.ToDouble(_plotxaxisleft), Maximum = OxyPlot.Axes.DateTimeAxis.ToDouble(_plotxaxisright), MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, IntervalLength = 80 };
                    dateAxis.TextColor = OxyColors.White; dateAxis.TitleColor = OxyColors.White;
                    _tickplot.Axes.Add(dateAxis);
                    var priceAxis = new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Price", MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot };
                    priceAxis.TextColor = OxyColors.White; priceAxis.TitleColor = OxyColors.White;
                    _tickplot.Axes.Add(priceAxis);

                    _tickplot.Series.Clear();
                    _tickplot.Series.Add(_tickseriesdict[symbol]);
                    _currentsymbol = symbol;
                }
            }
        }
    }
}
