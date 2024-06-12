using System.Collections.Generic;


namespace intStore.Models
{
    public class PaymentModel
    {
        public int id_Payments { get; set; }
        public string MethodName { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
