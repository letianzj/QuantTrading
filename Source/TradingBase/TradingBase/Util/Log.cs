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

using System.IO;

namespace TradingBase
{
    public class Log
    {
        string _logname = string.Empty;
        public string Program { get { return _logname; } }
        int _date = Util.ToIntDate(DateTime.Today);
        public int Date { get { return _date; } }

        StreamWriter _log = null;
        public bool _isEnabled = true;
        private string fullname = string.Empty;
        public string FullName { get { return fullname; } }

        private StringBuilder _content = new StringBuilder();
        public string Content { get { return _content.ToString(); } }

        bool _timestamps = true;
        bool _dateinname = true;
        string _path = string.Empty;
        bool _append = true;

        public Log(string path) : this(path, "log", true, true, true) { }

        public Log(string path, string logname, bool dateinlogname, bool appendtolog, bool timestamps)
        {
            _timestamps = timestamps;
            _dateinname = dateinlogname;
            _path = path;
            _append = appendtolog;
            _logname = logname;
            setfile();
        }

        void setfile()
        {
            fullname = getfn(_path, _logname, _dateinname);
            try
            {
                try
                {
                    if (_log != null)
                        _log.Close();
                }
                catch { }
                _log = new StreamWriter(fullname, _append);
                _log.AutoFlush = true;
            }
            catch (Exception) { _log = null; }
        }

        string getfn(string path, string logname, bool dateinlogname)
        {
            string fn = string.Empty;
            int inst = -1;
            do
            {
                inst++;
                string inststring = inst < 0 ? string.Empty : "." + inst.ToString();
                fn = path + "\\" + logname + (dateinlogname ? "." + _date : "") + inststring + ".txt";
            } while (!Util.IsFileWritetable(fn));
            return fn;
        }

        public event Action<string> SendDebug;
        /// <summary>
        /// log something
        /// </summary>
        /// <param name="msg"></param>
        public void GotDebug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
            if (!_isEnabled)
                return;
            try
            {
                if (_log != null)
                {
                    StringBuilder sb = new StringBuilder();
                    if (_timestamps)
                    {
                        DateTime now = DateTime.Now;
                        // see if date changed
                        int newdate = Util.ToIntDate(now);
                        // if date has changed, start new file
                        if (newdate != _date)
                        {
                            _date = newdate;
                            setfile();
                        }
                        sb.Append(now.ToString("HHmmss"));
                        sb.Append(": ");
                    }
                    sb.Append(msg);
                    _log.WriteLine(sb.ToString());
                    _content.Append(sb.ToString());
                }
            }
            catch { }
        }


        /// <summary>
        /// close the log
        /// </summary>
        public void Stop()
        {
            try
            {
                if (_log != null) _log.Close();
            }
            catch { }
        }
    }
}
