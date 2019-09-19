using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class StoreDetailsModel
    {
        // Details for the displayed product.
        public ProductModel Product { get; set; }

        // Products the logged in user may like based on similarity
        public List<ProductListModel> SimilarProducts { get; set; }

        // Products that are in the same category as the displayed product.
        public List<ProductListModel> RelatedProducts { get; set; }
        
        // Three random products that are different from the displayed one.
        public List<ProductListModel> NewProducts { get; set; }
        
        // List of categories to display as clickable links.
        public List<CategoryModel> Categories { get; set; }
    }
}