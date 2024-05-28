using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intStore.Models
{
    public class OrdersWithCart
    {
        public int id_OrderWithCart { get; set; }
        public Nullable<int> id_Product { get; set; }
        public Nullable<int> Quantity { get; set; }
    }
}
