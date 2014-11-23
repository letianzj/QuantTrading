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
using System.Windows.Shapes;

namespace BackTestWindow
{
    /// <summary>
    /// Interaction logic for BacktestWindow.xaml
    /// </summary>
    public partial class BacktestWindow : Window
    {
        public BacktestWindow()
        {
            InitializeComponent();

            _viewmodel = new BacktestViewModel();
            DataContext = _viewmodel;

            this.dgFill.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.dgIndicator.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.dgOrder.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.dgPosition.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.dgResult.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.dgTick.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;
            this.dgOptimum.AutoGeneratingColumn += dataGrid_AutoGeneratingColumn;

            Closing += _viewmodel.OnWindowClosing;
        }

        BacktestViewModel _viewmodel;

        void dataGrid_AutoGeneratingColumn(object sender,
                                    DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }

        private void onesec_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(1);
        }

        private void onemin_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(60);
        }


        private void fivemin_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(300);
        }

        private void tenmin_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(600);
        }

        private void thirtymin_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(1800);
        }

        private void onehour_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(3600);
        }

        private void ptend_Click(object sender, EventArgs e)
        {
            _viewmodel.PlayTo(int.MaxValue);
        }
    }
}
