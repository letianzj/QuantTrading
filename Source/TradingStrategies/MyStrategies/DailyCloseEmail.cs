using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using TradingBase;
using TechnicalAnalysisEngine;

namespace MyStrategies
{
    class DailyCloseEmail : StrategyBase
    {
        public DailyCloseEmail()
        {
            _name = "DailyCloseEmail";
            string _basketfile = @"c:\Workspace\QuantTrading\Config\basket.xml";
            Basket _basket = Basket.DeserializeFromXML(_basketfile);

            foreach (string s in _basket.Securities)
            {
                _symbols.Add(s);
            }
        }

        BarListTracker _barlisttracker;
        public override void Reset(bool popup = true)
        {
            // one day bar
            _barlisttracker = new BarListTracker(_symbols.ToArray(), 86400);
            _barlisttracker.GotNewBar += _barlisttracker_GotNewBar;

            emailsent_ = 0;
        }

        private void _barlisttracker_GotNewBar(string sym, int interval)
        {

        }

        int[] sentouttime_ = new int[] { 103000, 133000, 153000 };
        int emailsent_ = 0;
        public override void GotTick(Tick k)
        {
            if (IsActive)
            {
                if ((k.Time >= 93000) && (k.Time <= 160200))
                    _barlisttracker.NewTick(k);
                else if (k.Time > 160200)
                    Shutdown();

                if (k.Time > sentouttime_[emailsent_] + 1)
                {
                    emailsent_++;
                    if (emailsent_ == sentouttime_.Length)
                        Shutdown();

                    // send out email
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<table>");
                    sb.Append("<tr><td>Symbol</td><td>Open</td><td>High</td><td>Low</td><td>Close</td><td>Change</td><td>Change(%)</td></tr>");
                    foreach (string s in _symbols)
                    {
                        string[] symbol = s.Split(' ');
                        BarList bl = _barlisttracker[s, 86400];

                        try
                        {
                            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>",
                            symbol[0], (bl.Open()[0]).ToString("N2"), (bl.High()[0]).ToString("N2"),
                            (bl.Low()[0]).ToString("N2"), (bl.Close()[0]).ToString("N2"),
                            (bl.Close()[0] - _precloseDict[s]).ToString("N2"),
                            (bl.Close()[0] / _precloseDict[s] - 1).ToString("0.##%"));
                        }
                        catch { }
                    }
                    sb.Append("<table>");

                    string subject = "Current Market @ " + sentouttime_[emailsent_ - 1];
                    Util.Sendemail(subject, sb.ToString(), true);
                }
            }
        }

        private Dictionary<string, decimal> _precloseDict = new Dictionary<string, decimal>();
        public override void GotHistoricalBar(Bar b)
        {
            if (_precloseDict.ContainsKey(b.FullSymbol))
            {
                _precloseDict[b.FullSymbol] = b.Close;
            }
            else
            {
                _precloseDict.Add(b.FullSymbol, b.Close);
            }
        }
    }
}
