using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public abstract class OrderMessage : IBMessage
    {
        protected int orderId;

        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }
    }
}
