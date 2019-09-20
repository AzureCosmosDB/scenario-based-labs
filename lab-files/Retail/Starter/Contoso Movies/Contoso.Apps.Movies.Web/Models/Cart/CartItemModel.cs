namespace Contoso.Apps.Movies.Web.Models
{
    public class CartItemModel
    {
        public int ItemId { get; set; }

        public string CartId { get; set; }

        public string CartItemId { get; set; }

        public int Quantity { get; set; }

        public System.DateTime DateCreated { get; set; }

        public string ProductProductName { get; set; }

        public string ProductImagePath { get; set; }

        public string ProductThumbnailPath { get; set; }

        public double? ProductUnitPrice { get; set; }
    }
}