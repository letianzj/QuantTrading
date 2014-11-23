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

namespace TradingBase
{
    /// <summary>
    /// keep track of whether data is arriving, or connection is lost
    /// </summary>
    public class ConnectionTracker
    {
        /// <summary>
        /// default milliseconds between polls (MassAlert check interval)
        /// TickWatcher sleeps before re-check
        /// </summary>
        public const int DEFAULTPOLLMS = 5000;
        /// <summary>
        /// default seconds for timeout of a given symbol
        /// </summary>
        public const int DEFAULTTIMEOUTSEC = 300;
        TickWatcher _tw;
        IClient _ibclient = null;
        /// <summary>
        /// called when connection is made or refreshed
        /// </summary>
        public event Action GotConnect;
        /// <summary>
        /// called when no ticks have been received since timeout
        /// </summary>
        public event Action GotConnectFail;
        /// <summary>
        /// contains information useful for debugging
        /// </summary>
        public event Action<string> GotDebug;
        /// <summary>
        /// create a tracker for selected provider on client
        /// </summary>
        /// <param name="client"></param>
        public ConnectionTracker(IClient client) : this(DEFAULTPOLLMS,DEFAULTTIMEOUTSEC,client) { }
        /// <summary>
        /// create a tracker for preferred provider with custom tick timeout and poll frequency settings
        /// </summary>
        /// <param name="pollintervalms"></param>
        /// <param name="timeoutintervalsec"></param>
        /// <param name="client"></param>
        /// <param name="PreferredBroker"></param>
        /// <param name="connectany"></param>
        public ConnectionTracker(int pollintervalms, int timeoutintervalsec, IClient client)
        {
            _ibclient = client;
            _tw = new TickWatcher(pollintervalms);
            _tw.AlertThreshold = timeoutintervalsec;
            // handle situations when no ticks arrive
            _tw.GotMassAlert += new Action<int>(_tw_GotMassAlert);

            _connected = _ibclient.isServerConnected;

            if (!_connected && (GotConnectFail != null))
                GotConnectFail();
            if (_connected && (GotConnect != null))
                GotConnect();
        }

        public TickWatcher tw { get { return _tw; } set { _tw = value; } }
        /// <summary>
        /// # of seconds a symbol (or all symbols for MassAlert) has to stop ticking before alerts are sent
        /// </summary>
        public int AlertThreshold { get { return _tw.AlertThreshold; } set { _tw.AlertThreshold = value; } }
        /// <summary>
        /// # of MILLIseconds to wait between MassAlert tests
        /// </summary>
        public int PollInterval { get { return _tw.BackgroundPollInterval; } set { _tw.BackgroundPollInterval = value; } }

        /// <summary>
        /// provider present connected?
        /// </summary>
        public bool isConnected { get { return _connected; } }
        bool _connected = false;
        int _lastalert = 0;
        void _tw_GotMassAlert(int number)
        {
            _lastalert = number;
            // first disconnect
            if (_connected)
            {
                if (GotDebug != null)
                    GotDebug("No ticks from broker since: " + _lastalert +". Will attempt refresh.");
                if (GotConnectFail != null)
                    GotConnectFail();
            }
            _connected = false;
            if (_ibclient == null) return;
            // attempt to reconnect
            Reconnect();

        }
        bool _wait4firsttickreconnect = false;
        /// <summary>
        /// wait for first tick before reconnecting
        /// </summary>
        public bool ReconnectOnlyAfterFirstTick { get { return _wait4firsttickreconnect; } set { _wait4firsttickreconnect = value; } }
        /// <summary>
        /// reconnect/refresh now
        /// </summary>
        public void Reconnect()
        {

            try
            {
                _ibclient.Connect();
            }
            catch (Exception ex)
            {
                if (GotDebug != null)
                {
                    GotDebug("Cann't reconnect to IB: " + ex.Message);
                    return;
                }
            }
            bool success = _ibclient.isServerConnected;
            // change in connect status (eg fail->success)
            if (success)
            {
                if (GotDebug != null)
                    GotDebug("IB connection refresh at " + _lastalert);
                if (GotConnect != null)
                    GotConnect();
            }
            _connected = success;
        }

        /// <summary>
        /// start tracker
        /// not needed; automatic start, driven by newTick
        /// </summary>
        [Obsolete]
        public void Start()
        {
        }
        /// <summary>
        /// stop tracker
        /// </summary>
        public void Stop()
        {
            try
            {
                _tw.Stop();
            }
            catch { }
        }

        /// <summary>
        /// call this function from GotTick
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            _tw.newTick(k);
        }
    }
}
