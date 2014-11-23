namespace IBConnectionTest
{
    partial class IBConnectionTestForm
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
            this.buttonConnect = new System.Windows.Forms.Button();
            this.listBoxData = new System.Windows.Forms.ListBox();
            this.listBoxResponse = new System.Windows.Forms.ListBox();
            this.listBoxError = new System.Windows.Forms.ListBox();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReqMktData = new System.Windows.Forms.Button();
            this.btnCancelMktData = new System.Windows.Forms.Button();
            this.btnReqMktDepth = new System.Windows.Forms.Button();
            this.btnCancelMktDepth = new System.Windows.Forms.Button();
            this.btnHistData = new System.Windows.Forms.Button();
            this.btnCancelHistData = new System.Windows.Forms.Button();
            this.btnRTBars = new System.Windows.Forms.Button();
            this.btnCancelRTBars = new System.Windows.Forms.Button();
            this.btnPlaceOrder = new System.Windows.Forms.Button();
            this.btnCancelOrder = new System.Windows.Forms.Button();
            this.btnTime = new System.Windows.Forms.Button();
            this.btnReqContractData = new System.Windows.Forms.Button();
            this.reqOpenOrder = new System.Windows.Forms.Button();
            this.btnReqAllOpenOrders = new System.Windows.Forms.Button();
            this.btnReqAutoOpenOrders = new System.Windows.Forms.Button();
            this.btnReqAccountData = new System.Windows.Forms.Button();
            this.btnReqExecution = new System.Windows.Forms.Button();
            this.btnReqNextId = new System.Windows.Forms.Button();
            this.btnReqAccounts = new System.Windows.Forms.Button();
            this.btnGlobalCancel = new System.Windows.Forms.Button();
            this.btnReqMktDataType = new System.Windows.Forms.Button();
            this.btnMarketScanner = new System.Windows.Forms.Button();
            this.btnCancelImpliedVol = new System.Windows.Forms.Button();
            this.btnImpliedVol = new System.Windows.Forms.Button();
            this.btnCancelOptionPrice = new System.Windows.Forms.Button();
            this.btnCalcOptionPrice = new System.Windows.Forms.Button();
            this.btnWhatIf = new System.Windows.Forms.Button();
            this.btnExerciseOptions = new System.Windows.Forms.Button();
            this.btnExtended = new System.Windows.Forms.Button();
            this.btnNewsBulletins = new System.Windows.Forms.Button();
            this.btnLogConfig = new System.Windows.Forms.Button();
            this.btnFA = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(196, 12);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(114, 23);
            this.buttonConnect.TabIndex = 0;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // listBoxData
            // 
            this.listBoxData.FormattingEnabled = true;
            this.listBoxData.HorizontalScrollbar = true;
            this.listBoxData.Location = new System.Drawing.Point(24, 50);
            this.listBoxData.Name = "listBoxData";
            this.listBoxData.Size = new System.Drawing.Size(516, 316);
            this.listBoxData.TabIndex = 1;
            // 
            // listBoxResponse
            // 
            this.listBoxResponse.FormattingEnabled = true;
            this.listBoxResponse.HorizontalScrollbar = true;
            this.listBoxResponse.Location = new System.Drawing.Point(24, 396);
            this.listBoxResponse.Name = "listBoxResponse";
            this.listBoxResponse.Size = new System.Drawing.Size(516, 238);
            this.listBoxResponse.TabIndex = 2;
            // 
            // listBoxError
            // 
            this.listBoxError.FormattingEnabled = true;
            this.listBoxError.HorizontalScrollbar = true;
            this.listBoxError.Location = new System.Drawing.Point(24, 669);
            this.listBoxError.Name = "listBoxError";
            this.listBoxError.Size = new System.Drawing.Size(516, 238);
            this.listBoxError.TabIndex = 3;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(170, 930);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(90, 23);
            this.buttonClear.TabIndex = 4;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(355, 930);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(99, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Market and Historical Data";
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(329, 12);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(125, 23);
            this.buttonDisconnect.TabIndex = 7;
            this.buttonDisconnect.Text = "Disconnect";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 374);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "TWS Server Responses";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 645);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Errors and Message";
            // 
            // btnReqMktData
            // 
            this.btnReqMktData.Location = new System.Drawing.Point(565, 31);
            this.btnReqMktData.Name = "btnReqMktData";
            this.btnReqMktData.Size = new System.Drawing.Size(162, 23);
            this.btnReqMktData.TabIndex = 10;
            this.btnReqMktData.Text = "Req Mkt Data ...";
            this.btnReqMktData.UseVisualStyleBackColor = true;
            this.btnReqMktData.Click += new System.EventHandler(this.btnReqMktData_Click);
            // 
            // btnCancelMktData
            // 
            this.btnCancelMktData.Location = new System.Drawing.Point(565, 60);
            this.btnCancelMktData.Name = "btnCancelMktData";
            this.btnCancelMktData.Size = new System.Drawing.Size(162, 23);
            this.btnCancelMktData.TabIndex = 11;
            this.btnCancelMktData.Text = "Cancel Mkt Data ...";
            this.btnCancelMktData.UseVisualStyleBackColor = true;
            this.btnCancelMktData.Click += new System.EventHandler(this.btnCancelMktData_Click);
            // 
            // btnReqMktDepth
            // 
            this.btnReqMktDepth.Location = new System.Drawing.Point(565, 89);
            this.btnReqMktDepth.Name = "btnReqMktDepth";
            this.btnReqMktDepth.Size = new System.Drawing.Size(162, 23);
            this.btnReqMktDepth.TabIndex = 12;
            this.btnReqMktDepth.Text = "Req Mkt Depth ...";
            this.btnReqMktDepth.UseVisualStyleBackColor = true;
            this.btnReqMktDepth.Click += new System.EventHandler(this.btnReqMktDepth_Click);
            // 
            // btnCancelMktDepth
            // 
            this.btnCancelMktDepth.Location = new System.Drawing.Point(565, 118);
            this.btnCancelMktDepth.Name = "btnCancelMktDepth";
            this.btnCancelMktDepth.Size = new System.Drawing.Size(162, 23);
            this.btnCancelMktDepth.TabIndex = 13;
            this.btnCancelMktDepth.Text = "Cancel Mkt Depth ...";
            this.btnCancelMktDepth.UseVisualStyleBackColor = true;
            this.btnCancelMktDepth.Click += new System.EventHandler(this.btnCancelMktDepth_Click);
            // 
            // btnHistData
            // 
            this.btnHistData.Location = new System.Drawing.Point(565, 147);
            this.btnHistData.Name = "btnHistData";
            this.btnHistData.Size = new System.Drawing.Size(162, 23);
            this.btnHistData.TabIndex = 14;
            this.btnHistData.Text = "Historical Data ...";
            this.btnHistData.UseVisualStyleBackColor = true;
            this.btnHistData.Click += new System.EventHandler(this.btnHistData_Click);
            // 
            // btnCancelHistData
            // 
            this.btnCancelHistData.Location = new System.Drawing.Point(565, 176);
            this.btnCancelHistData.Name = "btnCancelHistData";
            this.btnCancelHistData.Size = new System.Drawing.Size(162, 23);
            this.btnCancelHistData.TabIndex = 15;
            this.btnCancelHistData.Text = "Cancel Hist Data ...";
            this.btnCancelHistData.UseVisualStyleBackColor = true;
            this.btnCancelHistData.Click += new System.EventHandler(this.btnCancelHistData_Click);
            // 
            // btnRTBars
            // 
            this.btnRTBars.Location = new System.Drawing.Point(565, 205);
            this.btnRTBars.Name = "btnRTBars";
            this.btnRTBars.Size = new System.Drawing.Size(162, 23);
            this.btnRTBars.TabIndex = 16;
            this.btnRTBars.Text = "Real Time Bars";
            this.btnRTBars.UseVisualStyleBackColor = true;
            this.btnRTBars.Click += new System.EventHandler(this.btnRTBars_Click);
            // 
            // btnCancelRTBars
            // 
            this.btnCancelRTBars.Location = new System.Drawing.Point(565, 234);
            this.btnCancelRTBars.Name = "btnCancelRTBars";
            this.btnCancelRTBars.Size = new System.Drawing.Size(162, 23);
            this.btnCancelRTBars.TabIndex = 17;
            this.btnCancelRTBars.Text = "Cancel Real Time Bars";
            this.btnCancelRTBars.UseVisualStyleBackColor = true;
            this.btnCancelRTBars.Click += new System.EventHandler(this.btnCancelRTBars_Click);
            // 
            // btnPlaceOrder
            // 
            this.btnPlaceOrder.Location = new System.Drawing.Point(565, 466);
            this.btnPlaceOrder.Name = "btnPlaceOrder";
            this.btnPlaceOrder.Size = new System.Drawing.Size(162, 23);
            this.btnPlaceOrder.TabIndex = 18;
            this.btnPlaceOrder.Text = "Place Order ...";
            this.btnPlaceOrder.UseVisualStyleBackColor = true;
            this.btnPlaceOrder.Click += new System.EventHandler(this.btnPlaceOrder_Click);
            // 
            // btnCancelOrder
            // 
            this.btnCancelOrder.Location = new System.Drawing.Point(565, 495);
            this.btnCancelOrder.Name = "btnCancelOrder";
            this.btnCancelOrder.Size = new System.Drawing.Size(162, 23);
            this.btnCancelOrder.TabIndex = 19;
            this.btnCancelOrder.Text = "Cancel Order ...";
            this.btnCancelOrder.UseVisualStyleBackColor = true;
            this.btnCancelOrder.Click += new System.EventHandler(this.btnCancelOrder_Click);
            // 
            // btnTime
            // 
            this.btnTime.Location = new System.Drawing.Point(565, 263);
            this.btnTime.Name = "btnTime";
            this.btnTime.Size = new System.Drawing.Size(162, 23);
            this.btnTime.TabIndex = 20;
            this.btnTime.Text = "Current Time";
            this.btnTime.UseVisualStyleBackColor = true;
            this.btnTime.Click += new System.EventHandler(this.btnTime_Click);
            // 
            // btnReqContractData
            // 
            this.btnReqContractData.Location = new System.Drawing.Point(565, 582);
            this.btnReqContractData.Name = "btnReqContractData";
            this.btnReqContractData.Size = new System.Drawing.Size(162, 23);
            this.btnReqContractData.TabIndex = 21;
            this.btnReqContractData.Text = "Request Contract Data";
            this.btnReqContractData.UseVisualStyleBackColor = true;
            this.btnReqContractData.Click += new System.EventHandler(this.btnReqContractData_Click);
            // 
            // reqOpenOrder
            // 
            this.reqOpenOrder.Location = new System.Drawing.Point(565, 611);
            this.reqOpenOrder.Name = "reqOpenOrder";
            this.reqOpenOrder.Size = new System.Drawing.Size(162, 23);
            this.reqOpenOrder.TabIndex = 22;
            this.reqOpenOrder.Text = "Req Open Order";
            this.reqOpenOrder.UseVisualStyleBackColor = true;
            this.reqOpenOrder.Click += new System.EventHandler(this.reqOpenOrder_Click);
            // 
            // btnReqAllOpenOrders
            // 
            this.btnReqAllOpenOrders.Location = new System.Drawing.Point(565, 640);
            this.btnReqAllOpenOrders.Name = "btnReqAllOpenOrders";
            this.btnReqAllOpenOrders.Size = new System.Drawing.Size(162, 23);
            this.btnReqAllOpenOrders.TabIndex = 23;
            this.btnReqAllOpenOrders.Text = "Req All Open Order";
            this.btnReqAllOpenOrders.UseVisualStyleBackColor = true;
            this.btnReqAllOpenOrders.Click += new System.EventHandler(this.btnReqAllOpenOrders_Click);
            // 
            // btnReqAutoOpenOrders
            // 
            this.btnReqAutoOpenOrders.Location = new System.Drawing.Point(565, 669);
            this.btnReqAutoOpenOrders.Name = "btnReqAutoOpenOrders";
            this.btnReqAutoOpenOrders.Size = new System.Drawing.Size(162, 23);
            this.btnReqAutoOpenOrders.TabIndex = 24;
            this.btnReqAutoOpenOrders.Text = "Req Auto Open Order";
            this.btnReqAutoOpenOrders.UseVisualStyleBackColor = true;
            this.btnReqAutoOpenOrders.Click += new System.EventHandler(this.btnReqAutoOpenOrders_Click);
            // 
            // btnReqAccountData
            // 
            this.btnReqAccountData.Location = new System.Drawing.Point(565, 698);
            this.btnReqAccountData.Name = "btnReqAccountData";
            this.btnReqAccountData.Size = new System.Drawing.Size(162, 23);
            this.btnReqAccountData.TabIndex = 25;
            this.btnReqAccountData.Text = "Req Account Data ...";
            this.btnReqAccountData.UseVisualStyleBackColor = true;
            this.btnReqAccountData.Click += new System.EventHandler(this.btnReqAccountData_Click);
            // 
            // btnReqExecution
            // 
            this.btnReqExecution.Location = new System.Drawing.Point(565, 727);
            this.btnReqExecution.Name = "btnReqExecution";
            this.btnReqExecution.Size = new System.Drawing.Size(162, 23);
            this.btnReqExecution.TabIndex = 26;
            this.btnReqExecution.Text = "Req Executions";
            this.btnReqExecution.UseVisualStyleBackColor = true;
            this.btnReqExecution.Click += new System.EventHandler(this.btnReqExecution_Click);
            // 
            // btnReqNextId
            // 
            this.btnReqNextId.Location = new System.Drawing.Point(565, 756);
            this.btnReqNextId.Name = "btnReqNextId";
            this.btnReqNextId.Size = new System.Drawing.Size(162, 23);
            this.btnReqNextId.TabIndex = 27;
            this.btnReqNextId.Text = "Req Next Id";
            this.btnReqNextId.UseVisualStyleBackColor = true;
            this.btnReqNextId.Click += new System.EventHandler(this.btnReqNextId_Click);
            // 
            // btnReqAccounts
            // 
            this.btnReqAccounts.Location = new System.Drawing.Point(565, 843);
            this.btnReqAccounts.Name = "btnReqAccounts";
            this.btnReqAccounts.Size = new System.Drawing.Size(162, 23);
            this.btnReqAccounts.TabIndex = 28;
            this.btnReqAccounts.Text = "Req Accounts";
            this.btnReqAccounts.UseVisualStyleBackColor = true;
            this.btnReqAccounts.Click += new System.EventHandler(this.btnReqAccounts_Click);
            // 
            // btnGlobalCancel
            // 
            this.btnGlobalCancel.Location = new System.Drawing.Point(565, 901);
            this.btnGlobalCancel.Name = "btnGlobalCancel";
            this.btnGlobalCancel.Size = new System.Drawing.Size(162, 23);
            this.btnGlobalCancel.TabIndex = 29;
            this.btnGlobalCancel.Text = "Global Cancel";
            this.btnGlobalCancel.UseVisualStyleBackColor = true;
            this.btnGlobalCancel.Click += new System.EventHandler(this.btnGlobalCancel_Click);
            // 
            // btnReqMktDataType
            // 
            this.btnReqMktDataType.Location = new System.Drawing.Point(565, 930);
            this.btnReqMktDataType.Name = "btnReqMktDataType";
            this.btnReqMktDataType.Size = new System.Drawing.Size(162, 23);
            this.btnReqMktDataType.TabIndex = 30;
            this.btnReqMktDataType.Text = "Req Mkt Data Type";
            this.btnReqMktDataType.UseVisualStyleBackColor = true;
            this.btnReqMktDataType.Click += new System.EventHandler(this.btnReqMktDataType_Click);
            // 
            // btnMarketScanner
            // 
            this.btnMarketScanner.Enabled = false;
            this.btnMarketScanner.Location = new System.Drawing.Point(565, 292);
            this.btnMarketScanner.Name = "btnMarketScanner";
            this.btnMarketScanner.Size = new System.Drawing.Size(162, 23);
            this.btnMarketScanner.TabIndex = 31;
            this.btnMarketScanner.Text = "Market Scanner ...";
            this.btnMarketScanner.UseVisualStyleBackColor = true;
            this.btnMarketScanner.Click += new System.EventHandler(this.btnMarketScanner_Click);
            // 
            // btnCancelImpliedVol
            // 
            this.btnCancelImpliedVol.Enabled = false;
            this.btnCancelImpliedVol.Location = new System.Drawing.Point(565, 350);
            this.btnCancelImpliedVol.Name = "btnCancelImpliedVol";
            this.btnCancelImpliedVol.Size = new System.Drawing.Size(162, 23);
            this.btnCancelImpliedVol.TabIndex = 33;
            this.btnCancelImpliedVol.Text = "Cancel Calc Impl Vol";
            this.btnCancelImpliedVol.UseVisualStyleBackColor = true;
            // 
            // btnImpliedVol
            // 
            this.btnImpliedVol.Enabled = false;
            this.btnImpliedVol.Location = new System.Drawing.Point(565, 321);
            this.btnImpliedVol.Name = "btnImpliedVol";
            this.btnImpliedVol.Size = new System.Drawing.Size(162, 23);
            this.btnImpliedVol.TabIndex = 32;
            this.btnImpliedVol.Text = "Calc Implied Vol";
            this.btnImpliedVol.UseVisualStyleBackColor = true;
            // 
            // btnCancelOptionPrice
            // 
            this.btnCancelOptionPrice.Enabled = false;
            this.btnCancelOptionPrice.Location = new System.Drawing.Point(565, 408);
            this.btnCancelOptionPrice.Name = "btnCancelOptionPrice";
            this.btnCancelOptionPrice.Size = new System.Drawing.Size(162, 23);
            this.btnCancelOptionPrice.TabIndex = 35;
            this.btnCancelOptionPrice.Text = "Cancel Calc Option Price";
            this.btnCancelOptionPrice.UseVisualStyleBackColor = true;
            // 
            // btnCalcOptionPrice
            // 
            this.btnCalcOptionPrice.Enabled = false;
            this.btnCalcOptionPrice.Location = new System.Drawing.Point(565, 379);
            this.btnCalcOptionPrice.Name = "btnCalcOptionPrice";
            this.btnCalcOptionPrice.Size = new System.Drawing.Size(162, 23);
            this.btnCalcOptionPrice.TabIndex = 34;
            this.btnCalcOptionPrice.Text = "Calc Option Price";
            this.btnCalcOptionPrice.UseVisualStyleBackColor = true;
            // 
            // btnWhatIf
            // 
            this.btnWhatIf.Location = new System.Drawing.Point(565, 437);
            this.btnWhatIf.Name = "btnWhatIf";
            this.btnWhatIf.Size = new System.Drawing.Size(162, 23);
            this.btnWhatIf.TabIndex = 36;
            this.btnWhatIf.Text = "What If ...";
            this.btnWhatIf.UseVisualStyleBackColor = true;
            this.btnWhatIf.Click += new System.EventHandler(this.btnWhatIf_Click);
            // 
            // btnExerciseOptions
            // 
            this.btnExerciseOptions.Location = new System.Drawing.Point(565, 524);
            this.btnExerciseOptions.Name = "btnExerciseOptions";
            this.btnExerciseOptions.Size = new System.Drawing.Size(162, 23);
            this.btnExerciseOptions.TabIndex = 37;
            this.btnExerciseOptions.Text = "Excercise Options ...";
            this.btnExerciseOptions.UseVisualStyleBackColor = true;
            this.btnExerciseOptions.Click += new System.EventHandler(this.btnExerciseOptions_Click);
            // 
            // btnExtended
            // 
            this.btnExtended.Enabled = false;
            this.btnExtended.Location = new System.Drawing.Point(565, 553);
            this.btnExtended.Name = "btnExtended";
            this.btnExtended.Size = new System.Drawing.Size(162, 23);
            this.btnExtended.TabIndex = 38;
            this.btnExtended.Text = "Extended";
            this.btnExtended.UseVisualStyleBackColor = true;
            this.btnExtended.Click += new System.EventHandler(this.btnExtended_Click);
            // 
            // btnNewsBulletins
            // 
            this.btnNewsBulletins.Location = new System.Drawing.Point(565, 785);
            this.btnNewsBulletins.Name = "btnNewsBulletins";
            this.btnNewsBulletins.Size = new System.Drawing.Size(162, 23);
            this.btnNewsBulletins.TabIndex = 39;
            this.btnNewsBulletins.Text = "News Bulletins ...";
            this.btnNewsBulletins.UseVisualStyleBackColor = true;
            this.btnNewsBulletins.Click += new System.EventHandler(this.btnNewsBulletins_Click);
            // 
            // btnLogConfig
            // 
            this.btnLogConfig.Location = new System.Drawing.Point(565, 814);
            this.btnLogConfig.Name = "btnLogConfig";
            this.btnLogConfig.Size = new System.Drawing.Size(162, 23);
            this.btnLogConfig.TabIndex = 40;
            this.btnLogConfig.Text = "Log Configuration";
            this.btnLogConfig.UseVisualStyleBackColor = true;
            this.btnLogConfig.Click += new System.EventHandler(this.btnLogConfig_Click);
            // 
            // btnFA
            // 
            this.btnFA.Location = new System.Drawing.Point(565, 872);
            this.btnFA.Name = "btnFA";
            this.btnFA.Size = new System.Drawing.Size(162, 23);
            this.btnFA.TabIndex = 41;
            this.btnFA.Text = "Financial Advisor";
            this.btnFA.UseVisualStyleBackColor = true;
            this.btnFA.Click += new System.EventHandler(this.btnFA_Click);
            // 
            // IBConnectionTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(748, 974);
            this.Controls.Add(this.btnFA);
            this.Controls.Add(this.btnLogConfig);
            this.Controls.Add(this.btnNewsBulletins);
            this.Controls.Add(this.btnExtended);
            this.Controls.Add(this.btnExerciseOptions);
            this.Controls.Add(this.btnWhatIf);
            this.Controls.Add(this.btnCancelOptionPrice);
            this.Controls.Add(this.btnCalcOptionPrice);
            this.Controls.Add(this.btnCancelImpliedVol);
            this.Controls.Add(this.btnImpliedVol);
            this.Controls.Add(this.btnMarketScanner);
            this.Controls.Add(this.btnReqMktDataType);
            this.Controls.Add(this.btnGlobalCancel);
            this.Controls.Add(this.btnReqAccounts);
            this.Controls.Add(this.btnReqNextId);
            this.Controls.Add(this.btnReqExecution);
            this.Controls.Add(this.btnReqAccountData);
            this.Controls.Add(this.btnReqAutoOpenOrders);
            this.Controls.Add(this.btnReqAllOpenOrders);
            this.Controls.Add(this.reqOpenOrder);
            this.Controls.Add(this.btnReqContractData);
            this.Controls.Add(this.btnTime);
            this.Controls.Add(this.btnCancelOrder);
            this.Controls.Add(this.btnPlaceOrder);
            this.Controls.Add(this.btnCancelRTBars);
            this.Controls.Add(this.btnRTBars);
            this.Controls.Add(this.btnCancelHistData);
            this.Controls.Add(this.btnHistData);
            this.Controls.Add(this.btnCancelMktDepth);
            this.Controls.Add(this.btnReqMktDepth);
            this.Controls.Add(this.btnCancelMktData);
            this.Controls.Add(this.btnReqMktData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.listBoxError);
            this.Controls.Add(this.listBoxResponse);
            this.Controls.Add(this.listBoxData);
            this.Controls.Add(this.buttonConnect);
            this.Name = "IBConnectionTestForm";
            this.Text = "IBConnection Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.ListBox listBoxData;
        private System.Windows.Forms.ListBox listBoxResponse;
        private System.Windows.Forms.ListBox listBoxError;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReqMktData;
        private System.Windows.Forms.Button btnCancelMktData;
        private System.Windows.Forms.Button btnReqMktDepth;
        private System.Windows.Forms.Button btnCancelMktDepth;
        private System.Windows.Forms.Button btnHistData;
        private System.Windows.Forms.Button btnCancelHistData;
        private System.Windows.Forms.Button btnRTBars;
        private System.Windows.Forms.Button btnCancelRTBars;
        private System.Windows.Forms.Button btnPlaceOrder;
        private System.Windows.Forms.Button btnCancelOrder;
        private System.Windows.Forms.Button btnTime;
        private System.Windows.Forms.Button btnReqContractData;
        private System.Windows.Forms.Button reqOpenOrder;
        private System.Windows.Forms.Button btnReqAllOpenOrders;
        private System.Windows.Forms.Button btnReqAutoOpenOrders;
        private System.Windows.Forms.Button btnReqAccountData;
        private System.Windows.Forms.Button btnReqExecution;
        private System.Windows.Forms.Button btnReqNextId;
        private System.Windows.Forms.Button btnReqAccounts;
        private System.Windows.Forms.Button btnGlobalCancel;
        private System.Windows.Forms.Button btnReqMktDataType;
        private System.Windows.Forms.Button btnMarketScanner;
        private System.Windows.Forms.Button btnCancelImpliedVol;
        private System.Windows.Forms.Button btnImpliedVol;
        private System.Windows.Forms.Button btnCancelOptionPrice;
        private System.Windows.Forms.Button btnCalcOptionPrice;
        private System.Windows.Forms.Button btnWhatIf;
        private System.Windows.Forms.Button btnExerciseOptions;
        private System.Windows.Forms.Button btnExtended;
        private System.Windows.Forms.Button btnNewsBulletins;
        private System.Windows.Forms.Button btnLogConfig;
        private System.Windows.Forms.Button btnFA;
    }
}

