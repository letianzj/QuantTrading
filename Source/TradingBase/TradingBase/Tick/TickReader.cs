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
    /// read tick files
    /// </summary>
    public class TickReader
    {
        public string FullSybmol { get; set; }
        public int Date { get; set; }
        public string FilePathName { get; set; }
        /// <summary>
        /// How many have read
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// how many in total
        /// </summary>
        public int TotalCount { get; set; }
        public bool IsValid 
        { 
            get {
                if (_instream.BaseStream != null)
                    return _instream.BaseStream.CanRead;
                else
                    return false;
            } 
        }
        StreamReader _instream;

        public TickReader(string filepathname)
        {
            Count = 0;
            FilePathName = filepathname;
            Date = Util.DateFromFileName(filepathname);
            FullSybmol = Util.SecurityFromFileName(filepathname).FullSymbol;
            _instream = new StreamReader(filepathname);
            // read the header for total count
            try
            {
                string msg = _instream.ReadLine();
                TotalCount = Convert.ToInt32(msg);
            }
            catch
            {
                throw new Exception("Exception: unable to read tick file: " + filepathname);
            }
        }

        public void Close()
        {
            _instream.Close();
        }

        /// <summary>
        /// returns true if more data to process, false otherwise
        /// </summary>
        public bool NextTick()
        {
            try
            {
                string msg = _instream.ReadLine();

                if (msg == null)        // end of file
                {
                    _instream.Close();
                    return false;
                }
                else 
                {
                    Tick k = Tick.Deserialize(msg);
                    Count++;

                    OnGotTick(k);

                    return true;
                }
            }
            catch (EndOfStreamException)
            {
                _instream.Close();
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        public event Action<Tick> GotTickHanlder;
        private void OnGotTick(Tick k)
        {
            var handler = GotTickHanlder;
            if (handler != null) handler(k);
        }
    }
}
