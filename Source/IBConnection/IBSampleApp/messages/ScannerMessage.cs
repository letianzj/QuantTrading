using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace IBSampleApp.messages
{
    public class ScannerMessage : IBMessage
    {
        private int requestId;
        private int rank;
        private ContractDetails contractDetails;
        private string distance;
        private string benchmark;
        private string projection;
        private string legsStr;

        public ScannerMessage(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            Type = MessageType.ScannerData;
            RequestId = reqId;
            Rank = rank;
            ContractDetails = contractDetails;
            Distance = distance;
            Benchmark = benchmark;
            Projection = projection;
            LegsStr = legsStr;
        }

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }

        public ContractDetails ContractDetails
        {
            get { return contractDetails; }
            set { contractDetails = value; }
        }

        public string Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public string Benchmark
        {
            get { return benchmark; }
            set { benchmark = value; }
        }
        

        public string Projection
        {
            get { return projection; }
            set { projection = value; }
        }
        

        public string LegsStr
        {
            get { return legsStr; }
            set { legsStr = value; }
        }

    }
}
