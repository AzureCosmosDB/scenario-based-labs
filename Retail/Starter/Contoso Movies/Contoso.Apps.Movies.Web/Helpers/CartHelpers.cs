using Contoso.Apps.Movies.Data.Logic;
using System;
using System.Web;

namespace Contoso.Apps.Movies.Web.Helpers
{
    public class CartHelpers {
        public const string CartSessionKey = "CartId";

        public ShoppingCartActions GetCart(HttpContext context) {

            /*
            using (var cart = new ShoppingCartActions(GetCartId())) {
                return cart;
            }
            */

            return null;

        }

        public string GetCartId() {
            if (HttpContext.Current.Session[CartSessionKey] == null) {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name)) {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else {
                    // Generate a new random GUID using System.Guid class.     
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return HttpContext.Current.Session[CartSessionKey].ToString();
        }
    }
}