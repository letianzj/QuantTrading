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

using Modules.RealTimeQuotePresentation.ViewModels;
using Modules.RealTimeQuotePresentation.Services;
using Modules.Framework.Interfaces;

namespace Modules.RealTimeQuotePresentation.Views
{
    /// <summary>
    /// Interaction logic for QuoteGridView.xaml
    /// </summary>
    public partial class QuoteGridView : UserControl
    {
        QuoteGridViewModel _viewmodel = new QuoteGridViewModel();

        public QuoteGridView()
        {
            InitializeComponent();
            this.DataContext = _viewmodel;

            this.QuoteGrid.Loaded += QuoteGrid_Loaded;
        }

        /// <summary>
        /// Spread datagridcolumns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void QuoteGrid_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in this.QuoteGrid.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }
    }
}
