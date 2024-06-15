using System;
using System.Collections.Generic;

namespace intStore.Models
{
    public class OrdersModel
    {
        public int IdOrder { get; set; }
        public Nullable<int> IdCart { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> IdStatusOrders { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> IdPayments { get; set; }

        public virtual ICollection<CartModel> Cart { get; set; }
    }
}
