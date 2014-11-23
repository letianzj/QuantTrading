using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBApi;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using IBSampleApp.messages;

namespace IBSampleApp.ui
{
    public class RealTimeBarsManager : HistoricalDataManager
    {
        public const int RT_BARS_ID_BASE = 40000000;

        public RealTimeBarsManager(IBClient ibClient, Chart rtBarsChart, DataGridView rtBarsGrid) : base(ibClient, rtBarsChart, rtBarsGrid)
        {
        }

        public void AddRequest(Contract contract, string whatToShow, bool useRTH)
        {
            Clear();
            //ibClient.ClientSocket.reqHistoricalData(currentTicker, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRTH, 1);
            ibClient.ClientSocket.reqRealTimeBars(currentTicker + RT_BARS_ID_BASE, contract, 5, whatToShow, useRTH);
        }

        public override void Clear()
        {
            ibClient.ClientSocket.cancelRealTimeBars(currentTicker + RT_BARS_ID_BASE);
            base.Clear();
        }

        public override void UpdateUI(IBMessage message)
        {
            barCounter++;
            Chart rtBarsChart = (Chart)uiControl;
            RealTimeBarMessage rtBar = (RealTimeBarMessage)message;

            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0);

            DateTime dt = start.AddMilliseconds(rtBar.Timestamp * 1000).ToLocalTime();

            // adding date and high
            rtBarsChart.Series[0].Points.AddXY(dt, rtBar.High);
            // adding low
            rtBarsChart.Series[0].Points[barCounter].YValues[1] = rtBar.Low;
            //adding open
            rtBarsChart.Series[0].Points[barCounter].YValues[2] = rtBar.Open;
            // adding close
            rtBarsChart.Series[0].Points[barCounter].YValues[3] = rtBar.Close;
            PopulateGrid(message);
        }
    }
}
