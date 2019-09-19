using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.Function.Common
{
    public class DbHelper
    {
        public static DocumentClient client;
        public static string databaseId;

        public static async Task<T> GetObject<T>(object id, string type)
        {
            FeedOptions defaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            var query = client.CreateDocumentQuery<T>(productCollectionUri, new SqlQuerySpec()
            {
                QueryText = "SELECT * FROM object f WHERE (f.ObjectId = @id and f.EntityType = @type)",
                Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", id),
                        new SqlParameter("@type", type)
                    }
            }, defaultOptions);

            T product = query.ToList().FirstOrDefault();

            return product;
        }

        public async static Task<Document> SaveObject(DbObject o)
        {
            Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "object");

            Document doc = await GetObject<Document>(o.ObjectId, o.EntityType);

            Document blah = null;

            if (doc != null)
            {
                blah = await client.ReplaceDocumentAsync(doc.SelfLink, o);
            }
            else
            {
                doc = await client.UpsertDocumentAsync(productCollectionUri, o);
            }
            

            return blah;
        }
    }
}