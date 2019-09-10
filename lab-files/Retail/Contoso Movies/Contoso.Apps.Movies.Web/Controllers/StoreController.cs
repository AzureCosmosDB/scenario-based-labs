using AutoMapper;
using Contoso.Apps.Common;
using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Common.Extensions;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Logic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Contoso.Apps.Movies.Web.Controllers
{
    [AllowAnonymous]
    public class StoreController : BaseController
    {
        public StoreController()
        {
        }

        // GET: Store
        public ActionResult Index(string categoryId)
        {
            //only take 10 products...
            List<Item> products = RecommendationHelper.Get("assoc", "", 12);
            var productsVm = Mapper.Map<List<Models.ProductListModel>>(products);

            // Retrieve category listing:
            var categoriesVm = Mapper.Map<List<Models.CategoryModel>>(categories.ToList());

            var vm = new Models.StoreIndexModel
            {
                Products = productsVm,
                Categories = categoriesVm
            };

            return View(vm);
        }

        public ActionResult Genre(int categoryId)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "product");
            var query = client.CreateDocumentQuery<Item>(collectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM product f WHERE f.CategoryId = @id",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", categoryId)
                    }
            }, DefaultOptions);

            List<Item> products = query.ToList().Take(12).ToList();

            var productsVm = Mapper.Map<List<Models.ProductListModel>>(products);

            // Retrieve category listing:
            var categoriesVm = Mapper.Map<List<Models.CategoryModel>>(categories.ToList());

            var vm = new Models.StoreIndexModel
            {
                Products = productsVm,
                Categories = categoriesVm
            };

            return View(vm);
        }

        // GET: Store/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");

            var query = client.CreateDocumentQuery<Item>(productCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM item f WHERE (f.ProductId = @id)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id)
                    }
            }, DefaultOptions);

            Item product = query.ToList().FirstOrDefault();

            if (product == null)
            {
                return HttpNotFound();
            }
            var productVm = Mapper.Map<Models.ProductModel>(product);

            // Find related products, based on the category:
            var relatedProducts = items.ToList().Where(p => p.CategoryID == product.CategoryID && p.ProductId != product.ProductId).Take(10).ToList();
            var relatedProductsVm = Mapper.Map<List<Models.ProductListModel>>(relatedProducts);

            // Retrieve category listing:
            var categoriesVm = Mapper.Map<List<Models.CategoryModel>>(categories);

            // Retrieve "new products" as a list of three random products not equal to the displayed one:
            var newProducts = items.ToList().Where(p => p.ProductId != product.ProductId).ToList().Shuffle().Take(3);

            var newProductsVm = Mapper.Map<List<Models.ProductListModel>>(newProducts);

            var vm = new Models.StoreDetailsModel
            {
                Product = productVm,
                RelatedProducts = relatedProductsVm,
                NewProducts = newProductsVm,
                Categories = categoriesVm
            };

            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                
            }
            base.Dispose(disposing);
        }
    }
}
