using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace IBSampleApp.messages
{
    public class PositionMessage : IBMessage 
    {
        private string account;
        private Contract contract;
        private int position;
        private double averageCost;
        
        public PositionMessage(string account, Contract contract, int pos, double avgCost)
        {
            Type = MessageType.Position;
            Account = account;
            Contract = contract;
            Position = pos;
            AverageCost = avgCost;
        }

        public string Account
        {
            get { return account; }
            set { account = value; }
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
        
        public double AverageCost
        {
            get { return averageCost; }
            set { averageCost = value; }
        }
    }
}
