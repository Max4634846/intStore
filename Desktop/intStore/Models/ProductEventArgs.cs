using System;

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
