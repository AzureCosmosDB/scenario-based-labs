using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class CartModel
    {
        public IList<Models.CartItemModel> CartItems { get; set; }
        public decimal OrderTotal { get; set; }
        public int ItemsTotal { get; set; }
    }
}