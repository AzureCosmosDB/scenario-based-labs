using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace Contoso.Apps.Movies.Data.Models
{
    [Serializable]
    public class OrderDetail : DbObject, IEntity
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        // Using the customer's email address instead of username, since we're bypassing authentication for this demo.
        public string Email { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double? UnitPrice { get; set; }

        /// <summary>
        /// Calculated field (read only) that returns the cost based on quantity * price.
        /// </summary>
        [ReadOnly(true)]
        public double Cost
        {
            get
            {
                double unitPrice = UnitPrice.HasValue ? UnitPrice.Value : 0;
                return Math.Round(unitPrice * Quantity, 2);
            }
        }

        [ReadOnly(true)]
        public Item Product { get; set; }

        public string ObjectId { get { return this.EntityType + "_" + this.OrderDetailId; } }

        public string EntityType { get { return "OrderDetail"; } }
    }
}