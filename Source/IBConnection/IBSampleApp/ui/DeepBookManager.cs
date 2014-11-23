using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBSampleApp.messages;
using IBApi;

namespace IBSampleApp.ui
{
    public class DeepBookManager : DataManager
    {
        public const int TICK_ID_BASE = 20000000;

        private int numRows = 3;

        bool isSubscribed = false;

        private const int BID_MAKER_IDX = 0;
        private const int BID_SIZE_IDX = 1;
        private const int BID_PRICE_IDX = 2;

        private const int ASK_PRICE_IDX = 3;
        private const int ASK_SIZE_IDX = 4;
        private const int ASK_MAKER_IDX = 5;
        
        public DeepBookManager(IBClient client, DataGridView dataGrid) : base(client, dataGrid)
        {
            
        }
        
        public void AddRequest(Contract contract, int numEntries)
        {
            numRows = numEntries;
            StopActiveRequests();
            ibClient.ClientSocket.reqMarketDepth(currentTicker + TICK_ID_BASE, contract, numRows);
            isSubscribed = true;
        }

        public override void Clear()
        {
        }

        public void StopActiveRequests()
        {
            if (isSubscribed)
            {
                ibClient.ClientSocket.cancelMktDepth(currentTicker + TICK_ID_BASE);
                ((DataGridView)uiControl).Rows.Clear();
                isSubscribed = false;
            }
        }

        public override void NotifyError(int requestId)
        {
            ((DataGridView)uiControl).Rows.Clear();
            isSubscribed = false;
        }

        public override void UpdateUI(IBMessage message)
        {
            DataGridView grid = (DataGridView)uiControl;
            if (grid.Rows.Count == 0)
                grid.Rows.Add(numRows * 2);

            DeepBookMessage entry = (DeepBookMessage)message;
            if (entry.Side == 1)
            {
                grid[BID_MAKER_IDX, GetBidIndex(entry.Position)].Value = entry.MarketMaker;
                grid[BID_SIZE_IDX, GetBidIndex(entry.Position)].Value = entry.Size;
                grid[BID_PRICE_IDX, GetBidIndex(entry.Position)].Value = entry.Price;
            }
            else
            {
                grid[ASK_MAKER_IDX, GetAskIndex(entry.Position)].Value = entry.MarketMaker;
                grid[ASK_SIZE_IDX, GetAskIndex(entry.Position)].Value = entry.Size;
                grid[ASK_PRICE_IDX, GetAskIndex(entry.Position)].Value = entry.Price;
            }
        }

        private int GetAskIndex(int position)
        {
            return (numRows - 1) - position;
        }

        private int GetBidIndex(int position)
        {
            return numRows + position;
        }
    }
}
