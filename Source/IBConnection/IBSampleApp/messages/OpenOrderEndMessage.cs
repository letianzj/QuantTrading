using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class OpenOrderEndMessage : OrderMessage
    {
        public OpenOrderEndMessage()
        {
            Type = MessageType.OpenOrderEnd;
        }
    }
}
