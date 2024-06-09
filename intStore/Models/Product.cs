using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace intStore.Models
{
    public class Product
    {
        public int id_Product { get; set; }
        public int id_Categories { get; set; }
        public string NameProduct { get; set; }
        public string Description { get; set; }
        public string NutritionalValue { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.DateTime> DateProduct { get; set; }
        public ImageSource ImageProduct { get; set; }
        public Nullable<int> id_Shipment { get; set; }
        public string Status{  get; set; }

        public virtual ICollection<OrdersWithCartModel> OrdersWithCartModel { get; set; }
        public virtual CategoriesModel Categories { get; set; }
    }
}
