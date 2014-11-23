/****************************** Project Header ******************************\
Project:	      QuantTrading
Author:			  Letian_zj @ Codeplex
URL:			  https://quanttrading.codeplex.com/
Copyright 2014 Letian_zj

This file is part of QuantTrading Project.

QuantTrading is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, 
either version 3 of the License, or (at your option) any later version.

QuantTrading is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with QuantTrading. 
If not, see http://www.gnu.org/licenses/.

\***************************************************************************/
using System;
using System.ComponentModel;

namespace TradingBase
{
    /// <summary>
    /// Order Status reported by enum order status.
    /// </summary>
    [Serializable()]
    public enum OrderStatus
    {
        /// <summary>
        /// indicates that you have transmitted the order, but have not yet received
        /// confirmation that it has been accepted by the order destination.
        /// This order status is not sent by TWS and should be explicitly set by the API developer when an order is submitted.
        /// </summary>
        [Description("PendingSubmit")]
        PendingSubmit,
        /// <summary>
        /// PendingCancel - indicates that you have sent a request to cancel the order
        /// but have not yet received cancel confirmation from the order destination.
        /// At this point, your order is not confirmed canceled. You may still receive
        /// an execution while your cancellation request is pending.
        /// This order status is not sent by TWS and should be explicitly set by the API developer when an order is canceled.
        /// </summary>
        [Description("PendingCancel")]
        PendingCancel,
        /// <summary>
        /// indicates that a simulated order type has been accepted by the IB system and
        /// that this order has yet to be elected. The order is held in the IB system
        /// (and the status remains DARK BLUE) until the election criteria are met.
        /// At that time the order is transmitted to the order destination as specified
        /// (and the order status color will change).
        /// </summary>
        [Description("PreSubmitted")]
        PreSubmitted,
        /// <summary>
        /// indicates that your order has been accepted at the order destination and is working.
        /// </summary>
        [Description("Submitted")]
        Submitted,
        /// <summary>
        /// indicates that the balance of your order has been confirmed canceled by the IB system.
        /// This could occur unexpectedly when IB or the destination has rejected your order.
        /// </summary>
        [Description("Cancelled")]
        Canceled,
        /// <summary>
        /// The order has been completely filled.
        /// </summary>
        [Description("Filled")]
        Filled,
        /// <summary>
        /// The Order is inactive
        /// </summary>
        [Description("Inactive")]
        Inactive,
        /// <summary>
        /// The order is Partially Filled
        /// </summary>
        [Description("PartiallyFilled")]
        PartiallyFilled,
        /// <summary>
        /// Api Pending
        /// </summary>
        [Description("ApiPending")]
        ApiPending,
        /// <summary>
        /// Api Cancelled
        /// </summary>
        [Description("ApiCancelled")]
        ApiCancelled,
        /// <summary>
        /// Indicates that there is an error with this order
        /// This order status is not sent by TWS and should be explicitly set by the API developer when an error has occured.
        /// </summary>
        [Description("Error")]
        Error,
        /// <summary>
        /// No Order Status
        /// </summary>
        [Description("")]
        None
    }
}
