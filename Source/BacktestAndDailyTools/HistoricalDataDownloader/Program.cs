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

/// As a console application, it is designed to run automatically by windows taskmanager
///     or by QTWindow.Process.Start

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TradingBase;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace HistoricalDataDownloader
{
    class Program
    {
        /// <summary>
        /// Arguments are GOOG/IB, rundate, basketfile, datapath
        /// </summary>
        static void Main(string[] args)
        {
            #region parameters
            string _client = "IB";          // IB or GOOG
            IClient _iclient;
            string _enddate = DateTime.Today.ToString("yyyyMMdd");
            string _basketfile = @"c:\QuantTrading\Config\basket.xml";
            string _datapath = @"c:\QuantTrading\HistData\";
            Basket _basket;

            if (args.Length > 0)
            {
                _client = args[0];
            }
            if (args.Length > 1)
            {
                _enddate = args[1];
            }
            if (args.Length > 2)
            {
                _basketfile = args[2];
            }
            if (args.Length > 3)
            {
                _datapath = args[3];
            }
            _datapath = _datapath + _client + @"\";
            
            _basket = Basket.DeserializeFromXML(_basketfile);
            #endregion

            _sec2bars = new Dictionary<string, List<string>>(_basket.Count);
            Dictionary<string,string> _outfiles = new Dictionary<string,string>(_basket.Count);
            Dictionary<string, long> _totalBars = new Dictionary<string, long>(_basket.Count);
            _processedBars = new Dictionary<string, long>(_basket.Count);

            DateTime _startTime = DateTime.SpecifyKind(DateTime.ParseExact(_enddate, "yyyyMMdd", null), DateTimeKind.Local);
            DateTime _endTime = _startTime + new TimeSpan(1, 0, 0, 0);          // one day later
            double sec = _endTime.Subtract(_startTime).TotalSeconds;      // should be 24 hours or 86,400 secs

            foreach(string s in _basket.Securities)
            {
                // set up bar counts
                _totalBars.Add(s, (long)sec);                       // one bar is one sec; if one minute, divide it by 60
                _processedBars.Add(s, 0);                           // initialize processed to 0

                // set up out filenames
                string filename = _datapath + s + "_" + _startTime.Date.ToString("yyyyMMdd") + ".csv";
                _outfiles.Add(s, filename);
                List<string> lines = new List<string>(90000);           // set something greater than 86,4000
                lines.Add("DateTime,Open,High,Low,Close,Volume");
                _sec2bars.Add(s, lines);
            }
            
            if (_client == "IB")
            {
                _iclient = new IBClient();          // currently it uses default ip:port
                _iclient.SendDebugEventDelegate += _iclient_SendDebugEventDelegate;
                _iclient.GotHistoricalBarDelegate += _iclient_GotHistoricalBarDelegate;
                _iclient.Connect();

                long totalRequests = (long)sec / 1800;           // one request is 30 min, totalRequests = 48
                TimeSpan thirtyMin = new TimeSpan(0, 30, 0);

                foreach (string sym in _basket.Securities)
                {
                    DateTime s = _startTime;
                    DateTime t = _startTime + thirtyMin;

                    Console.WriteLine("Requesting historical bars for :" + sym);
                    for (int i = 0; i < totalRequests; i++)
                    {
                        Console.WriteLine("Request #: " + (i+1).ToString() + "/"+totalRequests);
                        // 1 = 1 second
                        BarRequest br = new BarRequest(sym, 1, Util.ToIntDate(s.Date), Util.ToIntTime(s.Hour, s.Minute, s.Second),
                            Util.ToIntDate(t.Date), Util.ToIntTime(t.Hour, t.Minute, t.Second), _client);
                        _iclient.RequestHistoricalData(br, true);

                        // Do not make more than 60 historical data requests in any ten-minute period.
                        // If I have 10 names, each can only make 6 requests in ten minute;
                        // I use 5 minute for a pause; Then 24 hours takes 120 min or 1.5hour
                        // Thread.Sleep(new TimeSpan(0, 5, 0));
                        // wait 10 secs
                        Thread.Sleep(10000);
                        s += thirtyMin;
                        t += thirtyMin;
                    }
                }
            }
            else if (_client == "GOOG")
            {
                _iclient = new GoogleClient(1);
                _iclient.SendDebugEventDelegate += _iclient_SendDebugEventDelegate;
                _iclient.GotHistoricalBarDelegate += _iclient_GotHistoricalBarDelegate;

                foreach (string sym in _basket.Securities)
                {
                    Console.WriteLine("Requesting historical bars for :" + sym);
                    BarRequest br = new BarRequest(sym, 60, Util.ToIntDate(DateTime.Today), Util.ToIntTime(DateTime.Today),
                            Util.ToIntDate(DateTime.Today), Util.ToIntTime(DateTime.Today), _client);
                    _iclient.RequestHistoricalData(br);
                }
            }

            // write to files
            Console.WriteLine("Wait three minutes for bars being processed.....");
            Thread.Sleep(new TimeSpan(0, 3, 0));            // wait three minutes for all hist bar to be processed.

            foreach (string s in _basket.Securities)
            {
                List<string> noDups = _sec2bars[s].Distinct().ToList();
                //_sec2bars[s].Insert(0, _processedBars[s].ToString());
                File.WriteAllLines(_outfiles[s], noDups);
            }
        }

        // symbol --> list of historical bars
        static Dictionary<string, List<string>> _sec2bars;
        static Dictionary<string,long> _processedBars;

        static void _iclient_GotHistoricalBarDelegate(Bar b)
        {
            // Find the security file
            if (_sec2bars.ContainsKey(b.FullSymbol))
            {
                DateTime d = Util.ToDateTime(b.Date, b.BarStartTime);
                DateTime d_local = DateTime.SpecifyKind(d, DateTimeKind.Local);         // to local time
                string line = d_local.ToString("yyyy/MM/dd HH:mm:ss") + "," +b.Open + "," +  b.High + "," + b.Low + "," + b.Close + "," + b.Volume;
                
                _sec2bars[b.FullSymbol].Add(line);
                _processedBars[b.FullSymbol]++;
            }
        }

        static void _iclient_SendDebugEventDelegate(string obj)
        {
            Console.WriteLine("Exception: " + obj);
        }
    }
}
