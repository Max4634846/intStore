using System;
using System.Collections.Generic;

namespace intStore.Models
{
    public class Customer
    {
        public int Id_Customers { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> RegisterDate { get; set; }
        public string Password { get; set; }
        public int id_Payments { get; set; }

        public virtual ICollection<CartModel> Cart { get; set; }

        public virtual ICollection<OrdersModel> Orders { get; set; }
        public virtual ICollection<PaymentModel> Payments { get; set; }
    }
}
