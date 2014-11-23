using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class RealTimeBarMessage : HistoricalDataMessage
    {
        private long timestamp;
        private long longVolume;

        public long LongVolume
        {
            get { return longVolume; }
            set { longVolume = value; }
        }

        public long Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public RealTimeBarMessage(int reqId, long date, double open, double high, double low, double close, long volume, double WAP, int count)
            : base(reqId, "", open, high, low, close, -1, count, WAP, false)
        {
            Type = MessageType.RealTimeBars;
            Timestamp = date;
            LongVolume = volume;
        }
    }
}
