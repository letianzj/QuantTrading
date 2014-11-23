using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class ManagedAccountsMessage : IBMessage
    {
        List<string> managedAccounts;

        public ManagedAccountsMessage(string managedAccounts)
        {
            this.managedAccounts = new List<string>(managedAccounts.Split(','));
            Type = MessageType.ManagedAccounts;
        }

        public List<string> ManagedAccounts
        {
            get { return managedAccounts; }
            set { managedAccounts = value; }
        }
    }
}
