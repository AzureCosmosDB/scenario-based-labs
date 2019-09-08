using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Contoso.Apps.Movies.Web.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }

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

        [DisplayName("Receive SMS Notifications?")]
        public bool SMSOptIn { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        /// <summary>
        /// This is the transaction Id we receive from the payment processing gateway.
        /// </summary>
        [ScaffoldColumn(false)]
        public string PaymentTransactionId { get; set; }

        // Credit card information not stored in the database:
        [Required(ErrorMessage = "Name on credit card is required")]
        [DisplayName("Name on credit card")]
        public string NameOnCard { get; set; }

        [Required(ErrorMessage = "Please select a credit card type")]
        [DisplayName("Credit card type")]
        public string CreditCardType { get; set; }

        [Required(ErrorMessage = "Credit card number is required")]
        [DisplayName("Credit card #")]
        public string CreditCardNumber { get; set; }

        [Required(ErrorMessage = "Credit card expiration date is required")]
        [DisplayName("Expiration date")]
        public string ExpirationDate { get; set; }

        [Required(ErrorMessage = "Credit card security code (CCV) is required")]
        [DisplayName("Security code")]
        public string CCV { get; set; }

    }
}