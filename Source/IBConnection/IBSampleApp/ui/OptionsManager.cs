using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBSampleApp.messages;
using IBApi;
using IBSampleApp.util;

namespace IBSampleApp.ui
{
    public class OptionsManager
    {
        public const int OPTIONS_ID_BASE = 70000000;
        private const int OPTIONS_DATA_CALL_BASE = OPTIONS_ID_BASE + 100000;
        private const int OPTIONS_DATA_PUT_BASE = OPTIONS_ID_BASE + 200000;
        
        private const int OPTIONS_EXERCISING_BASE = OPTIONS_ID_BASE + 1000000;

        //Options' chain table indexes
        private const int EXPIRY_INDEX = 0;
        private const int STRIKE_INDEX = 1;
        private const int BID_INDEX = 2;
        private const int ASK_INDEX = 3;
        private const int IMPLIED_VOLATILITY_INDEX = 4;
        private const int DELTA_INDEX = 5;
        private const int GAMMA_INDEX = 6;
        private const int VEGA_INDEX = 7;
        private const int THETA_INDEX = 8;

        //Options positions table indexes (exercising)
        private const int POS_CONTRACT_IDX = 0;
        private const int POS_ACCOUNT_IDX = 1;
        private const int POS_POSITION_IDX = 2;
        private const int POS_MARKET_PRICE_IDX = 3;
        private const int POS_MARKET_VALUE_IDX = 4;
        private const int POS_AVG_COST_IDX = 5;
        private const int POS_UNREALISED_PNL_IDX = 6;
        private const int POS_REALISED_PNL_IDX = 7;

        private int currentMktDataCallRequest = OPTIONS_DATA_CALL_BASE;
        private int currentMktDataPutRequest = OPTIONS_DATA_PUT_BASE;
        private int currentOptionsExercisingRequest = OPTIONS_EXERCISING_BASE;

        private bool isRequestActive = false;
        
        private string optionsDataExchange;
        private bool useSnapshot = false;
        
        private IBClient ibClient;
        private DataGridView callGrid;
        private DataGridView putGrid;
        private DataGridView positionsGrid;

        private List<Contract> currentOptionsPositions = new List<Contract>();

        public OptionsManager(IBClient ibClient, DataGridView callGrid, DataGridView putGrid, DataGridView optionPositionsGrid)
        {
            this.ibClient = ibClient;
            this.callGrid = callGrid;
            this.putGrid = putGrid;
            this.positionsGrid = optionPositionsGrid;
        }

        public void UpdateUI(IBMessage message)
        {
            if (message is ContractDetailsMessage)
            {
                Contract contract = ((ContractDetailsMessage)message).ContractDetails.Summary;
                if (contract.Right.Equals("C"))
                {
                    int mktDataRequest = currentMktDataCallRequest++;                    
                    ibClient.ClientSocket.reqMktData(mktDataRequest, contract, "", useSnapshot);
                    UpdateContractDetails(callGrid, (mktDataRequest - OPTIONS_DATA_CALL_BASE), contract);
                }
                else
                {
                    int mktDataRequest = currentMktDataPutRequest++;
                    ibClient.ClientSocket.reqMktData(mktDataRequest, contract, "", useSnapshot);
                    UpdateContractDetails(putGrid, (mktDataRequest - OPTIONS_DATA_PUT_BASE), contract);
                }
            }
            else if (message is MarketDataMessage)
            {
                MarketDataMessage mktDataMsg = (MarketDataMessage)message;
                if (mktDataMsg.RequestId < OPTIONS_DATA_PUT_BASE)
                {
                    UpdateOptionGridTick(callGrid, (mktDataMsg.RequestId - OPTIONS_DATA_CALL_BASE), mktDataMsg);
                }
                else
                {
                    UpdateOptionGridTick(putGrid, (mktDataMsg.RequestId - OPTIONS_DATA_PUT_BASE), mktDataMsg);
                }
            }
        }

