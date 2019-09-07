using Contoso.Apps.Movies.Data.Models;
using System;

namespace Contoso.Apps.Movies.Data.Logic
{
    public class AddProducts
    {
        public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
        {
            var myProduct = new Product()
            {
                ProductName = ProductName,
                Description = ProductDesc,
                UnitPrice = Convert.ToDouble(ProductPrice),
                ImagePath = ProductImagePath,
                CategoryID = Convert.ToInt32(ProductCategory),
            };

            using (var _db = new ProductContext())
            {
                // Add product to DB.
                _db.Products.Add(myProduct);
                _db.SaveChanges();
            }
            // Success.
            return true;
        }
    }
}