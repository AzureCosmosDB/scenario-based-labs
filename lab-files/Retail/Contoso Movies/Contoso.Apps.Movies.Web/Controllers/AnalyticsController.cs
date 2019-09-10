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
            Contoso.Apps.Movies.Data.Models.User user = (Contoso.Apps.Movies.Data.Models.User)Session["User"];
            string name = user.Email;

            UserAnalyticsModel m = new UserAnalyticsModel();

            m.RecommendProductsAssoc = new List<Product>();
            m.RecommendProductsCollabBased = new List<Product>();
            m.RecommendProductsContentBased = new List<Product>();
            m.RecommendProductsHybrid = new List<Product>();
            m.RecommendProductsMatrixFactor = new List<Product>();
            m.RecommendProductsRanking = new List<Product>();

            /*
            m.RecommendProductsAssoc = RecommendationHelper.AssociationRecommendation(name, 12);
            m.RecommendProductsCollabBased = RecommendationHelper.CollaborationBasedRecommendation(name, 12);
            m.RecommendProductsContentBased = RecommendationHelper.ContentBasedRecommendation(name, 12);
            m.RecommendProductsHybrid = RecommendationHelper.HybridRecommendation(name, 12);
            m.RecommendProductsMatrixFactor = RecommendationHelper.MatrixFactorRecommendation(name, 12);
            m.RecommendProductsRanking = RecommendationHelper.RankingRecommendation(name, 12);
            */

            //get similar users...
            m.UsersJaccard = RecommendationHelper.JaccardRecommendation(name);
            m.UsersPearson = RecommendationHelper.PearsonRecommendation(name);
            
            //get the user events
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "collector_log");
            var query = client.CreateDocumentQuery<CollectorLog>(collectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM collector_log f WHERE f.UserId = @id",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", user.UserId)
                    }
            }, DefaultOptions);

            List<CollectorLog> logs = query.ToList().Take(100).ToList();
            m.Events = logs;

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