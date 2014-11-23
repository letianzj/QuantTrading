using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class HistoricalDataMessage : IBMessage 
    {
        protected int requestId;
        protected string date;
        protected double open;
        protected double high;
        protected double low;
        protected double close;
        protected int volume;
        protected int count;
        protected double wap;
        protected bool hasGaps;

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }
        
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        
        public double Open
        {
            get { return open; }
            set { open = value; }
        }
        

        public double High
        {
            get { return high; }
            set { high = value; }
        }
        
        public double Low
        {
            get { return low; }
            set { low = value; }
        }
        
        public double Close
        {
            get { return close; }
            set { close = value; }
        }
        
        public int Volume
        {
            get { return volume; }
            set { volume = value; }
        }
        
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public double Wap
        {
            get { return wap; }
            set { wap = value; }
        }

        public bool HasGaps
        {
            get { return hasGaps; }
            set { hasGaps = value; }
        }

        public HistoricalDataMessage(int reqId, string date, double open, double high, double low, double close, int volume, int count, double WAP, bool hasGaps)
        {
            Type = MessageType.HistoricalData;
            RequestId = reqId;
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            Count = count;
            Wap = WAP;
            HasGaps = hasGaps;
        }
    }
}
