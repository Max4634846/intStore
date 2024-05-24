using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace intStore.Models
{
    internal class testProduct
    {
        public int id_Product { get; set; }
        public string NameProduct { get; set; }
        public string Description { get; set; }
        public Nullable<double> Weight { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.DateTime> DateProduct { get; set; }
        public ImageSource ImageProduct { get; set; }
        public Nullable<int> id_Shipment { get; set; }
    }
}
