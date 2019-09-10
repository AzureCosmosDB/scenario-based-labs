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

        protected static IQueryable<Item> items;
        protected static IQueryable<Item> events;

        protected static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        static RecommendationHelper()
        {
            string endpointUrl = ConfigurationManager.AppSettings["dbConnectionUrl"];
            string authorizationKey = ConfigurationManager.AppSettings["dbConnectionKey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];

            client = new DocumentClient(new Uri(endpointUrl), authorizationKey, new ConnectionPolicy { ConnectionMode = ConnectionMode.Gateway, ConnectionProtocol = Protocol.Https });
            database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");
            items = client.CreateDocumentQuery<Item>(productCollectionUri, "SELECT * FROM item", DefaultOptions);
        }

        public static List<Item> GetRandom(int count)
        {
            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");
            items = client.CreateDocumentQuery<Item>(productCollectionUri, "SELECT * FROM item", DefaultOptions);
            Random r = new Random();
            int skip = r.Next(100);
            return items.ToList().Skip(skip).Take(count).ToList();
        }

        public static List<Movies.Data.Models.Item> AssociationRecommendationByContent(int itemId, int take)
        {
            return GetRandom(take);

            //get the pre-seeded objects based on confidence

            //return the "take" number of records

        }

        public static List<Movies.Data.Models.Item> AssociationRecommendationByUser(int userId, int take)
        {
            return GetRandom(take);

            //get the log events for the user.

            //take the last 20 events as the seed

            //get the pre-seeded objects based on confidence

            //for each rule returned, evaluate the confidence

            //return the "take" number of records

        }

        public static List<Movies.Data.Models.Item> ContentBasedRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Item> CollaborationBasedRecommendation(int userId, int take)
        {
            return GetRandom(take);

            int neighborhoodSize = 15;
            decimal minSim = 0.0m;
            int maxCandidates = 100;

            List<Item> recommendedItems = GetRatedItems(userId);

            DateTime start = DateTime.Now;
            int[] movieIds = null;

            //get similar items
            List<Item> candidateItems = null;

            //sort by similarity, take only max candidates
            candidateItems = candidateItems.Take(maxCandidates);

            foreach(Item candidate in candidateItems)
            {

            }

        }

        public static List<Movies.Data.Models.Item> MatrixFactorRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Item> HybridRecommendation(string name, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Item> RankingRecommendation(string name, int take)
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

        public static List<Item> GetViaFunction(string algo, string name)
        {
            return GetViaFunction(algo, name, 6);
        }

        public static List<Item> GetViaFunction(string algo, string name, int take)
        {
            List<Item> items = new List<Item>();

            switch (algo)
            {
                case "assoc":
                    items = RecommendationHelper.AssociationRecommendation(name, take);
                    break;
                case "content":
                    items = RecommendationHelper.ContentBasedRecommendation(name, take);
                    break;
                case "collab":
                    items = RecommendationHelper.CollaborationBasedRecommendation(name, take);
                    break;
                case "matrix":
                    items = RecommendationHelper.MatrixFactorRecommendation(name, take);
                    break;
                case "hybrid":
                    items = RecommendationHelper.HybridRecommendation(name, take);
                    break;
                case "ranking":
                    items = RecommendationHelper.RankingRecommendation(name, take);
                    break;
            }
            return items;
        }

        public static List<Item> Get(string algo, string name)
        {
            return Get(algo, name, 6);
        }

        public static List<Item> Get(string algo, string name, int take)
        {
            List<Item> items = new List<Item>();

            switch (algo)
            {
                case "assoc":
                    items = RecommendationHelper.AssociationRecommendation(name, take);
                    break;
                case "content":
                    items = RecommendationHelper.ContentBasedRecommendation(name, take);
                    break;
                case "collab":
                    items = RecommendationHelper.CollaborationBasedRecommendation(name, take);
                    break;
                case "matrix":
                    items = RecommendationHelper.MatrixFactorRecommendation(name, take);
                    break;
                case "hybrid":
                    items = RecommendationHelper.HybridRecommendation(name, take);
                    break;
                case "ranking":
                    items = RecommendationHelper.RankingRecommendation(name, take);
                    break;
            }

            return items;
        }
    }
}
