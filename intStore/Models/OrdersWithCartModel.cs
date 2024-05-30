using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intStore.Models
{
    public class OrdersWithCartModel
    {
        public int Id_OrderWithCart { get; set; }
        public Nullable<int> Id_Product { get; set; }
        public Nullable<int> Quantity { get; set; }


        public virtual ICollection<CartModel> Cart { get; set; }
        public virtual Product Product { get; set; }
    }
}
