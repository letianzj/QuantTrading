using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IBApi;
using IBSampleApp.ui;
using IBSampleApp.messages;
using IBSampleApp.types;

namespace IBSampleApp
{
    public partial class OrderDialog : Form
    {
        private MarginDialog marginDialog;
        private OrderManager orderManager;
        private int orderId;

        public OrderDialog(OrderManager orderManager)
        {
            InitializeComponent();
            InitialiseDropDowns();
            this.orderManager = orderManager;
            marginDialog = new MarginDialog();
        }

        public Order CurrentOrder
        {
            set { SetOrder(value); }
        }

        private void sendOrderButton_Click(object sender, EventArgs e)
        {            
            Contract contract = GetOrderContract();
            Order order = GetOrder();
            orderManager.PlaceOrder(contract, order);
            if (orderId != 0)
                orderId = 0;
            this.Visible = false;
        }

        private void checkMarginButton_Click(object sender, EventArgs e)
        {
            Contract contract = GetOrderContract();
            Order order = GetOrder();
            order.WhatIf = true;
            orderManager.PlaceOrder(contract, order);
        }

        public void HandleIncomingMessage(IBMessage message)
        {
            ProcessMessage(message);
        }

        private void ProcessMessage(IBMessage message)
        {
            switch (message.Type)
            {
                case MessageType.OpenOrder:
                    HandleOpenOrder((OpenOrderMessage)message);
                    break;
            }
        }

        private void HandleOpenOrder(OpenOrderMessage openOrderMessage)
        {
            if (openOrderMessage.Order.WhatIf)
                this.marginDialog.UpdateMarginInformation(openOrderMessage.OrderState);
        }

        private void closeOrderDialogButton_Click(object sender, EventArgs e)
        {
            if (orderId != 0)
                orderId = 0;
            this.Visible = false;
        }

        private void AlgoStrategy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox algoStrategyCombo = (ComboBox)sender;
            string selectedStrategy = (string)algoStrategyCombo.SelectedItem;
            DisableAlgoParams();
            switch (selectedStrategy)
            {
                case "Vwap":
                    EnableVWap();
                    break;
                case "Twap":
                    EnableTWap();
                    break;
                case "ArrivalPx":
                    EnableArrivalPx();
                    break;
                case "DarkIce":
                    EnableDarkIce();
                    break;
                case "PctVol":
                    EnablePctVol();
                    break;
            }
        }

        private void DisableAlgoParams()
        {
            startTime.Enabled = false;
            endTime.Enabled = false;
            allowPastEndTime.Enabled = false;
            maxPctVol.Enabled = false;
            pctVol.Enabled = false;
            strategyType.Enabled = false;
            noTakeLiq.Enabled = false;
            riskAversion.Enabled = false;
            forceCompletion.Enabled = false;
            displaySizeAlgo.Enabled = false;
            getDone.Enabled = false;
            noTradeAhead.Enabled = false;
            useOddLots.Enabled = false;
        }

        public void SetManagedAccounts(List<string> managedAccounts)
        {
            account.Items.AddRange(managedAccounts.ToArray());
            account.SelectedIndex = 0;
        }

        private Contract GetOrderContract()
        {
            Contract contract = new Contract();
            contract.Symbol = contractSymbol.Text;
            contract.SecType = contractSecType.Text;
            contract.Currency = contractCurrency.Text;
            contract.Exchange = contractExchange.Text;
            contract.Expiry = contractExpiry.Text;
            if(!contractStrike.Text.Equals(""))
                contract.Strike = Double.Parse(contractStrike.Text);
            if (!contractRight.Text.Equals("") || !contractRight.Text.Equals("None"))
                contract.Right = (string)((IBType)contractRight.SelectedItem).Value;
            contract.LocalSymbol = contractLocalSymbol.Text;
            return contract;
        }

        public void SetOrderContract(Contract contract)
        {
            contractSymbol.Text = contract.Symbol;
            contractSecType.Text = contract.SecType;
            contractCurrency.Text = contract.Currency;
            contractExchange.Text = contract.Exchange;
            contractExpiry.Text = contract.Expiry;
            contractStrike.Text = contract.Strike.ToString();
            contractRight.Text = contract.Right;
            contractLocalSymbol.Text = contract.LocalSymbol;
        }

        private Order GetOrder()
        {
            Order order = new Order();
            if (orderId != 0)
                order.OrderId = orderId;
            order.Action = action.Text;
            order.OrderType = orderType.Text;
            if(!lmtPrice.Text.Equals(""))
                order.LmtPrice = Double.Parse(lmtPrice.Text);
            if(!quantity.Text.Equals(""))
                order.TotalQuantity = Int32.Parse(quantity.Text);
            order.Account = account.Text;
            order.Tif = timeInForce.Text;
            if (!auxPrice.Text.Equals(""))
                order.AuxPrice = Double.Parse(auxPrice.Text);
            if (!displaySize.Text.Equals(""))
                order.DisplaySize = Int32.Parse(displaySize.Text);
            order = GetExtendedOrderAttributes(order);
            order = GetAdvisorAttributes(order);
            order = GetVolatilityAttributes(order);
            order = GetScaleAttributes(order);
            order = GetAlgoAttributes(order);
            return order;
        }

