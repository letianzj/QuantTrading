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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Modules.RealTimeQuotePresentation.ViewModels;
using Modules.Framework.Events;

namespace Modules.RealTimeQuotePresentation.Views
{
    /// <summary>
    /// Interaction logic for QuotePlotView.xaml
    /// </summary>
    public partial class QuotePlotView : UserControl, IDisposable
    {
        QuotePlotViewModel _viewmodel = new QuotePlotViewModel();
        EventAggregator _eventaggregator = ServiceLocator.Current.GetInstance<EventAggregator>();
        public QuotePlotView()
        {
            InitializeComponent();
            this.DataContext = _viewmodel;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            _eventaggregator.GetEvent<SymbolGridSelectionChangedEvent>().Subscribe(QuoteRowChangedHandler, ThreadOption.UIThread);

            stopwatch.Start();
        }

        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private long lastUpdateMilliSeconds;

        /// <summary>
        /// Avoid too many real time updates
        /// http://blog.bartdemeyer.be/2013/03/creating-graphs-in-wpf-using-oxyplot/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds > lastUpdateMilliSeconds + 5000)
            {
                //_viewmodel.UpdatePlotArea();
                TickPlotArea.InvalidateFlag = TickPlotArea.InvalidateFlag + 1;     // change invalidateflag value
                lastUpdateMilliSeconds = stopwatch.ElapsedMilliseconds;
            }
        }

        private void QuoteRowChangedHandler(string fullsymbol)
        {
            _viewmodel.UpdatePlotArea(fullsymbol);
            TickPlotArea.InvalidateFlag = TickPlotArea.InvalidateFlag + 1;     // change invalidateflag value
            lastUpdateMilliSeconds = stopwatch.ElapsedMilliseconds;
        }

        public void Dispose()
        {
            stopwatch.Stop();
        }
    }
}
