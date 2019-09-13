using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Logic
{
    public class RecommendationHelper
    {
        static public string endpointUrl;
        static public string authorizationKey;
        static public string databaseId;

        static protected DocumentClient client;
        static protected Database database;

        static protected DocumentCollection productColl, shoppingCartItems;

        protected static IQueryable<Item> items;
        protected static IQueryable<Item> events;

        protected static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        static RecommendationHelper()
        {
            
        }

        public static void Init()
        {
            try
            {
                client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
                database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;

                Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");
                items = client.CreateDocumentQuery<Item>(productCollectionUri, "SELECT * FROM item", DefaultOptions);

                DbHelper.client = client;
                DbHelper.databaseId = databaseId;
            }
            catch (Exception ex)
            {
                throw;
            }
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
        public static List<PredictionModel> CollaborationBasedRecommendation(int userId, int take)
        {
            int neighborhoodSize = 15;
            decimal minSim = 0.0m;
            int maxCandidates = 100;

            //inside this we do the implict rating of events for the user...
            Hashtable userRatedItems = GetRatedItems(userId, 100);
            
            //this is the mean rating a user gave (python code looks odd and maybe wrong)
            decimal ratingSum = 0;

            foreach(Item r in userRatedItems.Values)
            {
                ratingSum += r.Popularity;
            }

            decimal userMean = ratingSum / userRatedItems.Count;

            //get similar items
            List<SimilarItem> candidateItems = GetCandidateItems(userRatedItems.Keys, userRatedItems.Keys, minSim);

            //sort by similarity desc, take only max candidates
            candidateItems = candidateItems.OrderByDescending(c=>c.Similarity).Take(maxCandidates).ToList();

            Hashtable recs = new Hashtable();

            List<PredictionModel> precRecs = new List<PredictionModel>();

            foreach(SimilarItem candidate in candidateItems)
            {
                int target = candidate.Target;
                decimal pre = 0;
                decimal simSum = 0;

                List<SimilarItem> ratedItems = candidateItems.Where(c=>c.Target == target).Take(neighborhoodSize).ToList();

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
                            precRecs.Add(p);
                        }
                    }
                }
            }

            //sort based on the prediction, only take x of them
            List<PredictionModel> sortedItems = precRecs.OrderBy(c => c.Prediction).Take(take).ToList();

            return sortedItems;
        }

        private static List<SimilarItem> GetCandidateItems(ICollection keys1, ICollection keys2, decimal minSim)
        {
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

            //get the product
            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "similarity");

            var query = client.CreateDocumentQuery<SimilarItem>(objCollectionUri, new SqlQuerySpec()
            {
                QueryText = $"SELECT * FROM similarity f WHERE CONTAINS(f.SourceId , @sourceIds) and CONTAINS(f.TargetId , @targetIds) and f.Similiarty > @minSim)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@sourceIds", keys1),
                        new SqlParameter("@targetIds", keys2),
                        new SqlParameter("@minSim", minSim)
                    }
            }, options);

            List<SimilarItem> items = query.ToList();

            return items;
        }

        private static Hashtable GetRatedItems(int userId, int take)
        {
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            options.MaxItemCount = take;

            //get the product
            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "event");

            var query = client.CreateDocumentQuery<CollectorLog>(objCollectionUri, new SqlQuerySpec()
            {
                QueryText = $"SELECT * FROM event f WHERE (f.UserId = @userid)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@userid", userId)
                    }
            }, options);

            List<CollectorLog> ratedItems = query.ToList();

            Hashtable ht = new Hashtable();

            foreach(CollectorLog ir in ratedItems)
            {
                Item i = null;

                if (ht.ContainsKey(ir.ItemId))
                {
                    i = (Item)ht[ir.ItemId];                    
                }
                else
                {
                    i = new Item();
                    i.ItemId = int.Parse(ir.ItemId);
                    i.Popularity = 0;
                }

                switch(ir.Event)
                {
                    case "buy":
                        i.Popularity += 10;
                        break;
                    case "details":
                        i.Popularity += 3;
                        break;
                    case "addToCart":
                        i.Popularity += 5;
                        break;
                }

                ht[ir.ItemId] = i;
            }

            return ht;
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
                case "assocUser":
                    items = RecommendationHelper.AssociationRecommendationByUser(userId, take);

                    List<PredictionModel> precRecs1 = RecommendationHelper.CollaborationBasedRecommendation(userId, take);

                    break;
                case "top":
                    items = RecommendationHelper.TopRecommendation(userId, take);
                    break;
                case "assocContent":
                    items = RecommendationHelper.AssociationRecommendationByContent(userId, take);
                    break;
                case "content":
                    items = RecommendationHelper.ContentBasedRecommendation(userId, take);
                    break;
                case "collab":
                    List<PredictionModel> precRecs2 = RecommendationHelper.CollaborationBasedRecommendation(userId, take);

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

        private static List<Item> TopRecommendation(int userId, int take)
        {
            //return GetRandom(take);

            List<Item> items = new List<Item>();

            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            options.MaxItemCount = take;
            
            //get the product
            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var query = client.CreateDocumentQuery<Item>(objCollectionUri, new SqlQuerySpec()
            {
                QueryText = $"SELECT * FROM object f WHERE (f.EntityType = @type) order by f.BuyCount desc OFFSET 0 LIMIT {take}",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@type", "ItemAggregate")
                    }
            }, options);

            List<Item> topItems = query.ToList().Take(take).ToList();

            foreach(Item i in topItems)
            {
                Item n = null;
                Document doc = DbHelper.GetObject(i.ItemId, "Item");

                if (doc != null)
                {
                    n = (dynamic)doc;
                }
                else
                {
                    n = DbHelper.GetItem(i.ItemId);
                }

                items.Add(n);
            }

            return items;
        }
    }
}
