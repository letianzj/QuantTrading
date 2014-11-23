using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using IBApi;
using IBSampleApp.messages;
using System.Globalization;
using System.Windows.Forms;

namespace IBSampleApp.ui
{
    public class HistoricalDataManager : DataManager
    {
        public const int HISTORICAL_ID_BASE = 30000000;

        private string fullDatePattern = "yyyyMMdd  HH:mm:ss";
        private string yearMonthDayPattern = "yyyyMMdd";

        protected int barCounter = -1;
        protected DataGridView gridView;

        private List<HistoricalDataMessage> historicalData;

        public HistoricalDataManager(IBClient ibClient, Chart chart, DataGridView gridView) : base(ibClient, chart) 
        {
            Chart historicalChart = (Chart)uiControl;
            historicalChart.Series[0]["PriceUpColor"] = "Green";
            historicalChart.Series[0]["PriceDownColor"] = "Red";
            this.gridView = gridView;
        }

        public void AddRequest(Contract contract, string endDateTime, string durationString, string barSizeSetting, string whatToShow, int useRTH, int dateFormat)
        {
            Clear();
            ibClient.ClientSocket.reqHistoricalData(currentTicker+HISTORICAL_ID_BASE, contract, endDateTime, durationString, barSizeSetting, whatToShow, useRTH, 1);
        }

        public override void Clear()
        {
            barCounter = -1;
            Chart historicalChart = (Chart)uiControl;
            historicalChart.Series[0].Points.Clear();
            gridView.Rows.Clear();
            historicalData = new List<HistoricalDataMessage>();
        }

        public override void NotifyError(int requestId)
        {
        }

        public override void UpdateUI(IBMessage message)
        {
            switch (message.Type)
            {
                case MessageType.HistoricalData:
                    historicalData.Add((HistoricalDataMessage)message);
                    break;
                case MessageType.HistoricalDataEnd:
                    PaintChart();
                    break;
            }
        }

        private void PaintChart()
        {
            DateTime dt;
            Chart historicalChart = (Chart)uiControl;
            for (int i = 0; i < historicalData.Count; i++)
            {
                if (historicalData[i].Date.Length == fullDatePattern.Length)
                    DateTime.TryParseExact(historicalData[i].Date, fullDatePattern, null, DateTimeStyles.None, out dt);
                else if (historicalData[i].Date.Length == yearMonthDayPattern.Length)
                    DateTime.TryParseExact(historicalData[i].Date, yearMonthDayPattern, null, DateTimeStyles.None, out dt);
                else
                    continue;

                // adding date and high
                historicalChart.Series[0].Points.AddXY(dt, historicalData[i].High);
                // adding low
                historicalChart.Series[0].Points[i].YValues[1] = historicalData[i].Low;
                //adding open
                historicalChart.Series[0].Points[i].YValues[2] = historicalData[i].Open;
                // adding close
                historicalChart.Series[0].Points[i].YValues[3] = historicalData[i].Close;
                PopulateGrid(historicalData[i]);
            }
        }

        protected void PopulateGrid(IBMessage message)
        {
            HistoricalDataMessage bar = (HistoricalDataMessage)message;
            gridView.Rows.Add(1);
            gridView[0, gridView.Rows.Count -1].Value = bar.Date;
            gridView[1, gridView.Rows.Count - 1].Value = bar.Open;
            gridView[2, gridView.Rows.Count - 1].Value = bar.High;
            gridView[3, gridView.Rows.Count - 1].Value = bar.Low;
            gridView[4, gridView.Rows.Count - 1].Value = bar.Close;
            gridView[5, gridView.Rows.Count - 1].Value = bar.Volume;
            gridView[6, gridView.Rows.Count - 1].Value = bar.Wap;
        }
    }
}
