using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.Common
{
    public class DbHelper
    {
        public static CosmosClient client;
        public static string databaseId;

        public static async Task<ContainerResponse> GetOrCreateCollectionAsync(string collectionId)
        {
            return await client.GetDatabase(databaseId).CreateContainerIfNotExistsAsync(collectionId, "id", 400);
        }

        /// <summary>
        /// Retrieves an individual entity from the database.
        /// </summary>
        /// <typeparam name="T">The Type of entity to retrieve.</typeparam>
        /// <param name="id">The entity's ObjectId.</param>
        /// <param name="type">The entity type.</param>
        /// <param name="partitionKeyValue">If provided, the partition key is added to the query and a cross-partition query is avoided.</param>
        /// <returns></returns>
        public static async Task<T> GetObject<T>(object id, string type, string partitionKeyValue)
        {
            var container = client.GetContainer(databaseId, "object");

            var queryDef = new QueryDefinition("SELECT * FROM object f WHERE f.ObjectId = @id and f.EntityType = @type").WithParameter("@id", id.ToString()).WithParameter("@type", type);

            var options = new QueryRequestOptions {MaxItemCount = 1};

            // Include the partition key value if provided to avoid cross-partition queries.
            if (!string.IsNullOrWhiteSpace(partitionKeyValue))
            {
                options.PartitionKey = new PartitionKey(partitionKeyValue);
            }

            FeedIterator<T> setIterator = container.GetItemQueryIterator<T>(queryDef, requestOptions: options);

            while (setIterator.HasMoreResults)
            {
                foreach (T item in await setIterator.ReadNextAsync())
                {
                    return item;
                }
            }

            return default(T);
        }

        public async static Task<bool> DeleteObject(DbObject o)
        {
            var container = client.GetContainer(databaseId, "object");
            var blah = await container.DeleteItemAsync<DbObject>(o.ObjectId, new PartitionKey(o.EntityType));
            return true;
        }

        /*
        public static DbObject SaveObject(DbObject o)
        {
            try
            {
                var container = client.GetContainer(databaseId, "object");
                DbObject blah = container.UpsertItemAsync(o).Result;
                return blah;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        */

        public async static Task SaveObject(DbObject o)
        {
            var container = client.GetContainer(databaseId, "object");
            
            ItemResponse<DbObject> blah = null;
            
            if (o.id != null)
            {
                blah = await container.ReplaceItemAsync(o, o.id);

                //blah = client.ReplaceDocumentAsync(doc.SelfLink, doc).Result;
            }
            else
            {
                try
                {
                    o.id = Guid.NewGuid().ToString();
                    await container.CreateItemAsync(o);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static List<CollectorLog> GetUserLogs(int userId, int v)
        {
            /*
                logs = client.CreateDocumentQuery<CollectorLog>(collectionUri,
                    new SqlQuerySpec(
                        "SELECT * FROM events r WHERE r.userId = @userid",
                        new SqlParameterCollection(new[]
                        {
                        new SqlParameter { Name = "@userid", Value = userId.ToString() }
                        }
                        )
                        ), DefaultOptions
                ).ToList().Take(100).ToList();
                */

            return new List<CollectorLog>();
        }

        public static List<Item> GetMoviesByType(int id)
        {
            var container = client.GetContainer(databaseId, "object");
            var query = container.GetItemLinqQueryable<Item>(true).Where(c => c.CategoryId == id);
            return query.ToList();
        }

        public async static void GenerateAction(int userId, string itemId, string eventType, string sessionId)
        {
            Console.WriteLine($"{userId} performed {eventType}");

            CollectorLog log = new CollectorLog();
            log.id = Guid.NewGuid().ToString();
            log.UserId = userId.ToString();
            log.ItemId = itemId;
            log.Event = eventType;
            log.SessionId = sessionId;
            log.Created = DateTime.Now;

            if (eventType == "buy")
                log.OrderId = Guid.NewGuid().ToString().Replace("-", "");

            //add to cosmos db
            var container = client.GetContainer(databaseId, "events");
            var item = await container.CreateItemAsync(log);
        }

        public static async Task<Item> GetItem(int? itemId)
        {
            var objectId = $"Item_{itemId}";
            Item i = await DbHelper.GetObject<Item>(objectId, "Item", objectId);
            return i;
        }

        public void CreateCollections()
        {
            var col = GetOrCreateCollectionAsync("events");
            col = GetOrCreateCollectionAsync("object");
            col = GetOrCreateCollectionAsync("associations");
            col = GetOrCreateCollectionAsync("similarity");
        }

        internal async static Task<Item> GetItem(int itemId)
        {
            var objectId = $"Item_{itemId}";
            return await DbHelper.GetObject<Item>(objectId, "Item", objectId);

            /*
            FeedOptions defaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");

            var query = client.CreateDocumentQuery<Item>(productCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE (f.ObjectId = @id)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", itemId)
                    }
            }, defaultOptions);

            Item product = query.ToList().FirstOrDefault();

            return product;
            */
        }

        internal async static Task<Category> GetCategory(int id)
        {
            return await DbHelper.GetObject<Category>($"Category_{id}", "Category", "Category");

            /*
            FeedOptions defaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var query = client.CreateDocumentQuery<Category>(productCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE (f.CategoryId = @id) and f.EntityType = 'Category'",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id)
                    }
            }, defaultOptions);

            Category product = query.ToList().FirstOrDefault();

            return product;
            */
        }

        public IQueryable<T> GetQuery<T>(string type)
        {
            IQueryable<T> query = null;

            switch (type)
            {
                case "user":
                    break;
            }

            return query;
        }
    }
}