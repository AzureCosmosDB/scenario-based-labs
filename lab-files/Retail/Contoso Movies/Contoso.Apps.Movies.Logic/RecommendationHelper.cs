using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Contoso.Apps.Movies.Logic
{
    public class RecommendationHelper
    {
        static protected DocumentClient client;
        static protected Database database;
        static protected string databaseId;
        static protected DocumentCollection productColl, shoppingCartItems;

        protected static IQueryable<Product> products;

        protected static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        static RecommendationHelper()
        {
            string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];

            client = new DocumentClient(new Uri(endpointUrl), authorizationKey, new ConnectionPolicy { ConnectionMode = ConnectionMode.Gateway, ConnectionProtocol = Protocol.Https });
            database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "product");
            products = client.CreateDocumentQuery<Product>(productCollectionUri, "SELECT * FROM product", DefaultOptions);
        }

        public static List<Product> GetRandom(int count)
        {
            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "product");
            products = client.CreateDocumentQuery<Product>(productCollectionUri, "SELECT * FROM product", DefaultOptions);
            Random r = new Random();
            int skip = r.Next(100);
            return products.ToList().Skip(skip).Take(count).ToList();
        }

        public static List<Movies.Data.Models.Product> AssociationRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Product> ContentBasedRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Product> CollaborationBasedRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Product> MatrixFactorRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Product> HybridRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Product> RankingRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Data.Models.User> JaccardRecommendation(object name)
        {
            return new List<Data.Models.User>();
        }

        public static List<Data.Models.User> PearsonRecommendation(object name)
        {
            return new List<Data.Models.User>();
        }

        public static List<Product> Get(string algo, string name)
        {
            return Get(algo, name, 6);
        }

        public static List<Product> Get(string algo, string name, int take)
        {
            List<Product> products = new List<Product>();

            switch (algo)
            {
                case "assoc":
                    products = RecommendationHelper.AssociationRecommendation(name, take);
                    break;
                case "content":
                    products = RecommendationHelper.ContentBasedRecommendation(name, take);
                    break;
                case "collab":
                    products = RecommendationHelper.CollaborationBasedRecommendation(name, take);
                    break;
                case "matrix":
                    products = RecommendationHelper.MatrixFactorRecommendation(name, take);
                    break;
                case "hybrid":
                    products = RecommendationHelper.HybridRecommendation(name, take);
                    break;
                case "ranking":
                    products = RecommendationHelper.RankingRecommendation(name, take);
                    break;
            }

            return products;
        }
    }
}
