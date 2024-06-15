using System;


namespace intStore.Models
{
    public class CartModel
    {
         public int IdCart { get; set; }
        public Nullable<int> IdOrder { get; set; }
        public Nullable<int> IdCustomer { get; set; }
        public int IdOrdersWithCart { get; set; }
        public Nullable<int> Quantity { get; set; }
    
        public virtual Customer Customers { get; set; }
        public virtual OrdersModel Orders { get; set; }
        public virtual OrdersWithCartModel OrdersWithCart { get; set; }
    }
}
