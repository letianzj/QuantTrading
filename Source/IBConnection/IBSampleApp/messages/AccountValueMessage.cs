using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp.messages
{
    public class AccountValueMessage : IBMessage 
    {
        private string key;
        private string value;
        private string currency;
        private string accountName;

        public AccountValueMessage(string key, string value, string currency, string accountName)
        {
            Type = MessageType.AccountValue;
            Key = key;
            Value = value;
            Currency = currency;
            AccountName = accountName;
        }

        public string Key
        {
            get { return key; }
            set { key = value; }
        }
        
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        
        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }

        public string AccountName
        {
            get { return accountName; }
            set { accountName = value; }
        }

    }
}
