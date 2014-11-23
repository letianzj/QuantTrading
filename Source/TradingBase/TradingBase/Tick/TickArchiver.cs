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

using System.Collections.Concurrent;

namespace TradingBase
{
    /// <summary>
    /// Archive ticks for a group of symbols
    /// </summary>
    public class TickArchiver
    {
        string _folderpath;
        public string FolderPath { get { return _folderpath; } set { _folderpath = value; } }

        Dictionary<string, TickWriter> _filedict = new Dictionary<string, TickWriter>();
        ConcurrentDictionary<string, int> _datedict = new ConcurrentDictionary<string, int>();

        public TickArchiver(string folderpath)
        {
            _folderpath = folderpath;
        }

        bool _stopped = false;

        public void Stop()
        {
            try
            {
                foreach (string file in _filedict.Keys)
                {
                    _filedict[file].Close();
                }
                _stopped = true;
            }
            catch
            { }
        }

        public bool newTick(Tick t)
        {
            if (_stopped) return false;
            if ((t.FullSymbol == null) || (t.FullSymbol == "")) return false;
            TickWriter tw;
            // prepare last date of tick
            int lastdate = t.Date;
            lastdate = _datedict.GetOrAdd(t.FullSymbol, t.Date);

            // see if we need a new day
            bool samedate = lastdate == t.Date;
            // see if we have stream already
            bool havestream = _filedict.TryGetValue(t.FullSymbol, out tw);
            // if no changes, just save tick
            if (samedate && havestream)
            {
                try
                {
                    tw.newTick((Tick)t);
                    return true;
                }
                catch (IOException) { return false; }
            }
            else
            {
                try
                {
                    // if new date, close stream
                    if (!samedate)
                    {
                        try
                        {
                            tw.Close();
                        }
                        catch (IOException) { }
                    }
                    // ensure file is writable
                    string file = Util.SafeFilename(t.FullSymbol, _folderpath, t.Date);
                    if (TickUtil.IsFileWritetable(file))
                    {
                        // open new stream
                        tw = new TickWriter(_folderpath, t.FullSymbol, t.Date);
                        // save tick
                        tw.newTick((Tick)t);
                        // save stream
                        if (!havestream)
                            _filedict.Add(t.FullSymbol, tw);
                        else
                            _filedict[t.FullSymbol] = tw;
                        // save date if changed
                        if (!samedate)
                        {
                            _datedict[t.FullSymbol] = t.Date;
                        }
                    }
                }
                catch (IOException) { return false; }
                catch (Exception) { return false; }
            }

            return false;
        }
    }
}
