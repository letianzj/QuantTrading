using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IBApi
{
    /**
     * @class OrderComboLeg
     * @brief Allows to specify a price on an order's leg
     * @sa Order, ComboLeg
     */
    public class OrderComboLeg
    {
        
        public double price;

        /**
         * @brief The order's leg's price
         */
        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public OrderComboLeg()
        {
            price = Double.MaxValue;
        }

        public OrderComboLeg(double p_price)
        {
            price = p_price;
        }

        public bool Equals(Object other)
        {
            if (this == other)
            {
                return true;
            }
            else if (other == null)
            {
                return false;
            }

            OrderComboLeg theOther = (OrderComboLeg)other;

            if (price != theOther.Price)
            {
                return false;
            }

            return true;
        }
    }
}
