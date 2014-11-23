using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IBSampleApp.messages;
using IBApi;
using System.Windows.Forms;

namespace IBSampleApp.ui
{
    public class OrderManager
    {        
        private OrderDialog orderDialog;
        private IBClient ibClient;
        private List<string> managedAccounts;

        private List<OpenOrderMessage> openOrders = new List<OpenOrderMessage>();

        private DataGridView liveOrdersGrid;
        private DataGridView tradeLogGrid;

        public OrderManager(IBClient ibClient, DataGridView liveOrdersGrid, DataGridView tradeLogGrid)
        {
            this.ibClient = ibClient;
            this.orderDialog = new OrderDialog(this);
            this.liveOrdersGrid = liveOrdersGrid;
            this.tradeLogGrid = tradeLogGrid;
        }

        public List<string> ManagedAccounts
        {
            get { return managedAccounts; }
            set 
            {
                orderDialog.SetManagedAccounts(value);
                managedAccounts = value;
            }
        }

        public void PlaceOrder(Contract contract, Order order)
        {
            if (order.OrderId != 0)
            {
                ibClient.ClientSocket.placeOrder(order.OrderId, contract, order);
            }
            else
            {
                ibClient.ClientSocket.placeOrder(ibClient.NextOrderId, contract, order);
                ibClient.NextOrderId++;
            }
        }
        
        public void UpdateUI(IBMessage message)
        {
            switch (message.Type)
            {
                case MessageType.OpenOrder:
                    handleOpenOrder((OpenOrderMessage)message);
                    break;
                case MessageType.OpenOrderEnd:
                    break;
                case MessageType.OrderStatus:
                    handleOrderStatus((OrderStatusMessage)message);
                    break;
                case MessageType.ExecutionData:
                    HandleExecutionMessage((ExecutionMessage)message);
                    break;
                case MessageType.CommissionsReport:
                    HandleCommissionMessage((CommissionMessage)message);
                    break;
            }
        }

        public void OpenOrderDialog()
        {
            orderDialog.ShowDialog();
        }

        public void EditOrder()
        {
            if (liveOrdersGrid.SelectedRows.Count > 0 && (int)(liveOrdersGrid.SelectedRows[0].Cells[2].Value) != 0 && (int)(liveOrdersGrid.SelectedRows[0].Cells[1].Value) == ibClient.ClientId)
            {
                DataGridViewRow selectedRow = liveOrdersGrid.SelectedRows[0];
                int orderId = (int)selectedRow.Cells[2].Value;
                for (int i = 0; i < openOrders.Count; i++)
                {
                    if (openOrders[i].OrderId == orderId)
                    {
                        orderDialog.SetOrderContract(openOrders[i].Contract);
                        orderDialog.SetOrder(openOrders[i].Order);
                    }
                }

                orderDialog.ShowDialog();
            }
        }

        public void CancelSelection()
        {
            if (liveOrdersGrid.SelectedRows.Count > 0)
            {
                for (int i = 0; i < liveOrdersGrid.SelectedRows.Count; i++)
                {
                    int orderId = (int)liveOrdersGrid.SelectedRows[i].Cells[2].Value;
                    int clientId = (int)liveOrdersGrid.SelectedRows[i].Cells[1].Value;
                    OpenOrderMessage openOrder = GetOpenOrderMessage(orderId, clientId);
                    if(openOrder != null)
                        ibClient.ClientSocket.cancelOrder(openOrder.OrderId);
                }
            }
        }

        private OpenOrderMessage GetOpenOrderMessage(int orderId, int clientId)
        {
            for (int i = 0; i < openOrders.Count; i++)
            {
                if (openOrders[i].Order.OrderId == orderId && openOrders[i].Order.ClientId == clientId)
                    return openOrders[i];
            }
            return null;
        }

