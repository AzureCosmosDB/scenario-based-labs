using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace ContosoFunctionApp
{
    public static class FuncChangeFeed
    {
        [FunctionName("ChangeFeed")]
        public static void Run(
            [CosmosDBTrigger(
            databaseName: "moviegeek",
            collectionName: "event",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> events,
            [CosmosDB(
                databaseName: "moviegeek",
                collectionName: "item",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };
            var databaseId = "moviegeek";

            if (events != null && events.Count > 0)
            {
                //do the aggregate for each product...
                foreach (var group in events.GroupBy(singleEvent => singleEvent.GetPropertyValue<int>("ContentId")))
                {
                    int itemId = group.TakeLast<Document>(1).FirstOrDefault().GetPropertyValue<int>("ContentId");

                    //get the product
                    var database = client.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId }).Result;
                    Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");

                    var query = client.CreateDocumentQuery<Item>(productCollectionUri, new SqlQuerySpec()
                    {
                        QueryText = "SELECT * FROM item f WHERE (f.ItemId = @id)",
                        Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@id", itemId)
                    }
                    }, DefaultOptions);

                    Item product = query.ToList().FirstOrDefault();

                    if (product != null)
                    {
                        //update the product
                        product.BuyCount += group.Where(p => p.GetPropertyValue<string>("Event") == "buy").Count<Document>();
                        product.ViewDetailsCount += group.Where(p => p.GetPropertyValue<string>("Event") == "details").Count<Document>();
                        product.AddToCartCount += group.Where(p => p.GetPropertyValue<string>("Event") == "addToCart").Count<Document>();

                        var newItem = client.ReplaceDocumentAsync(productCollectionUri, product);
                    }
                }
            }
        }        
    }
}
