using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBSampleApp.messages;

namespace IBSampleApp.ui
{
    public partial class ComboContractResults : Form
    {
        public ComboContractResults()
        {
            InitializeComponent();
        }

        public void UpdateUI(ContractDetailsMessage message)
        {
            contractResults.Rows.Add(1);
            contractResults[0, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.Symbol;
            contractResults[1, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.Currency;
            contractResults[2, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.Multiplier;
            contractResults[3, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.Strike;
            contractResults[4, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.Right;
            contractResults[5, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.Expiry;
            contractResults[6, contractResults.Rows.Count - 1].Value = message.ContractDetails.Summary.ConId;
        }

        private void contractResultsClose_Click(object sender, EventArgs e)
        {
            contractResults.Rows.Clear();
            this.Visible = false;
        }

        private void addComboLeg_Click(object sender, EventArgs e)
        {
            int conId = (int)contractResults.SelectedRows[0].Cells[6].Value;
            this.Visible = false;
        }
    }
}