        private Order GetExtendedOrderAttributes(Order order)
        {
            order.OrderRef = orderReference.Text;
            if (!minQty.Text.Equals(""))
                order.MinQty = Int32.Parse(minQty.Text);
            order.GoodAfterTime = goodAfter.Text;
            order.GoodTillDate = goodUntil.Text;
            order.Rule80A = (string)((IBType)rule80A.SelectedItem).Value;
            order.TriggerMethod = (int)((IBType)triggerMethod.SelectedItem).Value;

            if(!percentOffset.Text.Equals(""))
                order.PercentOffset = Double.Parse(percentOffset.Text);
            if (!trailStopPrice.Text.Equals(""))
                order.TrailStopPrice = Double.Parse(trailStopPrice.Text);
            if (!trailingPercent.Text.Equals(""))
                order.TrailingPercent = Double.Parse(trailingPercent.Text);
            if (!discretionaryAmount.Text.Equals(""))
                order.DiscretionaryAmt = Int32.Parse(discretionaryAmount.Text);
            if (!nbboPriceCap.Text.Equals(""))
                order.NbboPriceCap = Double.Parse(nbboPriceCap.Text);

            order.OcaGroup = ocaGroup.Text;
            order.OcaType = (int)((IBType)ocaType.SelectedItem).Value;
            order.HedgeType = (string)((IBType)hedgeType.SelectedItem).Value;
            order.HedgeParam = hedgeParam.Text;
                        
            order.NotHeld = notHeld.Checked;
            order.BlockOrder = block.Checked;
            order.SweepToFill = sweepToFill.Checked;
            order.Hidden = hidden.Checked;
            order.OutsideRth = outsideRTH.Checked;
            order.AllOrNone = allOrNone.Checked;
            order.OverridePercentageConstraints = overrideConstraints.Checked;
            order.ETradeOnly = eTrade.Checked;
            order.FirmQuoteOnly = firmQuote.Checked;
            order.OptOutSmartRouting = optOutSmart.Checked;
            order.Transmit = transmit.Checked;

            return order;
        }

        private Order GetVolatilityAttributes(Order order)
        {
            if(!volatility.Text.Equals(""))
                order.Volatility = Double.Parse(volatility.Text);
            order.VolatilityType = (int)((IBType)volatilityType.SelectedItem).Value;
            if(continuousUpdate.Checked)
                order.ContinuousUpdate = 1;
            else
                order.ContinuousUpdate = 0;
            order.ReferencePriceType = (int)((IBType)optionReferencePrice.SelectedItem).Value;
            
            if(!deltaNeutralOrderType.Text.Equals("None"))
                order.DeltaNeutralOrderType = deltaNeutralOrderType.Text;

            if (!deltaNeutralAuxPrice.Text.Equals(""))
                order.DeltaNeutralAuxPrice = Double.Parse(deltaNeutralAuxPrice.Text);
            if (!deltaNeutralConId.Text.Equals(""))
                order.DeltaNeutralConId = Int32.Parse(deltaNeutralConId.Text);
            if (!stockRangeLower.Text.Equals(""))
                order.StockRangeLower = Double.Parse(stockRangeLower.Text);
            if (!stockRangeUpper.Text.Equals(""))
                order.StockRangeUpper = Double.Parse(stockRangeUpper.Text);
            return order;
        }

        private Order GetAdvisorAttributes(Order order)
        {
            order.FaGroup = faGroup.Text;
            order.FaPercentage = faPercentage.Text;
            order.FaMethod = (string)((IBType)faMethod.SelectedItem).Value;
            order.FaProfile = faProfile.Text;
            return order;
        }

        private Order GetScaleAttributes(Order order)
        {
            if(!initialLevelSize.Text.Equals(""))
                order.ScaleInitLevelSize = Int32.Parse(initialLevelSize.Text);
            if (!subsequentLevelSize.Text.Equals(""))
                order.ScaleSubsLevelSize = Int32.Parse(subsequentLevelSize.Text);
            if(!priceIncrement.Text.Equals(""))
                order.ScalePriceIncrement = Double.Parse(priceIncrement.Text);
            if (!priceAdjustValue.Text.Equals(""))
                order.ScalePriceAdjustValue = Double.Parse(priceAdjustValue.Text);
            if (!priceAdjustInterval.Text.Equals(""))
                order.ScalePriceAdjustInterval = Int32.Parse(priceAdjustInterval.Text);
            if (!profitOffset.Text.Equals(""))
                order.ScaleProfitOffset = Double.Parse(profitOffset.Text);
            if (!initialPosition.Text.Equals(""))
                order.ScaleInitPosition = Int32.Parse(initialPosition.Text);
            if (!initialFillQuantity.Text.Equals(""))
                order.ScaleInitFillQty = Int32.Parse(initialFillQuantity.Text);

            order.ScaleAutoReset = autoReset.Checked;
            order.ScaleRandomPercent = randomiseSize.Checked;
            
            return order;
        }

