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
        static DocumentClient client;
        static string databaseId;

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

        public static List<Product> GetMoviesByType(int v)
        {
            throw new NotImplementedException();
        }

        public static void GenerateAction(int userId, string contentId, string eventType, string sessionId)
        {
            CollectorLog log = new CollectorLog();
            log.UserId = userId;
            log.ContentId = contentId;
            log.Event = eventType;
            log.SessionId = sessionId;
            log.Created = DateTime.Now;

            //add to cosmos db
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "collector_log");
            var item = client.UpsertDocumentAsync(collectionUri, log);
        }
    }

}
