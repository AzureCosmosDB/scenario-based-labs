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
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq;

namespace ContosoFunctionApp
{
    public static class FuncTopItems
    {
        static FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };

        static DocumentClient client;
        static Database database;
        static string databaseId;
        static DocumentCollection productColl, shoppingCartItems;

        [FunctionName("TopItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            List<Item> items = new List<Item>();

            log.LogInformation($"Webhook was triggered!");

            try
            {
                string jsonContent = await req.ReadAsStringAsync();
                dynamic payload = JsonConvert.DeserializeObject(jsonContent);

                if (payload != null && payload.UserId != null)
                {
                    Uri productCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId, "item");

                    if (items == null)
                        items = client.CreateDocumentQuery<Item>(productCollectionUri, "SELECT * FROM item f ORDER BY f.BuyCount desc OFFSET 0 LIMIT 20 ", DefaultOptions).ToList();

                    return new OkObjectResult(items);
                }
                else
                {
                    var error = $"Error Processing Recommendation: No UserId";
                    return new BadRequestObjectResult(error);
                }
            }
            catch (Exception ex)
            {
                var error = $"Error Processing Recommendation: {ex.Message}";
                return new BadRequestObjectResult(error);
            }
        }        
    }
}
