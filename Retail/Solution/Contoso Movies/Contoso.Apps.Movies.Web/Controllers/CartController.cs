using AutoMapper;
using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Logic;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using Contoso.Apps.Common.Controllers;
using System.Threading.Tasks;

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

        public async Task<RedirectResult> AddToCart(int itemId)
        {
            var cartId = new Helpers.CartHelpers().GetCartId();

            using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions(cartId, databaseId, client, items, categories))
            {
                await usersShoppingCart.AddToCart(itemId);
            }

            return Redirect("Index");
        }

        // GET: Cart/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CartItem cartItem = await DbHelper.GetObject<CartItem>(id, "CartItem", id);

            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(items, "ItemId", "ProductName");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CartItemId,CartId,Quantity,DateCreated,ItemId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                await DbHelper.SaveObject(cartItem);

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(items, "ItemId", "ProductName", cartItem.ItemId);

            return View(cartItem);
        }

        // GET: Cart/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CartItem cartItem = await DbHelper.GetObject<CartItem>(id, "CartItem", id);

            if (cartItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(items, "ItemId", "ProductName", cartItem.ItemId);
            return View(cartItem);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CartItemId,CartId,Quantity,DateCreated,ItemId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                //Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "shoppingcartitems");

                await DbHelper.SaveObject(cartItem);
                
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(items, "ItemId", "ProductName", cartItem.ItemId);
            return View(cartItem);
        }

        // GET: Cart/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CartItem cartItem = await DbHelper.GetObject<CartItem>(id, "CartItem", id);

            if (cartItem == null)
            {
                return HttpNotFound();
            }

            return View(cartItem);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            CartItem ci = await DbHelper.GetObject<CartItem>(id, "CartItem", id);

            await DbHelper.DeleteObject(ci);
            
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
