using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBApi;
using IBSampleApp.messages;
using IBSampleApp.util;

namespace IBSampleApp.ui
{
    public class ScannerManager : DataManager
    {
        private int rowCounter = -1;

        private const int RANK_IDX = 0;
        private const int CONTRACT_IDX = 1;
        private const int DISTANCE_IDX = 2;
        private const int BENCHMARK_IDX = 3;
        private const int PROJECTION_IDX = 4;
        private const int LEGS_IDX = 5;

        public ScannerManager(IBClient client, DataGridView dataGrid)
            : base(client, dataGrid)
        {
        }

        public override void NotifyError(int requestId)
        {
        }

        public override void UpdateUI(IBMessage message)
        {
            ScannerMessage scannMessage = (ScannerMessage)message;
            DataGridView grid = (DataGridView)uiControl;
            rowCounter++;
            grid.Rows.Add();
            grid[RANK_IDX, rowCounter].Value = scannMessage.Rank;
            grid[CONTRACT_IDX, rowCounter].Value = Utils.ContractToString(scannMessage.ContractDetails.Summary);
            grid[DISTANCE_IDX, rowCounter].Value = scannMessage.Distance;
            grid[BENCHMARK_IDX, rowCounter].Value = scannMessage.Benchmark;
            grid[PROJECTION_IDX, rowCounter].Value = scannMessage.Projection;
            grid[LEGS_IDX, rowCounter].Value = scannMessage.LegsStr;
        }

        public override void Clear()
        {
            ibClient.ClientSocket.cancelScannerSubscription(currentTicker);
            DataGridView grid = (DataGridView)uiControl;
            grid.Rows.Clear();
            rowCounter = -1;
        }

        public void AddRequest(ScannerSubscription scannerSubscription)
        {
            ibClient.ClientSocket.reqScannerSubscription(currentTicker, scannerSubscription);
        }
    }
}
