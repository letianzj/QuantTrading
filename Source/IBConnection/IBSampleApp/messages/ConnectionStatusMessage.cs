using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class ConnectionStatusMessage : IBMessage
    {
        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
        }

        public ConnectionStatusMessage(bool isConnected)
        {
            Type = MessageType.ConnectionStatus;
            this.isConnected = isConnected;
        }

        
    }
}
