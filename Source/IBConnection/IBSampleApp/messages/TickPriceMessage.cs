using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class TickPriceMessage : MarketDataMessage
    {
        private double price;
        private int canAutoExecute;

        public TickPriceMessage(int requestId, int field, double price, int canAutoExecute) : base(MessageType.TickPrice, requestId, field)
        {
            this.price = price;
            this.canAutoExecute = canAutoExecute;
        }

        public int CanAutoExecute
        {
            get { return canAutoExecute; }
            set { canAutoExecute = value; }
        }
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

    }
}