        private void HandleCommissionMessage(CommissionMessage message)
        {
            for (int i = 0; i < tradeLogGrid.Rows.Count; i++)
            {
                if (((string)tradeLogGrid[0, i].Value).Equals(message.CommissionReport.ExecId))
                {
                    tradeLogGrid[7, i].Value = message.CommissionReport.Commission;
                    tradeLogGrid[8, i].Value = message.CommissionReport.RealizedPNL;
                }
            }
        }

        private void handleOpenOrder(OpenOrderMessage openOrder)
        {
            if (openOrder.Order.WhatIf)
                orderDialog.HandleIncomingMessage(openOrder);
            else
            {
                UpdateLiveOrders(openOrder);
                UpdateLiveOrdersGrid(openOrder);
            }
        }

        private void HandleExecutionMessage(ExecutionMessage message)
        {
            for (int i = 0; i < tradeLogGrid.Rows.Count; i++)
            {
                if (((string)tradeLogGrid[0, i].Value).Equals(message.Execution.ExecId))
                {
                    PopulateTradeLog(i, message);
                }
            }
            tradeLogGrid.Rows.Add(1);
            PopulateTradeLog(tradeLogGrid.Rows.Count-1, message);
        }

        private void PopulateTradeLog(int index, ExecutionMessage message)
        {
            tradeLogGrid[0, index].Value = message.Execution.ExecId;
            tradeLogGrid[1, index].Value = message.Execution.Time;
            tradeLogGrid[2, index].Value = message.Execution.AcctNumber;
            tradeLogGrid[3, index].Value = message.Execution.Side;
            tradeLogGrid[4, index].Value = message.Execution.Shares;
            tradeLogGrid[5, index].Value = message.Contract.Symbol + " " + message.Contract.SecType + " " + message.Contract.Exchange;
            tradeLogGrid[6, index].Value = message.Execution.Price;
        }

        private void handleOrderStatus(OrderStatusMessage statusMessage)
        {
            for (int i = 0; i < liveOrdersGrid.Rows.Count; i++)
            {
                if (liveOrdersGrid[0, i].Value.Equals(statusMessage.PermId))
                {
                    liveOrdersGrid[7, i].Value = statusMessage.Status;
                    return;
                }
            }
        }

        private void UpdateLiveOrders(OpenOrderMessage orderMesage)
        {
            for (int i = 0; i < openOrders.Count; i++ )
            {
                if (openOrders[i].Order.OrderId == orderMesage.OrderId)
                {
                    openOrders[i] = orderMesage;
                    return;
                }
            }
            openOrders.Add(orderMesage);
        }

        private void UpdateLiveOrdersGrid(OpenOrderMessage orderMessage)
        {
            for (int i = 0; i<liveOrdersGrid.Rows.Count; i++)
            {
                if ((int)(liveOrdersGrid[2, i].Value) == orderMessage.Order.OrderId)
                {
                    PopulateOrderRow(i, orderMessage);
                    return;
                }
            }
            liveOrdersGrid.Rows.Add(1);
            PopulateOrderRow(liveOrdersGrid.Rows.Count - 1, orderMessage);
        }

        private void PopulateOrderRow(int rowIndex, OpenOrderMessage orderMessage)
        {
            liveOrdersGrid[0, rowIndex].Value = orderMessage.Order.PermId;
            liveOrdersGrid[1, rowIndex].Value = orderMessage.Order.ClientId;
            liveOrdersGrid[2, rowIndex].Value = orderMessage.Order.OrderId;
            liveOrdersGrid[3, rowIndex].Value = orderMessage.Order.Account;
            liveOrdersGrid[4, rowIndex].Value = orderMessage.Order.Action;
            liveOrdersGrid[5, rowIndex].Value = orderMessage.Order.TotalQuantity;
            liveOrdersGrid[6, rowIndex].Value = orderMessage.Contract.Symbol+" "+orderMessage.Contract.SecType+" "+orderMessage.Contract.Exchange;
            liveOrdersGrid[7, rowIndex].Value = orderMessage.OrderState.Status;
        }
    }
}
