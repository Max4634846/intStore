using System;
using System.Collections.Generic;

namespace intStore.Models
{
    public class OrdersModel
    {
        public OrdersModel()
        {
            this.Cart = new HashSet<CartModel>();
        }
        public int id_Order { get; set; }
        public Nullable<int> id_Cart { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> id_StatusOrders { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> id_Payments { get; set; }

        public virtual ICollection<CartModel> Cart { get; set; }
    }
}
