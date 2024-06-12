using System;
using System.Collections.Generic;

namespace intStore.Models
{
    public class OrdersWithCartModel
    {
        public OrdersWithCartModel()
        {
            this.Cart = new HashSet<CartModel>();
        }
        public int Id_OrderWithCart { get; set; }
        public Nullable<int> Id_Product { get; set; }
        public Nullable<int> Quantity { get; set; }


        public virtual ICollection<CartModel> Cart { get; set; }
        public virtual Product Products { get; set; }
        public virtual CartModel CartModel { get; set; }
    }
}
