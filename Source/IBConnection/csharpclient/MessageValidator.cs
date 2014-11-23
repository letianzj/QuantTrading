using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Hint. we could separate the message validation from the Socket client class...
namespace IBApi
{
    public class MessageValidator
    {
        private int serverVersion;

        public int ServerVersion
        {
            get { return serverVersion;  }
            set { serverVersion = value; }
        }

        public MessageValidator(int serverVersion)
        {
            ServerVersion = serverVersion;
        }
    }
}
