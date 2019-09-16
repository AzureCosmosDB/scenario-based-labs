using System;
using System.Collections.Generic;
using System.Text;

namespace ContosoFunctionApp
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public System.DateTime OrderDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ReceiptUrl { get; set; }
        public bool SMSOptIn { get; set; }
        public string SMSStatus { get; set; }
        public string PaymentTransactionId { get; set; }
        public bool HasBeenShipped { get; set; }
        public decimal Total { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }



}
