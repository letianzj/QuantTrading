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

using TradingBase;

namespace BackTestWindow.UI
{
    /// <summary>
    /// Select a specific strategy from dll
    /// Interaction logic for StrategySelectorWindow.xaml
    /// </summary>
    public partial class StrategySelectorWindow : Window
    {
        string _dll;
        List<string> _selectedstrategies = new List<string>();
        List<string> _allstrategiesindll = new List<string>();

        public StrategySelectorWindow(string dllname)
        {
            InitializeComponent();

            _dll = dllname;

            try
            {
                _allstrategiesindll =  Util.GetStrategyListFromDLL(_dll);
                lbStrategies.DataContext = _allstrategiesindll;
            }
            catch
            {
                throw new Exception("StrategyBase Selector errors.");
            }
        }

        public List<string> StrategySelected
        {
            get { return _selectedstrategies; }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            _selectedstrategies.Clear();

            foreach (object o in lbStrategies.SelectedItems)
            {
                _selectedstrategies.Add(o as string);
            }
            
            // some items are selected
            if (_selectedstrategies.Count != 0)
            {
                this.DialogResult = true;
                Close();
            }
                
        }
    }
}
