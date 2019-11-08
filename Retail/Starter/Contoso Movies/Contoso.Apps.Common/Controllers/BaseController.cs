using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Contoso.Apps.Common.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BaseController : Controller
    {
        protected CosmosClient client;
        protected Database database;
        protected string databaseId;
        //protected DocumentCollection productColl, shoppingCartItems;

        static protected IQueryable<Item> items;
        static protected IQueryable<Category> categories;
        static protected IQueryable<Order> orders;
        static protected IQueryable<Movies.Data.Models.User> users;

        public BaseController()
        {
            string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];

            CosmosClientOptions options = new CosmosClientOptions();
            options.ConnectionMode = ConnectionMode.Gateway;

            client = new CosmosClient(endpointUrl, authorizationKey);

            var container = client.GetContainer(databaseId, "object");

            if (items == null)            
                items = container.GetItemLinqQueryable<Item>(true).Where(c=>c.EntityType == "Item");

            if (categories == null)
                categories = container.GetItemLinqQueryable<Category>(true).Where(c => c.EntityType == "Category");
            
            if (users == null)
                users = container.GetItemLinqQueryable<User>(true).Where(c => c.EntityType == "User");
        }

        public async Task<ActionResult> SetUser(int userId)
        {
            /*
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");
            var query = client.CreateDocumentQuery<Movies.Data.Models.User>(collectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE f.UserId = @id and f.EntityType = 'User'",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", userId)
                    }
            }, DefaultOptions);

            Movies.Data.Models.User user = query.ToList().FirstOrDefault();
            */

            Movies.Data.Models.User user = await DbHelper.GetObject<User>("User_" + userId, "User", "User");
            Session["User"] = user;

            this.HttpContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(user.Email), new string[] { /* fill roles if any */ });

            return RedirectToAction("Index", "Home");
        }

            public ActionResult NewUser(string email)
        {
            Movies.Data.Models.User user = new Movies.Data.Models.User();
            user.Email = "newuser@contosomovies.com";
            user.UserId = 1;

            Session["User"] = user;

            this.HttpContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(user.Email), new string[] { /* fill roles if any */ });

            return RedirectToAction("Index", "Home");
        }

        public string DisplayName { get; set; }

        protected override void EndExecute(IAsyncResult asyncResult)
        {
            if (Request.IsAuthenticated)
            {
                var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    string displayNameClaim = "http://schemas.microsoft.com/identity/claims/displayname";
                    var claim = claimsIdentity.FindFirst(displayNameClaim);
                    if (claim != null)
                    {
                        DisplayName = claim.Value;
                    }
                }
                //DisplayName = ((System.Security.Claims.ClaimsIdentity)User.Identity).FindFirst("http://schemas.microsoft.com/identity/claims/displayname").Value;
            }
            base.EndExecute(asyncResult);
        }
    }
}