        private Order GetAlgoAttributes(Order order)
        {
            if (algoStrategy.Text.Equals("") || algoStrategy.Text.Equals("None"))
                return order;
            List<TagValue> algoParams = new List<TagValue>();
            algoParams.Add(new TagValue("startTime", startTime.Text));
            algoParams.Add(new TagValue("endTime", endTime.Text));

            order.AlgoStrategy = algoStrategy.Text;

            /*Vwap Twap ArrivalPx DarkIce PctVol*/
            if (order.AlgoStrategy.Equals("VWap"))
            {
                algoParams.Add(new TagValue("maxPctVol", maxPctVol.Text));
                algoParams.Add(new TagValue("noTakeLiq", noTakeLiq.Text));
                algoParams.Add(new TagValue("getDone", getDone.Text));
                algoParams.Add(new TagValue("noTradeAhead", noTradeAhead.Text));
                algoParams.Add(new TagValue("useOddLots", useOddLots.Text));
                algoParams.Add(new TagValue("allowPastEndTime", allowPastEndTime.Text));
            }

            if (order.AlgoStrategy.Equals("Twap"))
            {
                algoParams.Add(new TagValue("strategyType", strategyType.Text));
                algoParams.Add(new TagValue("allowPastEndTime", allowPastEndTime.Text));
            }

            if (order.AlgoStrategy.Equals("ArrivalPx"))
            {
                algoParams.Add(new TagValue("allowPastEndTime", allowPastEndTime.Text));
                algoParams.Add(new TagValue("maxPctVol", maxPctVol.Text));
                algoParams.Add(new TagValue("riskAversion", riskAversion.Text));
                algoParams.Add(new TagValue("forceCompletion", forceCompletion.Text));
            }

            if (order.AlgoStrategy.Equals("DarkIce"))
            {
                algoParams.Add(new TagValue("allowPastEndTime", allowPastEndTime.Text));
                algoParams.Add(new TagValue("displaySize", displaySizeAlgo.Text));
            }

            if (order.AlgoStrategy.Equals("PctVol"))
            {
                algoParams.Add(new TagValue("pctVol", pctVol.Text));
                algoParams.Add(new TagValue("noTakeLiq", noTakeLiq.Text));
            }
            order.AlgoParams = algoParams;
            return order;
        }

        public void SetOrder(Order order)
        {
            orderId = order.OrderId;
            action.Text = order.Action;
            orderType.Text = order.OrderType;
            lmtPrice.Text = order.LmtPrice.ToString();
            quantity.Text = order.TotalQuantity.ToString();
            account.Text = order.Account;
            timeInForce.Text = order.Tif;
            auxPrice.Text = order.AuxPrice.ToString();
            displaySize.Text = order.DisplaySize.ToString();
            //order = GetExtendedOrderAttributes(order);
            //order = GetAdvisorAttributes(order);
            //order = GetVolatilityAttributes(order);
            //order = GetScaleAttributes(order);
            //order = GetAlgoAttributes(order);
        }

        private void EnableVWap()
        {
            startTime.Enabled = true;
            endTime.Enabled = true;
            maxPctVol.Enabled = true;
            noTakeLiq.Enabled = true;
            getDone.Enabled = true;
            noTradeAhead.Enabled = true;
            useOddLots.Enabled = true;
        }

        private void EnableTWap()
        {
            startTime.Enabled = true;
            endTime.Enabled = true;
            allowPastEndTime.Enabled = true;
            strategyType.Enabled = true;
        }

        private void EnableArrivalPx()
        {
            startTime.Enabled = true;
            endTime.Enabled = true;
            allowPastEndTime.Enabled = true;
            maxPctVol.Enabled = true;
            riskAversion.Enabled = true;
            forceCompletion.Enabled = true;
        }

        private void EnableDarkIce()
        {
            startTime.Enabled = true;
            endTime.Enabled = true;
            allowPastEndTime.Enabled = true;
            displaySizeAlgo.Enabled = true;
        }

        private void EnablePctVol()
        {
            startTime.Enabled = true;
            endTime.Enabled = true;
            pctVol.Enabled = true;
            noTakeLiq.Enabled = true;
        }

        private void InitialiseDropDowns()
        {
            rule80A.Items.AddRange(Rule80A.GetAll());
            rule80A.SelectedIndex = 0;

            triggerMethod.Items.AddRange(TriggerMethod.GetAll());
            triggerMethod.SelectedIndex = 0;

            faMethod.Items.AddRange(FaMethod.GetAll());
            faMethod.SelectedIndex = 0;

            ocaType.Items.AddRange(OCAType.GetAll());
            ocaType.SelectedIndex = 0;

            hedgeType.Items.AddRange(HedgeType.GetAll());
            hedgeType.SelectedIndex = 0;

            optionReferencePrice.Items.AddRange(ReferencePriceType.GetAll());
            optionReferencePrice.SelectedIndex = 0;

            volatilityType.Items.AddRange(VolatilityType.GetAll());
            volatilityType.SelectedIndex = 0;

            contractRight.Items.AddRange(ContractRight.GetAll());
            contractRight.SelectedIndex = 0;
        }
    }
}
