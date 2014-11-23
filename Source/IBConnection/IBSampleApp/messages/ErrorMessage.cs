using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBSampleApp
{
    public class ErrorMessage : IBMessage 
    {
        private string message;
        private int errorCode;
        private int requestId;

        public ErrorMessage(int requestId, int errorCode, string message)
        {
            Type = MessageType.Error;
            Message = message;
            RequestId = requestId;
            ErrorCode = errorCode;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        
        public int ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }
        

        public int RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

        public override string ToString()
        {
            return "Error. Request: "+RequestId+", Code: "+ErrorCode+" - "+Message;
        }
       
    }
}
