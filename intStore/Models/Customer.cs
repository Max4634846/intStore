using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace intStore.Models
{
    public class Customer
    {
        public int Id_Customers { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
        public string Password { get; set; }

        public virtual ICollection<CartModel> Cart { get; set; }

        public virtual ICollection<OrdersModel> Orders { get; set; }
    }
}
