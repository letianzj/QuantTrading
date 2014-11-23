using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace IBSampleApp.messages
{
    public class CommissionMessage : IBMessage
    {
        private CommissionReport commissionReport;

        public CommissionMessage(CommissionReport commissionReport)
        {
            Type = MessageType.CommissionsReport;
            CommissionReport = commissionReport;
        }

        public CommissionReport CommissionReport
        {
            get { return commissionReport; }
            set { commissionReport = value; }
        }
    }
}
