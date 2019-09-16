using Contoso.Apps.Movies.Data.Models;
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
        public static DocumentClient client;
        public static string databaseId;

        public static async Task<DocumentCollection> GetOrCreateCollectionAsync(string collectionId)
        {
            DocumentCollection collectionDefinition = new DocumentCollection();
            collectionDefinition.Id = collectionId;
            collectionDefinition.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });
            collectionDefinition.PartitionKey.Paths.Add("/id");

            return await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(databaseId),
                collectionDefinition,
                new RequestOptions { OfferThroughput = 400 });
        }

        public static Document GetObject(object id, string type)
        {
            FeedOptions defaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            //get the product
            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var query = client.CreateDocumentQuery<Document>(objCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE (f.ObjectId = @id and f.EntityType = @type)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@type", type)
                    }
            }, defaultOptions);

            Document doc = query.ToList().FirstOrDefault();

            return doc;
        }

        public static Document SaveObject(Document doc, DbObject o)
        {
            Document blah = null;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            if (doc != null)
            {
                blah = client.ReplaceDocumentAsync(doc.SelfLink, doc).Result;
            }
            else
            {
                try
                {
                    blah = client.UpsertDocumentAsync(collectionUri, o).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return blah;
        }

        public static Document SaveObject(DbObject o)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var blah = client.UpsertDocumentAsync(collectionUri, o).Result;

            return blah;
        }

        public static List<Item> GetMoviesByType(int id)
        {
            FeedOptions defaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            //get the product
            Uri objCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var query = client.CreateDocumentQuery<Item>(objCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE (f.CategoryId = @id)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id)
                    }
            }, defaultOptions);

            return query.ToList();
        }

        public static void GenerateAction(int userId, string itemId, string eventType, string sessionId)
        {
            Console.WriteLine($"{userId} performed {eventType}");

            CollectorLog log = new CollectorLog();
            log.UserId = userId;
            log.ItemId = itemId;
            log.Event = eventType;
            log.SessionId = sessionId;
            log.Created = DateTime.Now;

            //add to cosmos db
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "events");
            var item = client.UpsertDocumentAsync(collectionUri, log);
        }

        public static Item GetItem(int? itemId)
        {
            FeedOptions defaultOptions = new FeedOptions { EnableCrossPartitionQuery = false };

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var query = client.CreateDocumentQuery<Item>(productCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE f.ItemId = @id and f.EntityType = 'Item'",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", itemId)
                    }
            }, defaultOptions);

            Item product = query.ToList().FirstOrDefault();

            return product;
        }

        public void CreateCollections()
        {
            var col = GetOrCreateCollectionAsync("events");
            col = GetOrCreateCollectionAsync("object");
            col = GetOrCreateCollectionAsync("associations");
            col = GetOrCreateCollectionAsync("order");
            col = GetOrCreateCollectionAsync("orderdetail");
            col = GetOrCreateCollectionAsync("shoppingcartitem");
            col = GetOrCreateCollectionAsync("similarity");
        }
    }
}