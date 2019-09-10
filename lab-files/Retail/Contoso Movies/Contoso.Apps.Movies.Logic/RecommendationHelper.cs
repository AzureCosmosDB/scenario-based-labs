using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

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

        public static List<Movies.Data.Models.Item> ContentBasedRecommendation(int contentId, int take)
        {
            return GetRandom(take);
        }

        //aka NeighborhoodBasedRecs
        public static List<Movies.Data.Models.Item> CollaborationBasedRecommendation(int userId, int take)
        {
            return GetRandom(take);

            int neighborhoodSize = 15;
            decimal minSim = 0.0m;
            int maxCandidates = 100;

            List<ItemRating> userRatedItems = GetRatedItems(userId);

            DateTime start = DateTime.Now;
            int[] movieIds = null;

            //this is the mean rating a user gave (python code looks odd and maybe wrong)
            decimal ratingSum = 0;

            foreach(ItemRating r in userRatedItems)
            {
                ratingSum += r.Rating;
            }

            decimal userMean = ratingSum / userRatedItems.Count;

            //get similar items
            List<SimilarItem> candidateItems = null;

            //sort by similarity, take only max candidates
            candidateItems = candidateItems.Take(maxCandidates).ToList();

            Hashtable recs = new Hashtable();

            foreach(SimilarItem candidate in candidateItems)
            {
                int target = candidate.Target;
                decimal pre = 0;
                decimal simSum = 0;

                List<SimilarItem> ratedItems = null;

                if (ratedItems.Count > 1)
                {
                    foreach(SimilarItem simItem in ratedItems)
                    {
                        decimal r = 0; //rating of the movie - userMean;
                        pre += simItem.Similarity * r;
                        simSum += simItem.Similarity;

                        if (simSum > 0)
                        {
                            PredictionModel p = new PredictionModel();
                            p.Prediction = userMean + pre / simSum;
                            p.Items = ratedItems;
                            recs.Add(target, p);
                        }
                    }
                }
            }

        }

        private static List<ItemRating> GetRatedItems(int userId)
        {
            return new List<ItemRating>();
        }

        public static List<Movies.Data.Models.Item> MatrixFactorRecommendation(int userId, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Item> HybridRecommendation(int userId, int take)
        {
            return GetRandom(take);
        }

        public static List<Movies.Data.Models.Item> RankingRecommendation(int userId, int take)
        {
            return GetRandom(take);
        }

        public static List<Data.Models.User> JaccardRecommendation(int userId)
        {
            return new List<Data.Models.User>();
        }

        public static List<Data.Models.User> PearsonRecommendation(int userId)
        {
            return new List<Data.Models.User>();
        }

        public static List<Item> GetViaFunction(string algo, int userId, int contentId)
        {
            return GetViaFunction(algo, userId, contentId, 6);
        }

        public static List<Item> GetViaFunction(string algo, int userId, int contentId, int take)
        {
            List<Item> items = new List<Item>();

            string funcUrl = "";

            dynamic request = new System.Dynamic.ExpandoObject();
            request.Algo = algo;
            request.UserId = userId;
            request.ContentId = contentId;
            request.Take = take;

            string json = JsonConvert.DeserializeObject(request);

            switch (algo)
            {
                case "assoc":
                case "assocUser":
                    items = RecommendationHelper.AssociationRecommendationByUser(userId, take);
                    break;
                case "assocContent":
                    items = RecommendationHelper.AssociationRecommendationByContent(contentId, take);
                    break;
                case "content":
                    items = RecommendationHelper.ContentBasedRecommendation(contentId, take);
                    break;
                case "collab":
                    items = RecommendationHelper.CollaborationBasedRecommendation(userId, take);
                    break;
                case "matrix":
                    items = RecommendationHelper.MatrixFactorRecommendation(userId, take);
                    break;
                case "hybrid":
                    items = RecommendationHelper.HybridRecommendation(userId, take);
                    break;
                case "ranking":
                    items = RecommendationHelper.RankingRecommendation(userId, take);
                    break;
            }
            return items;
        }

        public static List<Item> Get(string algo, int userId, int contentId)
        {
            return Get(algo, userId, contentId, 6);
        }

        public static List<Item> Get(string algo, int userId, int contentId, int take)
        {
            List<Item> items = new List<Item>();

            switch (algo)
            {
                case "assoc":
                    items = RecommendationHelper.AssociationRecommendationByUser(userId, take);
                    break;
                case "content":
                    items = RecommendationHelper.ContentBasedRecommendation(userId, take);
                    break;
                case "collab":
                    items = RecommendationHelper.CollaborationBasedRecommendation(userId, take);
                    break;
                case "matrix":
                    items = RecommendationHelper.MatrixFactorRecommendation(userId, take);
                    break;
                case "hybrid":
                    items = RecommendationHelper.HybridRecommendation(userId, take);
                    break;
                case "ranking":
                    items = RecommendationHelper.RankingRecommendation(userId, take);
                    break;
            }

            return items;
        }
    }
}
