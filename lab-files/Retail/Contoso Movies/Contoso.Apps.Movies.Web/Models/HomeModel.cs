using Contoso.Apps.Movies.Data.Models;
using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class HomeModel
    {
        public IList<Models.ProductModel> Products { get; set; }

        public List<Product> RecommendProductsBought { get; set; }

        public List<Product> RecommendProductsLiked { get; set; }
    }
}