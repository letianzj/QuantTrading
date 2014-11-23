namespace IBSampleApp.ui
{
    partial class ComboContractResults
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contractResultsBox = new System.Windows.Forms.GroupBox();
            this.contractResults = new System.Windows.Forms.DataGridView();
            this.symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currenvy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.multiplier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.strike = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.right = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.expiry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.conId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.addComboLeg = new System.Windows.Forms.Button();
            this.contractResultsClose = new System.Windows.Forms.Button();
            this.contractResultsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contractResults)).BeginInit();
            this.SuspendLayout();
            // 
            // contractResultsBox
            // 
            this.contractResultsBox.Controls.Add(this.contractResults);
            this.contractResultsBox.Location = new System.Drawing.Point(12, 12);
            this.contractResultsBox.Name = "contractResultsBox";
            this.contractResultsBox.Size = new System.Drawing.Size(598, 198);
            this.contractResultsBox.TabIndex = 0;
            this.contractResultsBox.TabStop = false;
            this.contractResultsBox.Text = "Contracts found";
            // 
            // contractResults
            // 
            this.contractResults.AllowUserToAddRows = false;
            this.contractResults.AllowUserToDeleteRows = false;
            this.contractResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.contractResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.symbol,
            this.currenvy,
            this.multiplier,
            this.strike,
            this.right,
            this.expiry,
            this.conId});
            this.contractResults.Location = new System.Drawing.Point(6, 19);
            this.contractResults.Name = "contractResults";
            this.contractResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.contractResults.Size = new System.Drawing.Size(583, 170);
            this.contractResults.TabIndex = 0;
            // 
            // symbol
            // 
            this.symbol.HeaderText = "Symbol";
            this.symbol.Name = "symbol";
            this.symbol.ReadOnly = true;
            this.symbol.Width = 80;
            // 
            // currenvy
            // 
            this.currenvy.HeaderText = "Currency";
            this.currenvy.Name = "currenvy";
            this.currenvy.ReadOnly = true;
            this.currenvy.Width = 60;
            // 
            // multiplier
            // 
            this.multiplier.HeaderText = "Multiplier";
            this.multiplier.Name = "multiplier";
            this.multiplier.ReadOnly = true;
            this.multiplier.Width = 50;
            // 
            // strike
            // 
            this.strike.HeaderText = "Strike";
            this.strike.Name = "strike";
            this.strike.ReadOnly = true;
            this.strike.Width = 80;
            // 
            // right
            // 
            this.right.HeaderText = "Right";
            this.right.Name = "right";
            this.right.ReadOnly = true;
            this.right.Width = 50;
            // 
            // expiry
            // 
            this.expiry.HeaderText = "Expiry";
            this.expiry.Name = "expiry";
            this.expiry.ReadOnly = true;
            // 
            // conId
            // 
            this.conId.HeaderText = "conId";
            this.conId.Name = "conId";
            this.conId.ReadOnly = true;
            // 
            // addComboLeg
            // 
            this.addComboLeg.Location = new System.Drawing.Point(616, 31);
            this.addComboLeg.Name = "addComboLeg";
            this.addComboLeg.Size = new System.Drawing.Size(75, 23);
            this.addComboLeg.TabIndex = 1;
            this.addComboLeg.Text = "Add leg";
            this.addComboLeg.UseVisualStyleBackColor = true;
            this.addComboLeg.Click += new System.EventHandler(this.addComboLeg_Click);
            // 
            // contractResultsClose
            // 
            this.contractResultsClose.Location = new System.Drawing.Point(535, 216);
            this.contractResultsClose.Name = "contractResultsClose";
            this.contractResultsClose.Size = new System.Drawing.Size(75, 23);
            this.contractResultsClose.TabIndex = 2;
            this.contractResultsClose.Text = "Close";
            this.contractResultsClose.UseVisualStyleBackColor = true;
            this.contractResultsClose.Click += new System.EventHandler(this.contractResultsClose_Click);
            // 
            // ComboContractResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 251);
            this.ControlBox = false;
            this.Controls.Add(this.contractResultsClose);
            this.Controls.Add(this.addComboLeg);
            this.Controls.Add(this.contractResultsBox);
            this.Name = "ComboContractResults";
            this.Text = "ComboContractResults";
            this.contractResultsBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.contractResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox contractResultsBox;
        private System.Windows.Forms.Button addComboLeg;
        private System.Windows.Forms.Button contractResultsClose;
        private System.Windows.Forms.DataGridView contractResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn symbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn currenvy;
        private System.Windows.Forms.DataGridViewTextBoxColumn multiplier;
        private System.Windows.Forms.DataGridViewTextBoxColumn strike;
        private System.Windows.Forms.DataGridViewTextBoxColumn right;
        private System.Windows.Forms.DataGridViewTextBoxColumn expiry;
        private System.Windows.Forms.DataGridViewTextBoxColumn conId;
    }
}