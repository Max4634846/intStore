using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intStore.Models
{
    public class CartModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int OrdersWithCartId { get; set; }
        public int Quantity { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OrdersWithCartModel OrdersWithCart { get; set; }
    }
}
