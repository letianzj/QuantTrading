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

namespace Modules.OrderAndPositions.Model
{   
    public class OrderEntry : INotifyPropertyChanged
    {
        private long _orderid;
        private string _account;
        private string _symbol;
        private string _type;
        private decimal _price;
        private int _size;
        private int _time;
        private string _status;

        public OrderEntry(long id, string account, string symbol, string type, decimal price, int size, int time, string status) 
        {
            _orderid = id; _account = account; _symbol = symbol; _type = type; _price = price; _size = size; _time = time; _status = status;
        }

        public long OrderId
        {
            get { return _orderid; }
            set
            {
                _orderid = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("OrderId");
            }
        }

        public string Account
        {
            get { return _account; }
            set
            {
                _account = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Account");
            }
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

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Type");
            }
        }

        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Price");
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

        public int Time
        {
            get { return _time; }
            set
            {
                _time = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Time");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Status");
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
    }

    public class OrderTable : ObservableCollection<OrderEntry> {}


    public class FillEntry : INotifyPropertyChanged
    {
        private long _orderid;
        private int _filltime;
        private string _symbol;
        private int _fillsize;
        private decimal _fillprice;

        public FillEntry(long id, int time, string symbol, int size, decimal price)
        {
            _orderid = id; _filltime = time; _symbol = symbol; _fillsize = size; _fillprice = price;
        }
        public long OrderId
        {
            get { return _orderid; }
            set
            {
                _orderid = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("OrderId");
            }
        }

        public int FillTime
        {
            get { return _filltime; }
            set
            {
                _filltime = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("FillTime");
            }
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

        public int FillSize
        {
            get { return _fillsize; }
            set
            {
                _fillsize = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("FillSize");
            }
        }

        public decimal FillPrice
        {
            get { return _fillprice; }
            set
            {
                _fillprice = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("FillPrice");
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
    }

    public class FillTable : ObservableCollection<FillEntry> {}

    public class PositionEntry : INotifyPropertyChanged
    {
        private long _index;
        private string _symbol;
        private decimal _avgprice;
        private int _size;
        private decimal _closepl;
        private decimal _openpl;

        public PositionEntry(long index, string symbol, decimal avgprice, int size, decimal closepl, decimal openpl)
        {
            _index = index; _symbol = symbol; _avgprice = avgprice;
            _size = size; _closepl = closepl; _openpl = openpl;
        }

        public long Index
        {
            get { return _index; }
            set
            {
                _index = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Index");
            }
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

        public decimal AvgPrice
        {
            get { return _avgprice; }
            set
            {
                _avgprice = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("AvgPrice");
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

        public decimal ClosePL
        {
            get { return _closepl; }
            set
            {
                _closepl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ClosePL");
            }
        }

        public decimal OpenPL
        {
            get { return _openpl; }
            set
            {
                _openpl = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("OpenPL");
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
    }

    public class PositionTable : ObservableCollection<PositionEntry> {}

    public class ResultEntry : INotifyPropertyChanged
    {
        private string _statistics;
        private string _result;

        public string Statistics
        {
            get { return _statistics; }
            set
            {
                _statistics = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Statistics");
            }
        }

        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Result");
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
    }

    public class ResultTable : ObservableCollection<ResultEntry> {}
}
