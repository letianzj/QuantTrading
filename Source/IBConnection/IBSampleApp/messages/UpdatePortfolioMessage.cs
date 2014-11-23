using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace IBSampleApp.messages
{
    public class UpdatePortfolioMessage : IBMessage
    {
        private Contract contract;
        private int position;
        private double marketPrice;
        private double marketValue;
        private double averageCost;
        private double unrealisedPNL;
        private double realisedPNL;
        private string accountName;

        public UpdatePortfolioMessage(Contract contract, int position, double marketPrice, double marketValue, double averageCost, double unrealisedPNL, double realisedPNL, string accountName)
        {
            Type = MessageType.PortfolioValue;
            Contract = contract;
            Position = position;
            MarketPrice = marketPrice;
            MarketValue = marketValue;
            AverageCost = averageCost;
            UnrealisedPNL = unrealisedPNL;
            RealisedPNL = realisedPNL;
            AccountName = accountName;
        }

        public Contract Contract
        {
            get { return contract; }
            set { contract = value; }
        }
        
        public int Position
        {
            get { return position; }
            set { position = value; }
        }
        
        public double MarketPrice
        {
            get { return marketPrice; }
            set { marketPrice = value; }
        }
        
        public double MarketValue
        {
            get { return marketValue; }
            set { marketValue = value; }
        }
        
        public double AverageCost
        {
            get { return averageCost; }
            set { averageCost = value; }
        }
        
        public double UnrealisedPNL
        {
            get { return unrealisedPNL; }
            set { unrealisedPNL = value; }
        }
        
        public double RealisedPNL
        {
            get { return realisedPNL; }
            set { realisedPNL = value; }
        }
        
        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
        }

    }
}
