using System;

namespace intStore.Models
{
    public class CartModel
    {
         public int id_Cart { get; set; }
        public Nullable<int> id_Order { get; set; }
        public Nullable<int> id_Customer { get; set; }
        public int id_OrdersWithCart { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        public virtual Customer Customers { get; set; }
        public virtual OrdersModel Orders { get; set; }
        public virtual OrdersWithCartModel OrdersWithCart { get; set; }
    }
}
