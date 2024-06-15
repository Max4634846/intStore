using System;
using System.Collections.Generic;

namespace intStore.Models
{
    public class OrdersWithCartModel
    {
        public int IdOrderWithCart { get; set; }
        public Nullable<int> IdProduct { get; set; }
        public Nullable<int> Quantity { get; set; }


        public virtual ICollection<CartModel> Cart { get; set; }
        public virtual Product Products { get; set; }
        public virtual CartModel CartModel { get; set; }
    }
}
