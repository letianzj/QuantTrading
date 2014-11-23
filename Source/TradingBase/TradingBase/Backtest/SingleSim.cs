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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TradingBase
{
    /// <summary>
    /// Unlike MultiSim, multiple symbols in the same day are not played in sequence.
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class SingleSim : HistSim
    {
        // working variables
        Broker _broker = new Broker();
        bool _inited = false;
        long _nextticktime = ENDSIM;
        int _executions = 0;
        int _availticks = 0;
        volatile int _tickcount;
        long lasttime = long.MinValue;
        bool orderok = true;
        // full path of each tick file
        string[] _tickfiles = new string[0];
        // date of each tick file
        List<long> _dates = new List<long>();
        // tick reader of each tick file
        List<TickReader> _secreaders = new List<TickReader>();
        

        // user-facing interfaces
        /// <summary>
        /// An estimate of total ticks available for processing; not available for SingleSim
        /// </summary>
        public int TicksPresent { get { return 0; } }
        /// <summary>
        /// Ticks processed in this simulation run.
        /// </summary>
        public int TicksProcessed { get { return _tickcount; } }
        /// <summary>
        /// Fills executed during this simulation run.
        /// </summary>
        public int FillCount { get { return _executions; } }
        /// <summary>
        /// Gets next tick in the simulation
        /// </summary>
        public long NextTickTime { get { return _nextticktime; } }
        /// <summary>
        /// Gets broker used in the simulation
        /// </summary>
        public Broker SimBroker { get { return _broker; } }

        // event
        public event Action<Tick> GotTickHandler;
        public event Action<string> GotDebugHandler;

        public static long ENDSIM = long.MaxValue;
        public static long STARTSIM = long.MinValue;

        /// <summary>
        /// Create a historical simulator
        /// </summary>
        /// <param name="filenames">list of tick files to use, obtained from OpenFileDialog</param>
        public SingleSim(string[] filenames)
        {
            Reset();
            SetTickFiles(filenames);
        }

        /// <summary>
        /// Reset the simulation
        /// </summary>
        public void Reset()
        {
            orderok = true;
            simstart = 0;
            simend = 0;
            _inited = false;
            _tickfiles = new string[0];
            _dates.Clear();
            _secreaders.Clear();
            _nextticktime = STARTSIM;
            _broker.Reset();
            _executions = 0;
            _tickcount = 0;
        }

        public void SetTickFiles(string[] filenames)
        {
            _tickfiles = filenames;
        }

        /// <summary>
        /// Reinitialize the cache
        /// </summary>
        public void Initialize()
        {
            if (_inited) return; // only init once
            if (_tickfiles.Length == 0) return;     // no tick files

            List<long> ds = new List<long>(_tickfiles.Length);
            List<TickReader> trs = new List<TickReader>(_tickfiles.Length);
            // now we have our list, initialize instruments from files
            for (int i = 0; i < _tickfiles.Length; i++)
            {
                long d = Util.DateFromFileName(_tickfiles[i]);
                TickReader tr = new TickReader(_tickfiles[i]);

                ds.Add(d);
                trs.Add(tr);
            }
            // setup our initial index
            long[] didx = ds.ToArray();
            TickReader[] tridx = trs.ToArray();
            Array.Sort(didx, tridx);
            // save index and objects in order
            _secreaders.Clear();
            _dates.Clear();
            _secreaders.AddRange(tridx);
            _dates.AddRange(didx);
            totaltickfiles = _tickfiles.Length - 1;

            OnDebug("Initialized " + (_tickfiles.Length) + " instruments.");
            OnDebug(string.Join(Environment.NewLine.ToString(), _tickfiles));
            // check for event
            if (GotTickHandler != null)
                hasevent = true;
            else
                OnDebug("No GotTick event handler defined!");
            // read in single tick just to get first time for user
            isnexttick();

            // count total ticks represented by files
            // get total ticks represented by files
            _availticks = 0;
            for (int i = 0; i < trs[i].Count; i++)
                if (trs[i] != null)
                    _availticks += trs[i].TotalCount;

            _inited = true;
        }

        Tick next;
        bool hasnext = false;
        bool hasevent = false;
        bool lasttick = false;

        int currenttickfileindex = 0;
        int totaltickfiles = 0;

        /// <summary>
        /// Read one tick file (indexed by currenttickfileindex) to the end
        /// then move on to the next tick file
        /// until the currenttickfileindex points to the end (passes totaltickfiles)
        /// </summary>
        bool isnexttick()
        {
            // finished all the tick files
            if (currenttickfileindex > totaltickfiles)
            {
                lasttick = true;
                // push out last tick stored in variable next
                TickFiles_gotTick(null);
                return false;
            }
            if (!_secreaders[currenttickfileindex].NextTick())
                currenttickfileindex++;
            return true;
        }

        void TickFiles_gotTick(Tick t)
        {
            // process next tick if present
            if (hasnext)
            {
                // execute any pending orders
                SimBroker.Execute(next);
                // send existing tick
                if (hasevent)
                    OnGotTick(next);
                // update last time
                lasttime = next.Datetime;
                orderok &= lasttick || (t.Datetime >= next.Datetime);

            }
            if (lasttick)
            {
                hasnext = false;
                return;
            }
            // update next tick
            next = t;
            hasnext = true;
            _nextticktime = t.Datetime;
            _tickcount++;
        }

        long simstart = 0;
        long simend = 0;
        TimeSpan runtime
        {
            get
            {
                DateTime start = new DateTime(simstart);
                DateTime end = new DateTime(simend);
                return end.Subtract(start);
            }
        }

        /// <summary>
        /// Run simulation to specific time
        /// </summary>
        /// <param name="time">Simulation will run until this time (use HistSim.ENDSIM for last time)</param>
        public void PlayTo(long ftime)
        {
            simstart = DateTime.Now.Ticks;
            orderok = true;
            go = true;
            if (!_inited)
                Initialize();
            if (_inited)
            {
                // process
                while (go && (NextTickTime < ftime) && isnexttick())
                    ;
            }
            else throw new Exception("Histsim was unable to initialize");
            // mark end of simulation
            simend = DateTime.Now.Ticks;
        }

        bool go = true;
        public void Stop()
        {
            go = false;
        }
        
        Security ParseSecurityFromFileName(int tickfileidx) { return ParseSecurityFromFileName(_tickfiles[tickfileidx]); }
        Security ParseSecurityFromFileName(string file)
        {
            Security s = Util.SecurityFromFileName(file);
            return s;
        }

        private void OnGotTick(Tick t)
        {
            var handler = GotTickHandler;
            if (handler != null) handler(t);
        }

        private void OnDebug(string message)
        {
            var handler = GotDebugHandler;
            if (handler != null) handler(message);
        }
    }
}
