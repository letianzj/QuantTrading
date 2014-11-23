using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBApi;
using IBSampleApp.messages;
using IBSampleApp.types;

namespace IBSampleApp.ui
{
    public class ContractManager
    {
        private IBClient ibClient;
        private TextBox fundamentals;
        private DataGridView contractDetailsGrid;
        private ComboContractResults comboContractResults;

        public const int CONTRACT_ID_BASE = 60000000;
        public const int CONTRACT_DETAILS_ID = CONTRACT_ID_BASE + 1;
        public const int FUNDAMENTALS_ID = CONTRACT_ID_BASE + 2;

        private bool isComboLegRequest = false;

        private bool contractRequestActive = false;
        private bool fundamentalsRequestActive = false;
        
        public ContractManager(IBClient ibClient, TextBox fundamentalsOutput, DataGridView contractDetailsGrid)
        {
            IbClient = ibClient;
            Fundamentals = fundamentalsOutput;
            ContractDetailsGrid = contractDetailsGrid;
            comboContractResults = new ComboContractResults();

        }

        public void UpdateUI(IBMessage message)
        {
            switch (message.Type)
            {
                case MessageType.ContractData:
                    if (isComboLegRequest)
                        comboContractResults.UpdateUI((ContractDetailsMessage)message);
                    else
                        HandleContractMessage((ContractDetailsMessage)message);
                    break;
                case MessageType.ContractDataEnd:
                    HandleContractDataEndMessage((ContractDetailsEndMessage)message);
                    break;
                case MessageType.FundamentalData:
                    HandleFundamentalsData((FundamentalsMessage)message);
                    break;
            }
        }

        public void HandleContractDataEndMessage(ContractDetailsEndMessage contractDetailsEndMessage)
        {
            if (IsComboLegRequest)
                comboContractResults.Show();
            contractRequestActive = false;
            IsComboLegRequest = false;
        }

        public void HandleContractMessage(ContractDetailsMessage contractDetailsMessage)
        {
            ContractDetailsGrid.Rows.Add(1);
            ContractDetailsGrid[0, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Symbol;
            ContractDetailsGrid[1, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.LocalSymbol;
            ContractDetailsGrid[2, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.SecType;
            ContractDetailsGrid[3, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Currency;
            ContractDetailsGrid[4, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Exchange;
            ContractDetailsGrid[5, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.PrimaryExch;
            ContractDetailsGrid[6, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Expiry;
            ContractDetailsGrid[7, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Multiplier;
            ContractDetailsGrid[8, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Strike;
            ContractDetailsGrid[9, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.Right;
            ContractDetailsGrid[10, ContractDetailsGrid.Rows.Count - 1].Value = contractDetailsMessage.ContractDetails.Summary.ConId;
        }

        public void HandleRequestError(int requestId)
        {
            if (requestId == CONTRACT_DETAILS_ID)
            {
                isComboLegRequest = false;
                contractRequestActive = false;
            }
            else if (requestId == FUNDAMENTALS_ID)
                fundamentalsRequestActive = false;
        }

        public void HandleFundamentalsData(FundamentalsMessage fundamentalsMessage)
        {
            fundamentals.Text = fundamentalsMessage.Data;
            fundamentalsRequestActive = false;
        }

        public void RequestContractDetails(Contract contract)
        {
            if (!contractRequestActive)
            {
                contractDetailsGrid.Rows.Clear();
                ibClient.ClientSocket.reqContractDetails(CONTRACT_DETAILS_ID, contract);
            }
        }

        public void RequestFundamentals(Contract contract, string reportType)
        {
            fundamentals.Text = "";
            if (!fundamentalsRequestActive)
            {
                fundamentalsRequestActive = true;
                ibClient.ClientSocket.reqFundamentalData(FUNDAMENTALS_ID, contract, reportType);
            }
            else
            {
                fundamentalsRequestActive = false;
                ibClient.ClientSocket.cancelFundamentalData(FUNDAMENTALS_ID);
            }
        }

        public ComboContractResults ComboContractResults
        {
            get { return comboContractResults; }
            set { comboContractResults = value; }
        }

        public IBClient IbClient
        {
            get { return ibClient; }
            set { ibClient = value; }
        }

        public TextBox Fundamentals
        {
            get { return fundamentals; }
            set { fundamentals = value; }
        }

        public DataGridView ContractDetailsGrid
        {
            get { return contractDetailsGrid; }
            set { contractDetailsGrid = value; }
        }

        public bool IsComboLegRequest
        {
            get { return isComboLegRequest; }
            set { isComboLegRequest = value; }
        }
    }
}
