using Contoso.Apps.Movies.Data.Models;
using Contoso.Apps.Movies.Logic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contoso.Apps.Movies.Web.Controllers
{
    public class DataController : ApiController
    {
        protected DocumentClient client;
        protected Database database;
        protected string databaseId;
        protected DocumentCollection productColl, shoppingCartItems;

        protected static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        public DataController()
        {
            string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];

            client = new DocumentClient(new Uri(endpointUrl), authorizationKey, new ConnectionPolicy { ConnectionMode = ConnectionMode.Gateway, ConnectionProtocol = Protocol.Https });
            database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;
        }

        public List<CollectorLog> Logs()
        {
            List<CollectorLog> logs = new List<CollectorLog>();

            string name = this.User.Identity.Name;

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "collector_log");

            logs = client.CreateDocumentQuery<CollectorLog>(collectionUri,
                new SqlQuerySpec(
                    "SELECT * FROM collector_log r WHERE r.UserId = @userid",
                    new SqlParameterCollection(new[]
                    {
                        new SqlParameter { Name = "@userid", Value = name }
                    }
                    )
                    ), DefaultOptions
            ).ToList();

            return logs;
        }

        public List<Data.Models.User> SimilarUsers(string algo)
        {
            List<Data.Models.User> users = new List<Data.Models.User>();

            string name = this.User.Identity.Name;

            switch (algo)
            {
                case "jaccard":
                    users = RecommendationHelper.JaccardRecommendation(name);
                    break;
                case "pearson":
                    users = RecommendationHelper.PearsonRecommendation(name);
                    break;
            }

            return users;
        }

        public List<Product> Recommend(string algo)
        {
            List<Product> products = new List<Product>();

            string name = this.User.Identity.Name;
            products = RecommendationHelper.Get(algo, name);

            return products;
        }
    }
}