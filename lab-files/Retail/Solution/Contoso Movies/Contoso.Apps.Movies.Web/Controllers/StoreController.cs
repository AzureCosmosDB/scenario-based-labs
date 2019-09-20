using AutoMapper;
using Contoso.Apps.Common;
using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Common.Extensions;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Logic;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            Contoso.Apps.Movies.Data.Models.User user = (Contoso.Apps.Movies.Data.Models.User)Session["User"];

            List<Item> products = new List<Item>();

            //only take 10 products...
            if (user != null)
            {
                string name = user.Email;
                int userId = user.UserId;
                products = RecommendationHelper.GetViaFunction("assoc", userId, 12);
            }
            else
            {
                products = RecommendationHelper.GetViaFunction("top", 0, 12);
            }

            var randomVm = Mapper.Map<List<Models.ProductListModel>>(RecommendationHelper.GetViaFunction("random", 0, 12));
            
            var productsVm = Mapper.Map<List<Models.ProductListModel>>(products);

            // Retrieve category listing:
            var categoriesVm = Mapper.Map<List<Models.CategoryModel>>(categories.ToList());

            var vm = new Models.StoreIndexModel
            {
                RandomProducts = randomVm,
                Products = productsVm,
                Categories = categoriesVm
            };

            return View(vm);
        }

        public ActionResult Genre(int categoryId)
        {
            var container = client.GetContainer(databaseId, "object");
            
            var query = container.GetItemLinqQueryable<Item>(true).Where(c=>c.CategoryId == categoryId && c.EntityType == "Item");

            List<Item> products = query.ToList();

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
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Item product = await DbHelper.GetItem(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var productVm = Mapper.Map<Models.ProductModel>(product);

            //Get the simliar product to this item...
            var similarProducts = RecommendationHelper.GetViaFunction("content", 0, id.Value);
            var similarProductsVm = Mapper.Map<List<Models.ProductListModel>>(similarProducts);

            // Find related products, based on the category
            var relatedProducts = items.Where(p => p.CategoryId == product.CategoryId && p.ItemId != product.ItemId).Take(10).ToList();
            var relatedProductsVm = Mapper.Map<List<Models.ProductListModel>>(relatedProducts);

            // Retrieve category listing
            var categoriesVm = Mapper.Map<List<Models.CategoryModel>>(categories);

            // Retrieve "new products" as a list of three random products not equal to the displayed one
            var newProducts = items.Where(p => p.ItemId != product.ItemId).Take(50).ToList().Shuffle().Take(3);

            var newProductsVm = Mapper.Map<List<Models.ProductListModel>>(newProducts);

            var vm = new Models.StoreDetailsModel
            {
                Product = productVm,
                RelatedProducts = relatedProductsVm,
                SimilarProducts = similarProductsVm,
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
