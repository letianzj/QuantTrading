using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class AccountDownloadEndMessage : IBMessage
    {
        private string account;
        
        public AccountDownloadEndMessage(string account)
        {
            Type = MessageType.AccountDownloadEnd;
            Account = account;
        }

        public string Account
        {
            get { return account; }
            set { account = value; }
        }
    }
}
