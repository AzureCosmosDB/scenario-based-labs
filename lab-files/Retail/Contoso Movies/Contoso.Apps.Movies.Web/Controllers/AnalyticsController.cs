using Contoso.Apps.Common.Controllers;
using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Logic;
using Contoso.Apps.Movies.Web.Controllers;
using Contoso.Apps.Movies.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace Contoso.Apps.Movies.Controllers
{
    [AllowAnonymous]
    public class AnalyticsController : BaseController
     {
        public ActionResult User()
        {
            UserAnalyticsModel m = new UserAnalyticsModel();

            Contoso.Apps.Movies.Data.Models.User user = (Contoso.Apps.Movies.Data.Models.User)Session["User"];

            if (user != null)
            {
                string name = user.Email;
                int userId = user.UserId;

                m.RecommendProductsAssoc = RecommendationHelper.GetViaFunction("assoc", userId, 12);
                m.RecommendProductsCollabBased = new List<Item>();
                m.RecommendProductsContentBased = new List<Item>();
                m.RecommendProductsHybrid = new List<Item>();
                m.RecommendProductsMatrixFactor = new List<Item>();
                m.RecommendProductsRanking = new List<Item>();

                //get similar users...
                /*
                m.UsersJaccard = RecommendationHelper.JaccardRecommendation(userId);
                m.UsersPearson = RecommendationHelper.PearsonRecommendation(userId);
                */

                //get the user events
                Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "event");
                var query = client.CreateDocumentQuery<CollectorLog>(collectionUri, new SqlQuerySpec()
                {
                    QueryText = "SELECT * FROM event f WHERE f.UserId = @id",
                    Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", user.UserId)
                    }
                }, DefaultOptions);

                List<CollectorLog> logs = query.ToList().Take(100).ToList();
                m.Events = logs;
            }

            return View(m);
        }

        public ActionResult Content()
        {
            string name = this.HttpContext.User.Identity.Name;

            UserAnalyticsModel m = new UserAnalyticsModel();

            return View(m);
        }

    }
}