using Contoso.Apps.Movies.Data.Models;
using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class HomeModel
    {
        public HomeModel()
        {
            this.RecommendProductsBought = new List<Item>();
            this.RecommendProductsLiked = new List<Item>();
            this.RecommendProductsTop = new List<Item>();
        }
        public IList<Models.ProductModel> Products { get; set; }

        public List<Item> RecommendProductsBought { get; set; }

        public List<Item> RecommendProductsLiked { get; set; }

        public List<Item> RecommendProductsTop { get; set; }
    }
}