using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Apps.Movies.Data.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        protected IQueryable<Item> items;
        protected IQueryable<Category> categories;
        protected IQueryable<Order> orders;
        protected IQueryable<CartItem> shoppingCartItems;

        protected string databaseId;
        protected CosmosClient client;
        
        //protected static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        public ShoppingCartActions(string cartId, string databaseId, CosmosClient client, IQueryable<Item> items, IQueryable<Category> categories)
        {
            this.databaseId = databaseId;
            this.client = client;
            this.items = items;
            this.categories = categories;
            this.ShoppingCartId = cartId;

            //collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var container = client.GetContainer(databaseId, "object");
            var options = new QueryRequestOptions {PartitionKey = new PartitionKey(cartId)};
            shoppingCartItems = container.GetItemLinqQueryable<CartItem>(true, null, options).Where(c => c.CartId == cartId && c.EntityType == "CartItem");

            /*
            shoppingCartItems = client.CreateDocumentQuery<CartItem>(collectionUri,
                new SqlQuerySpec(
                    "SELECT * FROM object r WHERE r.CartId = @cartid and r.EntityType = 'CartItem'",
                    new SqlParameterCollection(new[]
                    {
                        new SqlParameter { Name = "@cartid", Value = cartId }
                    }
                    )
                    ),DefaultOptions
            );
            */
        }

        public string ShoppingCartId { get; private set; }

        public async Task AddToCart(int id)
        {
            // Retrieve the product from the database.           
            //ShoppingCartId = GetCartId();

            var cartItem = shoppingCartItems.ToList().SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ItemId == id);

            if (cartItem == null)
            {
                Item p = await DbHelper.GetItem(id);

                // Create a new cart item if no cart item exists.                 
                cartItem = new CartItem {
                    CartItemId = Guid.NewGuid().ToString(),
                    ItemId = id,
                    CartId = ShoppingCartId,
                    Product = p,
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                await DbHelper.SaveObject(cartItem);

                //client.UpsertDocumentAsync(collectionUri, cartItem);
            }
            else {
                // If the item does exist in the cart,                  
                // then add one to the quantity.                 
                cartItem.Quantity++;
            }
        }

        public void Dispose()
        {
            
        }

        public List<CartItem> GetCartItems()
        {
            //ShoppingCartId = GetCartId();

            return shoppingCartItems.ToList();
        }

        public decimal GetTotal()
        {
            //ShoppingCartId = GetCartId();
            // Multiply product price by quantity of that product to get        
            // the current price for each of those products in the cart.  
            // Sum all product price totals to get the cart total.   
            decimal? total = decimal.Zero;

            total = (decimal?)(from cartItems in shoppingCartItems.ToList()
                               where cartItems.CartId == ShoppingCartId
                               select (int?)cartItems.Quantity *
                               cartItems.Product.UnitPrice).Sum();

            return total ?? decimal.Zero;
        }

        public void UpdateShoppingCartDatabase(String cartId, ShoppingCartUpdates[] CartItemUpdates)
        {
            try
            {
                int CartItemCount = CartItemUpdates.Count();
                List<CartItem> myCart = GetCartItems();
                foreach (var cartItem in myCart)
                {
                    // Iterate through all rows within shopping cart list
                    for (int i = 0; i < CartItemCount; i++)
                    {
                        if (cartItem.Product.ItemId == CartItemUpdates[i].ProductId)
                        {
                            if (CartItemUpdates[i].PurchaseQuantity < 1 || CartItemUpdates[i].RemoveItem == true)
                            {
                                RemoveItem(cartId, cartItem.ItemId);
                            }
                            else
                            {
                                UpdateItem(cartId, cartItem.ItemId, CartItemUpdates[i].PurchaseQuantity);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp);
            }
        }

        public void RemoveItem(string removeCartID, int removeProductID)
        {
            try
            {
                var myItem = (from c in shoppingCartItems.ToList() where c.CartId == removeCartID && c.Product.ItemId == removeProductID select c).FirstOrDefault();

                if (myItem != null)
                {
                    DbHelper.DeleteObject(myItem);

                    /*
                    Document doc = client.CreateDocumentQuery(collectionUri,
                        new SqlQuerySpec(
                            "SELECT * FROM object r WHERE r.CartId = @cartid and r.ItemId == @productid and r.EntityType = 'CartItem'",
                            new SqlParameterCollection(new[] 
                            {
                                new SqlParameter { Name = "@cartid", Value = removeCartID },
                                new SqlParameter { Name = "@productid", Value = removeProductID }
                            }
                            )
                            )
                    ).FirstOrDefault();

                    client.DeleteDocumentAsync(doc.SelfLink);
                    */
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Remove Cart Item - " + exp.Message.ToString(), exp);
            }
        }

        public void UpdateItem(string updateCartID, int updateProductID, int quantity)
        {
            try
            {
                var myItem = (from c in shoppingCartItems.ToList() where c.CartId == updateCartID && c.Product.ItemId == updateProductID select c).FirstOrDefault();
                if (myItem != null)
                {
                    myItem.Quantity = quantity;

                    var container = client.GetContainer(databaseId, "object");
                    container.UpsertItemAsync(myItem);

                    //client.UpsertDocumentAsync(collectionUri, myItem);
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp);
            }

        }

        public async void EmptyCart()
        {
            //ShoppingCartId = GetCartId();
            var cartItems = shoppingCartItems.ToList().Where(
                c => c.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                CartItem ci = await DbHelper.GetObject<CartItem>(cartItem.ObjectId, "CartItem", "CartItem");
                DbHelper.DeleteObject(ci);

                /*
                Document doc = client.CreateDocumentQuery(collectionUri,
                        new SqlQuerySpec(
                            "SELECT * FROM object r WHERE r.CartId = @cartid and r.ItemId = @itemid and r.EntityType = 'CartItem'",
                            new SqlParameterCollection(new[]
                            {
                                new SqlParameter { Name = "@cartid", Value = cartItem.CartId },
                                new SqlParameter { Name = "@itemid", Value = cartItem.ItemId }
                            }
                            )
                            )
                    ).ToList().FirstOrDefault();

                client.DeleteDocumentAsync(doc.SelfLink);
                */
            }
        }

        public int GetCount()
        {
            //ShoppingCartId = GetCartId();

            // Get the count of each item in the cart and sum them up          
            int? count = (from cartItems in shoppingCartItems.ToList()
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Quantity).Sum();

            // Return 0 if all entries are null         
            return count ?? 0;
        }

        public struct ShoppingCartUpdates
        {
            public int ProductId;
            public int PurchaseQuantity;
            public bool RemoveItem;
        }
    }
}