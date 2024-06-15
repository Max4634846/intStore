using System.Collections.Generic;


namespace intStore.Models
{
    public class PaymentModel
    {
        public int IdPayments { get; set; }
        public string MethodName { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
