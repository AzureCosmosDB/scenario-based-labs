using AutoMapper;
using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Logic;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using Contoso.Apps.Common.Controllers;

namespace Contoso.Apps.Movies.Web.Controllers
{
    [AllowAnonymous]
    public class CartController : BaseController
    {
        public CartController()
        {    
            
        }

        // GET: Cart
        public ActionResult Index()
        {
            var cartId = new Helpers.CartHelpers().GetCartId();

            var vm = new CartModel();

            using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions(cartId, databaseId, client, items, categories))
            {
                var shoppingCartItems = usersShoppingCart.GetCartItems();
                var cartItemsVM = Mapper.Map<List<CartItemModel>>(shoppingCartItems);
                vm.CartItems = cartItemsVM;
                vm.ItemsTotal = usersShoppingCart.GetCount();
                vm.OrderTotal = usersShoppingCart.GetTotal();
            }
            //var shoppingCartItems = db.ShoppingCartItems.Include(c => c.Product);
            return View(vm);
        }

        public RedirectResult AddToCart(int productId)
        {
            var cartId = new Helpers.CartHelpers().GetCartId();
            using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions(cartId, databaseId, client, items, categories))
            {
                usersShoppingCart.AddToCart(productId);
            }
            return Redirect("Index");
        }

        // GET: Cart/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "shoppingcartitems");

            var query = client.CreateDocumentQuery<CartItem>(collectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM shoppingcartitems f WHERE (f.id = @id)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id)
                    }
            }, DefaultOptions);

            CartItem cartItem = query.ToList().FirstOrDefault();

            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(items, "ProductId", "ProductName");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemId,CartId,Quantity,DateCreated,ProductId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "shoppingcartitems");
                client.UpsertDocumentAsync(collectionUri, cartItem);

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(items, "ProductId", "ProductName", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "shoppingcartitems");

            var query = client.CreateDocumentQuery<CartItem>(collectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM shoppingcartitems f WHERE (f.id = @id)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id)
                    }
            }, DefaultOptions);

            CartItem cartItem = query.ToList().FirstOrDefault();

            if (cartItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(items, "ProductId", "ProductName", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemId,CartId,Quantity,DateCreated,ProductId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "shoppingcartitems");
                client.UpsertDocumentAsync(collectionUri, cartItem);
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(items, "ProductID", "ProductName", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: Cart/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "shoppingcartitems");

            var query = client.CreateDocumentQuery<CartItem>(collectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM shoppingcartitems f WHERE (f.id = @id)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id)
                    }
            }, DefaultOptions);

            CartItem cartItem = query.ToList().FirstOrDefault();

            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(databaseId, "shoppingcartitems", id),
                new RequestOptions { PartitionKey = new PartitionKey("id") });
            
            return RedirectToAction("Index");
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
