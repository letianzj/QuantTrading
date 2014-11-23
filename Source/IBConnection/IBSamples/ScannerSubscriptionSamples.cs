using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;

namespace IBSamples
{
    public class ScannerSubscriptionSamples
    {
        public static ScannerSubscription GetScannerSubscription()
        {
            ScannerSubscription scanSub = new ScannerSubscription();
            scanSub.Instrument = "STOCK.EU";
            scanSub.LocationCode = "STK.EU.IBIS";
            scanSub.ScanCode = "HOT_BY_VOLUME";
            return scanSub;
        }
    }
}
