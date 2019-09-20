using Contoso.Apps.Movies.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Contoso.Apps.Common.Extensions;
using Contoso.Apps.Movies.Offers.Models;

namespace Contoso.Apps.Movies.Offers.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class OffersController : ApiController
    {
        List<Item> allProducts = new List<Item>();

        // GET api/values
        [Route("api/get")]
        public IEnumerable<OfferedProduct> Get()
        {
            List<OfferedProduct> productOffers = new List<OfferedProduct>();
            // Retrieve 3 random products.
            // In a real-world scenario, you may return a list of products that are on sale,
            // or based off of popularity or other factors.
            var products = allProducts.Shuffle().Take(3);

            if (products != null && products.Count() > 0)
            {
                // Randomizer for sales price.
                var rando = new Random();
                // Loop through the products and populate a new OfferedProduct class for each.
                foreach (var product in products)
                {
                    double salePrice = 0;
                    double percentage = 0;
                    if (product.UnitPrice.HasValue && product.UnitPrice.Value > 0)
                    {
                        // Reduce price by random percentage between 5 and 40%.
                        percentage = (double)rando.Next(5, 41);
                        salePrice = Math.Round(product.UnitPrice.Value - (product.UnitPrice.Value * (percentage / 100)), 2);
                    }

                    productOffers.Add(new OfferedProduct
                    {
                        ItemId = product.ItemId,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        ImagePath = product.ImagePath,
                        ThumbnailPath = product.ThumbnailPath,
                        UnitPrice = product.UnitPrice,
                        SalePrice = salePrice,
                        SalePercentage = percentage
                    });
                }
            }

            return productOffers;
        }

    }
}
