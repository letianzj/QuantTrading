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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Modules.RealTimeQuotePresentation.Model
{
    public class QuoteGridLine : INotifyPropertyChanged
    {
        private string _symbol;
        private int _bidsize;
        private int _asksize;
        private int _size;
        private QuotePair _bidpair;
        private QuotePair _askpair;
        private QuotePair _tradepair;
        private decimal _preclose;
        private decimal _change;
        private string _changepercentage;


        public QuoteGridLine(string sym, int bs, decimal b, decimal o, int os, int ts, decimal pc, decimal c, decimal cp)
        {
            Symbol = sym;
            BidSize = bs;
            AskSize = os;
            Size = ts;
            PreClose = pc;
            Change = c;
            ChangePercentage = String.Format("{0:P3}.", cp);
            _bidpair = new QuotePair(0, 0);
            _askpair = new QuotePair(0, 0);
            _tradepair = new QuotePair(0, 0);
        }
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                _symbol = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Symbol");
            }
        }

        public int BidSize
        {
            get { return _bidsize; }
            set
            {
                _bidsize = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("BidSize");
            }
        }

        public QuotePair BidPair
        {
            get { return _bidpair; }
            set
            {
                _bidpair = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("BidPair");
            }
        }

        public QuotePair AskPair
        {
            get { return _askpair; }
            set
            {
                _askpair = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AskPair");
            }
        }

        public int AskSize
        {
            get { return _asksize; }
            set
            {
                _asksize = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AskSize");
            }
        }

        public QuotePair TradePair
        {
            get { return _tradepair; }
            set
            {
                _tradepair = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TradePair");
            }
        }

        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Size");
            }
        }
        public decimal PreClose
        {
            get { return _preclose; }
            set
            {
                _preclose = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("PreClose");
            }
        }

        public decimal Change
        {
            get { return _change; }
            set
            {
                _change = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Change");
            }
        }

        public string ChangePercentage
        {
            get { return _changepercentage; }
            set
            {
                _changepercentage = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ChangePercentage");
            }
        }

        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public struct QuotePair
        {
            public decimal _oldvalue;
            public decimal _newvalue;
            public QuotePair(decimal ov, decimal nv)
            {
                _oldvalue = ov;
                _newvalue = nv;
            }
            /*
            public QuotePair(decimal nv)
            {
                _oldvalue = _newvalue;
                _newvalue = nv;
            }

            public decimal Value
            {
                get { return _newvalue; }
                set
                {
                    _oldvalue = _newvalue;
                    _newvalue = value;
                }
            }
            */
        }
    }

    public class QuoteGridList : ObservableCollection<QuoteGridLine>
    {
    }


    [ValueConversion(typeof(QuoteGridLine.QuotePair),typeof(decimal))]
    public class QuotePairToQuoteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            QuoteGridLine.QuotePair qp = (QuoteGridLine.QuotePair)value;
            return qp._newvalue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(QuoteGridLine.QuotePair), typeof(System.Windows.Media.Brush))]
    public class QuotePairToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            QuoteGridLine.QuotePair qp = (QuoteGridLine.QuotePair)value;
            System.Windows.Media.Brush brush = qp._newvalue > qp._oldvalue ? System.Windows.Media.Brushes.Green
                        : (qp._newvalue < qp._oldvalue ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.White);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
