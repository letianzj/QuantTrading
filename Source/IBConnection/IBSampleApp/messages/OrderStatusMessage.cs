using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class OrderStatusMessage : OrderMessage
    {
        private string status;
        private int filled;
        private int remaining;
        private double avgFillPrice;
        private int permId;
        private int parentId;
        private double lastFillPrice;
        private int clientId;
        private string whyHeld;

        public OrderStatusMessage(int orderId, string status, int filled, int remaining, double avgFillPrice,
           int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
            Type = MessageType.OrderStatus;
            OrderId = orderId;
            Status = status;
            Filled = filled;
            Remaining = remaining;
            AvgFillPrice = avgFillPrice;
            PermId = permId;
            ParentId = parentId;
            LastFillPrice = lastFillPrice;
            ClientId = clientId;
            WhyHeld = whyHeld;
        }
        
        public string Status
        {
            get { return status; }
            set { status = value; }
        }
        
        public int Filled
        {
            get { return filled; }
            set { filled = value; }
        }
        
        public int Remaining
        {
            get { return remaining; }
            set { remaining = value; }
        }
        
        public double AvgFillPrice
        {
            get { return avgFillPrice; }
            set { avgFillPrice = value; }
        }
        
        public int PermId
        {
            get { return permId; }
            set { permId = value; }
        }
        
        public int ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }
        
        public double LastFillPrice
        {
            get { return lastFillPrice; }
            set { lastFillPrice = value; }
        }
        
        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        public string WhyHeld
        {
            get { return whyHeld; }
            set { whyHeld = value; }
        }

    }
}
