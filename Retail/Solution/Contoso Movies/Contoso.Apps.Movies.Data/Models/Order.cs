using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class Order : DbObject, IEntity
    {
        public int OrderId { get; set; }
        public string PartitionKey => OrderId.ToString();
        public System.DateTime OrderDate { get; set; }

        // Using the customer's email address instead of username, since we're bypassing authentication for this demo.
        //public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [DisplayName("Postal Code")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(40)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(24)]
        public string Phone { get; set; }

        public bool SMSOptIn { get; set; }

        [ScaffoldColumn(false)]
        public string SMSStatus { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// The Url to the generated Pdf receipt.
        /// </summary>
        [ScaffoldColumn(false)]
        public string ReceiptUrl { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        [ScaffoldColumn(false)]
        public string PaymentTransactionId { get; set; }

        [ScaffoldColumn(false)]
        public bool HasBeenShipped { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

        public string ObjectId { get { return this.EntityType + "_" + this.OrderId; } }

        public string EntityType { get { return "Order"; } }
    }
}