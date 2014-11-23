using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class TickOptionMessage : MarketDataMessage
    {
        private double impliedVolatility;
        private double delta;
        private double optPrice;
        private double pvDividend;
        private double gamma;
        private double vega;
        private double theta;
        private double undPrice;

        public TickOptionMessage(int requestId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
            : base(MessageType.TickOptionComputation, requestId, field)
        {
            ImpliedVolatility = impliedVolatility;
            Delta = delta;
            OptPrice = optPrice;
            PvDividend = pvDividend;
            Gamma = gamma;
            Vega = vega;
            Theta = theta;
            UndPrice = undPrice;
        }

        public double ImpliedVolatility
        {
            get { return impliedVolatility; }
            set { impliedVolatility = value; }
        }
        
        public double Delta
        {
            get { return delta; }
            set { delta = value; }
        }        

        public double OptPrice
        {
            get { return optPrice; }
            set { optPrice = value; }
        }
        
        public double PvDividend
        {
            get { return pvDividend; }
            set { pvDividend = value; }
        }

        public double Gamma
        {
            get { return gamma; }
            set { gamma = value; }
        }

        public double Vega
        {
            get { return vega; }
            set { vega = value; }
        }

        public double Theta
        {
            get { return theta; }
            set { theta = value; }
        }

        public double UndPrice
        {
            get { return undPrice; }
            set { undPrice = value; }
        }
                
    }
}
