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
using System.IO;
using System.Threading.Tasks;

namespace TradingBase
{
    /// <summary>
    /// Write ticks for one symbol to a text file
    /// </summary>
    public class TickWriter
    {
        string _fullsymbol;
        string _path;
        int _date;
        string _file;
        int _count = 0;
        StreamWriter _outstream;

        public string FullSymbol { get { return _fullsymbol; } }
        public string FilePath { get { return _path; } }
        public string FileName { get { return _file; } }
        public int Date { get { return _date; } }
        public int Count { get { return _count; } }

        public TickWriter(string path, string fullsymbol) : this(path, fullsymbol, Util.ToIntDate(DateTime.Now)) { }
        public TickWriter(string path, string fullsymbol, int date)
        {
            _fullsymbol = fullsymbol;
            _path = path;
            _date = date;
            // generate file name for symbol and date
            _file = Util.SafeFilename(_fullsymbol, _path, _date);

            // always overwrite
            _outstream = new StreamWriter(_file, false);

            // reserve haad for the tick count
            ReserverHead();
        }

        public void Close()
        {
            if (_outstream != null)
            {
                // Append on top total ticks for check
                // string s = _count.ToString();
                //_outstream.BaseStream.Position = _reservedplace - s.Length;
                //_outstream.Write(s);
                // Overwrite the haader
                _outstream.BaseStream.Position = 0;
                _outstream.Write(_count.ToString());
                // write to disk
                _outstream.Flush();
                _outstream.Close();
            }
        }

        /// <summary>
        /// Write in the following sequence: Date Time trade bid ask depth
        /// Empty quote is Tick field default value, which is 0
        /// </summary>
        public void newTick(Tick k)
        {
            // get types
            _outstream.WriteLine(k.ToString());

            // write to disk
            _outstream.Flush();
            // count it
            _count++;
        }

        private const int _reservedplace = 20;
        /// <summary>
        /// reserve at the beginning of tick file for tick count
        /// </summary>
        private void ReserverHead()
        {
            char[] c = new char[_reservedplace];
            for (int i = 0; i < _reservedplace; i++)
                c[i] = ' ';
            _outstream.WriteLine(c);         // reserve blanks
        }
    }
}
