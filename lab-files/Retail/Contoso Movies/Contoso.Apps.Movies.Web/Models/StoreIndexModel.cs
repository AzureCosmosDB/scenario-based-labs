using System.Collections.Generic;

namespace Contoso.Apps.Movies.Web.Models
{
    public class StoreIndexModel
    {
        // List of products.
        public List<ProductListModel> Products { get; set; }

        public List<ProductListModel> RandomProducts { get; set; }
        // List of categories used to filter through the products.
        public List<CategoryModel> Categories { get; set; }
    }
}