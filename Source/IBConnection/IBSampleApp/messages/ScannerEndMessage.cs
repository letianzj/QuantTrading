using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class ScannerEndMessage : IBMessage
    {
        private int requestId;

        public ScannerEndMessage(int requestId)
        {
            Type = MessageType.ScannerDataEnd;
            RequestId = requestId;
        }

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }
    }
}
