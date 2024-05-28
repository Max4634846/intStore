using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intStore.Models
{
    public class ProductEventArgs : EventArgs
    {
        public Product Product { get; }
        public int Quantity { get; }

        public ProductEventArgs(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}
