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
using Newtonsoft.Json;

namespace Contoso.Apps.Movies.Logic
{
    public class RecommendationHelper
    {
        static public string endpointUrl;
        static public string authorizationKey;
        static public string databaseId;

        static protected CosmosClient client;

        protected static IQueryable<Item> items;
        protected static IQueryable<Item> events;

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

            //TODO #3
            

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
            var container = client.GetContainer(databaseId, "events");

            var query = container.GetItemLinqQueryable<CollectorLog>(true)
                .Where(c => c.UserId == userId.ToString());
                
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
        public static List<string> CollaborativeBasedRecommendation(int userId, int take)
        {
            List<string> itemIds = new List<string>();

            //TODO #4

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
                var query = container.GetItemLinqQueryable<ItemRating>(true).Where(c => c.UserId == userId.ToString()).Take(take);
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
                case "collab":
                    List<string> precRecs2 = RecommendationHelper.CollaborativeBasedRecommendation(userId, take);

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
                    .Where(c => c.EntityType == "Item")
                    .Distinct();

                items = query.ToList();
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

            List<Item> topItems = new List<Item>();

            List<string> itemIds = new List<string>();

            //TODO #2 - add code below here...
            

            return topItems;
        }
    }
}
