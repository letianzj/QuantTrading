namespace IBSampleApp.ui
{
    partial class MarginDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarginDialog));
            this.equityWithLoanLabel = new System.Windows.Forms.Label();
            this.initialMarginLabel = new System.Windows.Forms.Label();
            this.maintenanceMarginLabel = new System.Windows.Forms.Label();
            this.equityWithLoanResult = new System.Windows.Forms.Label();
            this.initialMarginResult = new System.Windows.Forms.Label();
            this.maintenanceMarginResult = new System.Windows.Forms.Label();
            this.acceptMarginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // equityWithLoanLabel
            // 
            this.equityWithLoanLabel.AutoSize = true;
            this.equityWithLoanLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.equityWithLoanLabel.Location = new System.Drawing.Point(36, 9);
            this.equityWithLoanLabel.Name = "equityWithLoanLabel";
            this.equityWithLoanLabel.Size = new System.Drawing.Size(101, 13);
            this.equityWithLoanLabel.TabIndex = 0;
            this.equityWithLoanLabel.Text = "Equity with loan:";
            // 
            // initialMarginLabel
            // 
            this.initialMarginLabel.AutoSize = true;
            this.initialMarginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.initialMarginLabel.Location = new System.Drawing.Point(54, 37);
            this.initialMarginLabel.Name = "initialMarginLabel";
            this.initialMarginLabel.Size = new System.Drawing.Size(83, 13);
            this.initialMarginLabel.TabIndex = 1;
            this.initialMarginLabel.Text = "Initial margin:";
            // 
            // maintenanceMarginLabel
            // 
            this.maintenanceMarginLabel.AutoSize = true;
            this.maintenanceMarginLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maintenanceMarginLabel.Location = new System.Drawing.Point(12, 63);
            this.maintenanceMarginLabel.Name = "maintenanceMarginLabel";
            this.maintenanceMarginLabel.Size = new System.Drawing.Size(125, 13);
            this.maintenanceMarginLabel.TabIndex = 2;
            this.maintenanceMarginLabel.Text = "Maintenance margin:";
            // 
            // equityWithLoanResult
            // 
            this.equityWithLoanResult.AutoSize = true;
            this.equityWithLoanResult.Location = new System.Drawing.Point(143, 9);
            this.equityWithLoanResult.Name = "equityWithLoanResult";
            this.equityWithLoanResult.Size = new System.Drawing.Size(28, 13);
            this.equityWithLoanResult.TabIndex = 3;
            this.equityWithLoanResult.Text = "0.00";
            this.equityWithLoanResult.Visible = false;
            // 
            // initialMarginResult
            // 
            this.initialMarginResult.AutoSize = true;
            this.initialMarginResult.Location = new System.Drawing.Point(143, 37);
            this.initialMarginResult.Name = "initialMarginResult";
            this.initialMarginResult.Size = new System.Drawing.Size(28, 13);
            this.initialMarginResult.TabIndex = 4;
            this.initialMarginResult.Text = "0.00";
            // 
            // maintenanceMarginResult
            // 
            this.maintenanceMarginResult.AutoSize = true;
            this.maintenanceMarginResult.Location = new System.Drawing.Point(143, 63);
            this.maintenanceMarginResult.Name = "maintenanceMarginResult";
            this.maintenanceMarginResult.Size = new System.Drawing.Size(28, 13);
            this.maintenanceMarginResult.TabIndex = 5;
            this.maintenanceMarginResult.Text = "0.00";
            // 
            // acceptMarginButton
            // 
            this.acceptMarginButton.Location = new System.Drawing.Point(146, 95);
            this.acceptMarginButton.Name = "acceptMarginButton";
            this.acceptMarginButton.Size = new System.Drawing.Size(49, 23);
            this.acceptMarginButton.TabIndex = 6;
            this.acceptMarginButton.Text = "OK";
            this.acceptMarginButton.UseVisualStyleBackColor = true;
            this.acceptMarginButton.Click += new System.EventHandler(this.acceptMarginButton_Click);
            // 
            // MarginDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 130);
            this.ControlBox = false;
            this.Controls.Add(this.acceptMarginButton);
            this.Controls.Add(this.maintenanceMarginResult);
            this.Controls.Add(this.initialMarginResult);
            this.Controls.Add(this.equityWithLoanResult);
            this.Controls.Add(this.maintenanceMarginLabel);
            this.Controls.Add(this.initialMarginLabel);
            this.Controls.Add(this.equityWithLoanLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MarginDialog";
            this.Text = "Post-Trade Margin Requirements";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label equityWithLoanLabel;
        private System.Windows.Forms.Label initialMarginLabel;
        private System.Windows.Forms.Label maintenanceMarginLabel;
        private System.Windows.Forms.Label equityWithLoanResult;
        private System.Windows.Forms.Label initialMarginResult;
        private System.Windows.Forms.Label maintenanceMarginResult;
        private System.Windows.Forms.Button acceptMarginButton;
    }
}