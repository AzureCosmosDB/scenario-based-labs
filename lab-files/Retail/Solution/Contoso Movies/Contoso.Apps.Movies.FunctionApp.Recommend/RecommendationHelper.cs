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
using Microsoft.Azure.Cosmos;
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

        static protected CosmosClient client;

        //static protected DocumentCollection productColl, shoppingCartItems;

        protected static IQueryable<Item> items;
        protected static IQueryable<Item> events;

        //protected static readonly FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        static RecommendationHelper()
        {
            
        }

        public static void Init()
        {
            try
            {
                client = new CosmosClient(endpointUrl, authorizationKey);

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
            Random r = new Random();
            int skip = r.Next(100);

            var container = client.GetContainer(databaseId, "object");

            var items = container.GetItemLinqQueryable<Item>(true).Where(c => c.EntityType == "Item").Skip(skip).Take(count);

            return items.ToList();
        }

        public static List<Movies.Data.Models.Item> AssociationRecommendationByContent(int itemId, int take)
        {
            return GetRandom(take);

            //get the pre-seeded objects based on confidence

            //return the "take" number of records

        }

        public static List<Item> AssociationRecommendationByUser(int userId, int take)
        {
            List<Item> items = new List<Item>();

            List<string> itemIds = new List<string>();

            //get 20 log events for the user.
            List<CollectorLog> logs = GetUserLogs(userId, 20);

            if (logs.Count == 0)
                return items;

            List<Rule> rules = GetSeededRules(logs);

            //get the pre-seeded objects based on confidence
            List<Recommendation> recs = new List<Recommendation>();

            //for each rule returned, evaluate the confidence
            foreach (Rule r in rules)
            {
                Recommendation rec = new Recommendation();
                rec.id = int.Parse(r.target);
                rec.confidence = r.confidence;
                recs.Add(rec);

                itemIds.Add(rec.id.ToString());
            }

            items = GetItemsByImdbIds(itemIds);

            //return the "take" number of records
            return items.Take(take).ToList();
        }

        private static List<Rule> GetSeededRules(List<CollectorLog> logs)
        {
            List<Rule> rules = new List<Rule>();
            List<string> strKeys1 = new List<string>();

            foreach(CollectorLog cl in logs)
            {
                strKeys1.Add(cl.ContentId);
            }

            try
            {
                //FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
                //Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "associations");

                var container = client.GetContainer(databaseId, "associations");

                var query = container.GetItemLinqQueryable<Rule>(true)
                    .Where(c => strKeys1.Contains(c.source) && !strKeys1.Contains(c.target))
                    .OrderByDescending(c=>c.confidence);

                rules = query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return rules;
        }

        private static List<CollectorLog> GetUserLogs(int userId, int take)
        {
            /*
            FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };
            options.MaxItemCount = take;

            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "events");

            var query = client.CreateDocumentQuery<CollectorLog>(objCollectionUri, new SqlQuerySpec()
            {
                QueryText = $"SELECT * FROM events f WHERE f.userId = @userid order by f.created DESC",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@userid", userId.ToString())
                    }
            }, options);
            */

            var container = client.GetContainer(databaseId, "events");

            var query = container.GetItemLinqQueryable<CollectorLog>(true)
                .Where(c => c.UserId == userId);
                
            if (take > 0)
                return query.Take(take).ToList();
            else
                return query.ToList();
        }

        public static List<Movies.Data.Models.Item> ContentBasedRecommendation(int contentId, int take)
        {
            return GetRandom(take);
        }

        //aka NeighborhoodBasedRecs
        public static List<string> CollaborationBasedRecommendation(int userId, int take)
        {
            int neighborhoodSize = 15;
            double minSim = 0.0;
            int maxCandidates = 100;

            //inside this we do the implict rating of events for the user...
            Hashtable userRatedItems = GetRatedItems(userId, 100);

            if (userRatedItems.Count == 0)
                return new List<string>();

            //this is the mean rating a user gave
            double ratingSum = 0;

            foreach(double r in userRatedItems.Values)
            {
                ratingSum += r;
            }

            double userMean = ratingSum / userRatedItems.Count;

            //get similar items
            List<SimilarItem> candidateItems = GetCandidateItems(userRatedItems.Keys, minSim);

            //sort by similarity desc, take only max candidates
            candidateItems = candidateItems.OrderByDescending(c=>c.similarity).Take(maxCandidates).ToList();

            Hashtable recs = new Hashtable();

            List<PredictionModel> precRecs = new List<PredictionModel>();

            foreach(SimilarItem candidate in candidateItems)
            {
                int target = candidate.Target;
                double pre = 0;
                double simSum = 0;

                List<SimilarItem> ratedItems = candidateItems.Where(c=>c.Target == target).Take(neighborhoodSize).ToList();

                if (ratedItems.Count > 1)
                {
                    foreach (SimilarItem simItem in ratedItems)
                    {
                        try
                        {
                            string source = userRatedItems[simItem.sourceItemId].ToString();

                            //rating of the movie - userMean;
                            double r = double.Parse(source) - userMean;

                            pre += simItem.similarity * r;
                            simSum += simItem.similarity;

                            if (simSum > 0)
                            {
                                PredictionModel p = new PredictionModel();
                                p.Prediction = userMean + pre / simSum;
                                p.Items = ratedItems;
                                precRecs.Add(p);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }

            //sort based on the prediction, only take x of them
            List<PredictionModel> sortedItems = precRecs.OrderByDescending(c => c.Prediction).Take(take).ToList();

            List<string> itemIds = new List<string>();

            //get first model's items...
            foreach(PredictionModel pm in sortedItems)
            {
                foreach(SimilarItem ri in pm.Items)
                {
                    if (ri.targetItemId != null)
                    {
                        itemIds.Add(ri.targetItemId.ToString());
                        break;
                    }
                }
            }

            return itemIds;
        }

        private static List<SimilarItem> GetCandidateItems(ICollection keys1, double minSim)
        {
            List<SimilarItem> items = new List<SimilarItem>();

            List<string> strKeys1 = new List<string>();
            
            foreach (object key in keys1)
                strKeys1.Add(key.ToString());

            try
            {
                var container = client.GetContainer(databaseId, "similarity");
                var query = container.GetItemLinqQueryable<SimilarItem>(true)
                    .Where(c => strKeys1.Contains(c.sourceItemId) && !strKeys1.Contains(c.targetItemId) && c.similarity > minSim);

                items = query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return items;
        }

        private static Hashtable GetRatedItems(int userId, int take)
        {
            Hashtable ht = new Hashtable();
            
            //get from ratings collection (offline)
            List<ItemRating> ratedItems = GetUserRanking(userId, take);

            foreach(ItemRating ir in ratedItems)
            {
                if (!ht.ContainsKey(ir.ItemId))
                    ht.Add(ir.ItemId, ir.Rating);
            }

            return ht;

            //online code
            /*
            List<CollectorLog> ratedItems = GetUserLogs(userId, 0);

            foreach(CollectorLog ir in ratedItems)
            {
                Item i = null;

                if (ht.ContainsKey(ir.ContentId))
                {
                    i = (Item)ht[ir.ContentId];                    
                }
                else
                {
                    i = new Item();
                    i.ItemId = int.Parse(ir.ContentId);
                    i.ImdbId = ir.ContentId;
                    i.Popularity = 0;
                }

                switch(ir.Event)
                {
                    case "buy":
                        i.Popularity += 1;
                        break;
                    case "details":
                        i.Popularity += 50;
                        break;
                    case "addToCart":
                        i.Popularity += 10;
                        break;
                    case "genre":
                        i.Popularity += 15;
                        break;
                }

                ht[ir.ContentId] = i;
            }

            return ht;
            */
        }

        private static List<ItemRating> GetUserRanking(int userId, int take)
        {
            List<ItemRating> items = new List<ItemRating>();

            try
            {
                var container = client.GetContainer(databaseId, "ratings");
                var query = container.GetItemLinqQueryable<ItemRating>(true).Where(c => c.UserId == userId).Take(take);
                items = query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                case "assocUser":

                    items = RecommendationHelper.AssociationRecommendationByUser(userId, take);

                    //fall back to top items...
                    if (items.Count == 0)
                        items = RecommendationHelper.TopRecommendation(userId, take);

                    break;
                case "top":
                    items = RecommendationHelper.TopRecommendation(userId, take);
                    break;
                case "random":
                    items = GetRandom(take);
                    break;
                case "assocContent":
                    items = RecommendationHelper.AssociationRecommendationByContent(userId, take);
                    break;
                case "content":
                    items = RecommendationHelper.ContentBasedRecommendation(userId, take);
                    break;
                case "collab":
                    List<string> precRecs2 = RecommendationHelper.CollaborationBasedRecommendation(userId, take);

                    if (precRecs2.Count > 0)
                        items = GetItemsByImdbIds(precRecs2);

                    if (items.Count == 0)
                        items = RecommendationHelper.TopRecommendation(userId, take);

                    break;
            }

            return items;
        }

        private static List<Item> GetItemsByIds(List<int> itemIds)
        {
            List<Item> items = new List<Item>();

            try
            {
                var container = client.GetContainer(databaseId, "object");
                var query = container.GetItemLinqQueryable<Item>(true)
                    .Where(c => itemIds.Contains(c.ItemId))
                    .Where(c => c.EntityType == "Item");

                items = query.ToList();

                /*
                FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

                //get the product
                Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

                var query = client.CreateDocumentQuery<Item>(objCollectionUri, options)
                .Where(c => itemIds.Contains(c.ItemId))
                .Where(c => c.EntityType == "Item");

                items = query.ToList();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return items;
        }

        private static List<Item> GetItemsByImdbIds(List<string> itemIds)
        {
            List<Item> items = new List<Item>();

            try
            {
                var container = client.GetContainer(databaseId, "object");
                var query = container.GetItemLinqQueryable<Item>(true)
                    .Where(c => itemIds.Contains(c.ImdbId))
                    .Where(c => c.EntityType == "Item");

                items = query.ToList();

                /*
                FeedOptions options = new FeedOptions { EnableCrossPartitionQuery = true };

                //get the product
                Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

                var query = client.CreateDocumentQuery<Item>(objCollectionUri, options)
                .Where(c => itemIds.Contains(c.ImdbId))
                .Where(c => c.EntityType == "Item");

                items = query.ToList();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return items;
        }

        private static List<Item> TopRecommendation(int userId, int take)
        {
            List<Item> items = new List<Item>();

            var container = client.GetContainer(databaseId, "object");
            var query = container.GetItemLinqQueryable<Item>(true)
                .Where(c => c.EntityType == "Item")
                .OrderByDescending(c => c.BuyCount)
                .Take(take);

            items = query.ToList();

            /*
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
            */

            List<string> itemIds = new List<string>();

            foreach(Item i in items)
            {
                itemIds.Add(i.ItemId.ToString());
            }

            List<Item> topItems = GetItemsByImdbIds(itemIds);

            return topItems;
        }
    }
}
