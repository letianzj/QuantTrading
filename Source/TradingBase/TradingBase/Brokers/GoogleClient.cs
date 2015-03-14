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

using System.ComponentModel;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace TradingBase
{
    /// <summary>
    /// Get google http real time quote and 1min historical bar
    /// </summary>
    public class GoogleClient : IClient
    {
        // Properties
        public long ServerTime { get; private set; }
        public string Account { get; private set; }
        public int ClientId { get; set; }
        public long NextValidOrderId { get { return 1; } }
        public bool RequestPositionsOnStartup { get; set; }
        public int ServerVersion { get; private set; }
        public bool isServerConnected { get; private set; }

        public int RefreshInterval { get; set; }

        private CancellationTokenSource tokensource;
        private CancellationToken cancellationtoken;

        // Methods
        public GoogleClient(int refreshinterval)
        {
            Account = "GOOGWEB";
            ClientId = 0;
            RequestPositionsOnStartup = true;
            ServerVersion = 0;
            RefreshInterval = refreshinterval;
        }

        // It requests accounts, and poisitions associated to that account
        public void Connect()       // replace Mode()
        {
            try
            {
                ServerTime = Util.ToIntTime(DateTime.Now);

                if (GotServerUpDelegate != null)
                    GotServerUpDelegate("Google server is up.");
                if (GotAccountsDelegate != null)
                    GotAccountsDelegate("Default account");

                // no position
                //Position p = new Position();
                if (GotPositionDelegate != null)
                {
                    //    GotPositionDelegate(p);
                    Debug("No positions provided by google client.");
                }

                OnGotServerInitialized("ServerVersion:" + ServerVersion);
                OnGotServerInitialized("NextValidOrderId:" + NextValidOrderId);
                OnGotServerInitialized("ServerTime:" + ServerTime);
                OnGotServerInitialized("Account:" + Account);
            }
            catch (Exception e)
            {
                Debug("Error connecting Google client");
                Debug(e.Message);
            }
            
        }

        public void Disconnect()            // replace Disconnect(), stop()
        {
            try
            {
                // cancel market data background worker
                tokensource.Cancel();

                CancelMarketData();
                    

                if (GotServerDownDelegate != null)
                    GotServerDownDelegate("Google serveer is down");

                isServerConnected = false;
            }
            catch (Exception e)
            {
                // Relegate
                Debug("Error disconnecting from google.");
                Debug(e.Message);
            }
        }

        public void Start() { }

        //********************************* Member Variables *************************************//
        // PositionRequest
        // AccountRequest
        #region member variables
        string url = @"https://finance.google.com/finance/info?q=";       // url+symbol
        Dictionary<string, string> SecurityFullNameToGoogleSymbol = new Dictionary<string, string>();
        Dictionary<string, decimal> SecurityFullNameToLastPrice = new Dictionary<string, decimal>();


        public event Action<Tick> GotTickDelegate;
        public event Action<Trade> GotFillDelegate;
        public event Action<Order> GotOrderDelegate;
        public event Action<long> GotOrderCancelDelegate;
        public event Action<Position> GotPositionDelegate;
        public event Action<Bar> GotHistoricalBarDelegate;
        public event Action<string> GotServerInitializedDelegate;
        public event Action<string> SendDebugEventDelegate;

        public event Action<string> GotServerUpDelegate;
        public event Action<string> GotServerDownDelegate;
        public event Action<string> GotAccountsDelegate;

        #endregion



        //********************************* Outgoing Messages *************************************//
        #region Outgoing Messages
        public void RequestMarketData(Basket b)
        {
            string fullname;
            try
            {
                for (int i = 0; i < b.Count; i++)
                {
                    fullname = b[i];
                    if (fullname.Contains("FUT") || fullname.Contains("OPT") || fullname.Contains("FOP"))
                    {
                        Debug("Google Client only support STK and IDX at this time. " + fullname + " skipped.");
                        continue;
                    }
                    string[] fn = fullname.Split(' ');
                    if (!SecurityFullNameToGoogleSymbol.ContainsKey(fullname))
                    {
                        SecurityFullNameToGoogleSymbol.Add(fullname, fn[0]);
                        SecurityFullNameToLastPrice.Add(fullname, 0.0m);
                    }
                }
            }
            catch (Exception e)
            {
                Debug("Error in requesting market data from Google client");
                Debug(e.Message);
            }

            if (!isServerConnected)
            {
                tokensource = new CancellationTokenSource();
                cancellationtoken = tokensource.Token;

                var task = Task.Factory.StartNew(() => GetQuoteLoop(cancellationtoken), cancellationtoken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                    .ContinueWith(antecendent => { Debug("GOOG client stopped"); });
                isServerConnected = true;
            }
        }

        // Unsubscribe
        public void CancelMarketData()
        {
            SecurityFullNameToGoogleSymbol.Clear();
        }

        public void RequestMarketDepth(int depth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// http://www.marketcalls.in/database/google-realtime-intraday-backfill-data.html
        /// http://www.codeproject.com/KB/IP/google_finance_downloader.aspx
        /// </summary>
        /// <param name="br"></param>
        public void RequestHistoricalData(BarRequest br, bool useRTH=false)
        {
            // Google always returns the most recent data. 
            // i is interval in seconds, set to 60s, ignore interval
            // ignore date; p: period
            // q is the symbol (AAPL)
            // x is the exchange (NASD)
            // sessions is the session requested (ext_hours)
            // p is the time period (5d = 5 days), set to 1d, ignore time span
            // f is the requested fields (d,c,v,o,h,l)
            // df = (cpct)
            // auto = (1)
            // ts is potentially a time stamp (1324323553 905) or time shift

            try
            {
                using (WebClient client = new WebClient())
                {
                    string google;
                    if (br.Interval != 86400)
                    {
                       google = @"https://www.google.com/finance/getprices?i="+br.Interval.ToString()+@"&p=1d&f=d,o,h,l,c,v&df=cpct&q=";
                    }
                    else   // for oneday, today is empty
                    {
                        google = @"https://www.google.com/finance/getprices?i=86400&p=2d&f=d,o,h,l,c,v&df=cpct&q=";
                    }

                    string[] symbol = br.FullSymbol.Split(' ');

                    System.IO.Stream data = client.OpenRead(google + symbol[0]);
                    System.IO.StreamReader read = new System.IO.StreamReader(data);

                    string[] lines = new string[] { read.ReadToEnd() };
                    string[] lines2 = lines[0].Split('\n');

                    // get time zone adjustment
                    // In line 6, GOOG has time zone offset = -240 which is new york time; 
                    //      while SPX has time zone offset = -300, which is chicago time.
                    // The following find the additional offset relative to local time.
                    /*
                    int localtimezonediffinminutes = (int)Util.GetUtcOffset(DateTime.Today).TotalMinutes;       // negative offset
                    string stime = lines[6];
                    int itime;
                    bool btime = Int32.TryParse(stime.Substring(stime.IndexOf('=') + 1), out itime);        // negative offset
                    int additionaloffset = localtimezonediffinminutes - itime;          // (-240) - (-300) = 60 mins
                    */

                    IEnumerable<string> history = lines2.Skip(7);        // skip the first 7 lines: header

                    int nlines = 0;                  // count of lines
                    string[] entries;
                    DateTime dstart = DateTime.Now;        // just for initialization
                    DateTime dt = DateTime.Now;

                    foreach (string line in history)
                    {
                        if (!string.IsNullOrEmpty(line))                 // skip empty lines, i.e., the last line
                        {
                            entries = line.Split(',');

                            if (nlines == 0)
                            {
                                // http://www.epochconverter.com/
                                dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                double secs = double.Parse(entries[0].Remove(0, 1));        // remove character 'a'
                                dt = dt.AddSeconds(secs); 
                                // GMT to EST
                                // dstart = TimeZoneInfo.ConvertTimeFromUtc(dt1, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
                                dt = TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
                                dstart = dt;
                            }
                            else
                            {
                                dstart = dt.AddSeconds(Int32.Parse(entries[0])*br.Interval);
                            }
                            
                            nlines++;
                            // write line to database
                            Bar bar = new Bar();
                            bar.Interval = 1;       // 1 sec

                            bar.FullSymbol = br.FullSymbol;

                            bar.Open = decimal.Parse(entries[4]);
                            bar.Date = Util.ToIntDate(dstart);
                            bar.BarOrderInADay = bar.GetOrder(Util.ToIntTime(dstart));
                            bar.High = decimal.Parse(entries[2]);
                            bar.Low = decimal.Parse(entries[3]);
                            bar.Close = decimal.Parse(entries[1]);
                            bar.Volume = long.Parse(entries[5]);

                            if (GotHistoricalBarDelegate != null)
                                GotHistoricalBarDelegate(bar);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug("Error in requesting historical data from Google client.");
                Debug(e.Message);
            }
        }

        public void PlaceOrder(Order o)
        {
            // Assuming that the order is immediately filled
            if (o.IsMarket == false)
            {
                Debug("GoogleClient doesn't fill order type other than market order.");
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                // Order immediately acknowledged
                o.OrderStatus = OrderStatus.Submitted;
                GotOrderDelegate(o);
                Trade trade = (Trade)o;        // Order is Trade
                trade.Account = Account;
                trade.Id = o.Id;

                trade.FullSymbol = o.FullSymbol;
                trade.TradePrice = SecurityFullNameToLastPrice[o.FullSymbol];
                trade.TradeSize = o.OrderSize;
                // trade.Security = SecurityType.STK;


                trade.TradeDate = Util.ToIntDate(DateTime.Now);
                trade.TradeTime = Util.ToIntTime(DateTime.Now);

                // immediately filled
                if (GotFillDelegate != null)
                    GotFillDelegate(trade);
            }
        }

        public void CancelOrder(long strategyOrderId)
        {
            // Assuming that the order has been immediately filled
            // So no order can be canceled
            // throw new NotImplementedException();
            if (GotOrderCancelDelegate != null)
                Debug("Can't cancel order from google client.");
        }
        #endregion

        //********************************* Incoming Messages *************************************//
        #region Incoming Messages
        /// <summary>
        /// Post and Get HTTP requests
        /// GotTick
        /// </summary>
        private void GetQuoteLoop(CancellationToken token)
        {
            string query;

            // it is called after market data request,
            // so SecurityFullNameToGoogleSymbol is not empty
            query = string.Join(",", SecurityFullNameToGoogleSymbol.Select(x => x.Value).ToArray());
            query = url + query;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                
                try
                {
                    // Create a request for the URL. 
                    HttpWebRequest grequest = (HttpWebRequest)WebRequest.Create(query);
                    // If required by the server, set the credentials.
                    grequest.Credentials = CredentialCache.DefaultCredentials;
                    // Get the response.
                    HttpWebResponse gresponse = (HttpWebResponse)grequest.GetResponse();
                    // Display the status.
                    // Console.WriteLine(((HttpWebResponse)gresponse).StatusDescription);
                    // Get the stream containing content returned by the server.
                    Stream gdatastream = gresponse.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    StreamReader greader = new StreamReader(gdatastream);

                    // Read the content.
                    string quotestr = greader.ReadToEnd();

                    // Display the content.
                    // Console.WriteLine(quotestr);
                    quotestr = quotestr.Replace("//", "");
                    // Clean up the streams and the response.
                    greader.Close();
                    gdatastream.Close();
                    gresponse.Close();

                    var quote = JsonConvert.DeserializeObject<List<RealTimeData>>(quotestr);

                            
                    DateTime ct = DateTime.Now;
                    int i = 0;
                    // quote has the same order as that in securities; use this logic to retrieve symbol directly
                    foreach (var sec in SecurityFullNameToGoogleSymbol)
                    {
                        Tick k = new Tick();        // it should create a new tick. Otherwise it overrides.
                        k.Date = ct.Year * 10000 + ct.Month * 100 + ct.Day;
                        k.Time = ct.Hour * 10000 + ct.Minute * 100 + ct.Second;
                        //DateTime dt = DateTime.SpecifyKind(DateTime.Parse(quote[0].lt_dts), DateTimeKind.Utc);      // Z shouldn't refer to local time
                        //dt = dt.ToLocalTime();
                        //k.Date = Util.ToIntDate(dt);
                        //k.Time = Util.ToIntTime(dt);

                        k.FullSymbol = sec.Key;

                        k.TradePrice = Convert.ToDecimal(quote[i].l);
                        k.TradeSize = Convert.ToInt32(quote[i++].s);
                        k.TradeSize = 1000;           // overwrite. It seems that google hasn't provided size yet.
                        SecurityFullNameToLastPrice[sec.Key] = k.TradePrice;

                        if (k.IsValid)
                        {
                            if (GotTickDelegate != null)
                                GotTickDelegate(k);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug("GoogleClient error: " + ex.Message);
                }

                // Sleep 30 seconds
                // Console.WriteLine("Time .... " + Util.ToIntTime(DateTime.Now));
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 0, 0, RefreshInterval));
                
            }
        }

        #endregion

        //********************************* Auxiliary Functions *************************************//
        #region Auxiliary Functions
        void Debug(string obj)
        {
            if (SendDebugEventDelegate != null)
                SendDebugEventDelegate(obj);
        }

        void OnGotServerInitialized(string msg)
        {
            if (GotServerInitializedDelegate != null)
                GotServerInitializedDelegate(msg);
        }
        #endregion
    }

    /// <summary>
    /// http://json2csharp.com/
    /// </summary>
    public class RealTimeData
    {
        public string id { get; set; }
        public string t { get; set; }
        public string e { get; set; }
        public string l { get; set; }
        public string l_fix { get; set; }
        public string l_cur { get; set; }
        public string s { get; set; }
        public string ltt { get; set; }
        public string lt { get; set; }
        public string lt_dts { get; set; }
        public string c { get; set; }
        public string c_fix { get; set; }
        public string cp { get; set; }
        public string cp_fix { get; set; }
        public string ccol { get; set; }
        public string pcls_fix { get; set; }
        public string el { get; set; }
        public string el_fix { get; set; }
        public string el_cur { get; set; }
        public string elt { get; set; }
        public string ec { get; set; }
        public string ec_fix { get; set; }
        public string ecp { get; set; }
        public string ecp_fix { get; set; }
        public string eccol { get; set; }
        public string div { get; set; }
        public string yld { get; set; }
    };
}