        public void HandlePosition(UpdatePortfolioMessage positionMessage)
        {
            if (positionMessage.Contract.SecType.Equals("OPT"))
            {
                for (int i = 0; i < positionsGrid.Rows.Count; i++)
                {
                    if (positionsGrid[POS_CONTRACT_IDX, i].Value.Equals(Utils.ContractToString(positionMessage.Contract)))
                    {
                        positionsGrid[POS_ACCOUNT_IDX, i].Value = positionMessage.AccountName;
                        positionsGrid[POS_POSITION_IDX, i].Value = positionMessage.Position;
                        positionsGrid[POS_MARKET_PRICE_IDX, i].Value = positionMessage.MarketPrice;
                        positionsGrid[POS_MARKET_VALUE_IDX, i].Value = positionMessage.MarketValue;
                        positionsGrid[POS_AVG_COST_IDX, i].Value = positionMessage.AverageCost;
                        positionsGrid[POS_UNREALISED_PNL_IDX, i].Value = positionMessage.UnrealisedPNL;
                        positionsGrid[POS_REALISED_PNL_IDX, i].Value = positionMessage.RealisedPNL;
                        return;
                    }
                }

                positionsGrid.Rows.Add(1);
                positionsGrid[0, positionsGrid.Rows.Count - 1].Value = Utils.ContractToString(positionMessage.Contract);
                positionsGrid[1, positionsGrid.Rows.Count - 1].Value = positionMessage.AccountName;
                positionsGrid[2, positionsGrid.Rows.Count - 1].Value = positionMessage.Position;
                positionsGrid[3, positionsGrid.Rows.Count - 1].Value = positionMessage.AverageCost;
                currentOptionsPositions.Add(positionMessage.Contract);
            }
        }

        public void AddOptionChainRequest(Contract contract, string optionExchange, bool useSnapshot)
        {
            Clear();
            IsRequestActive = true;
            this.useSnapshot = useSnapshot;
            this.optionsDataExchange = optionExchange;
            ibClient.ClientSocket.reqContractDetails(OPTIONS_ID_BASE, contract);
        }

        public void Clear()
        {
            IsRequestActive = false;

            for (int i = currentMktDataCallRequest - 1; i >= OPTIONS_DATA_CALL_BASE; i--)
                ibClient.ClientSocket.cancelMktData(i);
            for (int i = currentMktDataPutRequest - 1; i >= OPTIONS_DATA_PUT_BASE; i--)
                ibClient.ClientSocket.cancelMktData(i);

            currentMktDataCallRequest = OPTIONS_DATA_CALL_BASE;
            currentMktDataPutRequest = OPTIONS_DATA_PUT_BASE;
            callGrid.Rows.Clear();
            putGrid.Rows.Clear();
        }

        private void UpdateContractDetails(DataGridView grid, int row, Contract contract)
        {
            grid.Rows.Add();
            grid[EXPIRY_INDEX, row].Value = contract.Expiry;
            grid[STRIKE_INDEX, row].Value = contract.Strike;
        }

        private void UpdateOptionGridTick(DataGridView grid, int row, MarketDataMessage message)
        {
            if (message is TickPriceMessage)
            {
                UpdateOptionGridPrice(grid, row, (TickPriceMessage)message);
            }
            else if (message is TickOptionMessage)
            {
                UpdateOptionGridTickOption(grid, row, (TickOptionMessage)message);
            }
        }

        private void UpdateOptionGridPrice(DataGridView grid, int row, TickPriceMessage message)
        {
            switch (message.Field)
            {
                case TickType.ASK:
                    grid[ASK_INDEX, row].Value = message.Price;
                    break;
                case TickType.BID:
                    grid[BID_INDEX, row].Value = message.Price;
                    break;
            }
        }

       
        private void UpdateOptionGridTickOption(DataGridView grid, int row, TickOptionMessage message)
        {
            grid[IMPLIED_VOLATILITY_INDEX, row].Value = message.ImpliedVolatility;
            grid[DELTA_INDEX, row].Value = message.Delta;
            grid[GAMMA_INDEX, row].Value = message.Gamma;
            grid[VEGA_INDEX, row].Value = message.Vega;
            grid[THETA_INDEX, row].Value = message.Theta;
        }

        public bool IsRequestActive
        {
            get { return isRequestActive; }
            set { isRequestActive = value; }
        }

        public void ExerciseOptions(int ovrd, int quantity, string exchange, int action) 
        {
            if (positionsGrid.SelectedRows.Count > 0 )
            {
                DataGridViewRow selectedRow = positionsGrid.SelectedRows[0];
                int contractIndex = selectedRow.Index;
                string account = (string)selectedRow.Cells[POS_ACCOUNT_IDX].Value;
                Contract contract = currentOptionsPositions[contractIndex];
                contract.Exchange = exchange;
                ibClient.ClientSocket.exerciseOptions(currentOptionsExercisingRequest, contract, action, quantity, account, ovrd);
            }
        }

    }
}
