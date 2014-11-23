using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class ContractDetailsEndMessage : IBMessage
    {
        public ContractDetailsEndMessage()
        {
            Type = MessageType.ContractDataEnd;
        }
    }
}
