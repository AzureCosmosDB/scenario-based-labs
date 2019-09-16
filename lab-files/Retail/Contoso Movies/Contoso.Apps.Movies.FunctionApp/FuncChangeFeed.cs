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
using Contoso.Apps.Common;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;

namespace ContosoFunctionApp
{
    public class FuncChangeFeed
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Use Dependency Injection to inject the HttpClientFactory service that was configured in Startup.cs.
        public FuncChangeFeed(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName("ChangeFeed")]
        public async Task Run(
            [CosmosDBTrigger(
            databaseName: "moviegeek",
            collectionName: "events",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> events,
            [CosmosDB(
                databaseName: "moviegeek",
                collectionName: "object",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log, ExecutionContext ctx)
        {
            FeedOptions DefaultOptions = new FeedOptions { EnableCrossPartitionQuery = true };
            var databaseId = "moviegeek";

            //config
            var config = new ConfigurationBuilder()
                .SetBasePath(ctx.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            //cosmob connection
            DbHelper.client = client;
            DbHelper.databaseId = databaseId;

            try
            {
                //event hub connection
                EventHubClient eventHubClient;
                string EventHubConnectionString = config["eventHubConnection"];
                string EventHubName = "store";

                var connectionStringBuilder = new EventHubsConnectionStringBuilder(EventHubConnectionString)
                {
                    EntityPath = EventHubName
                };

                eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

                foreach (var e in events)
                {
                    string data = JsonConvert.SerializeObject(e);
                    var result = eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(data)));
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

            try
            {
                // Have the HttpClient factory create a new client instance.
                var httpClient = _httpClientFactory.CreateClient("LogicAppClient");

                // Create the payload to send to the Logic App.
                foreach (var e in events)
                {
                    var payload = new LogicAppAlert
                    {
                        data = JsonConvert.SerializeObject(e),
                        recipientEmail = Environment.GetEnvironmentVariable("RecipientEmail")
                    };

                    var postBody = JsonConvert.SerializeObject(payload);

                    var httpResult = await httpClient.PostAsync(Environment.GetEnvironmentVariable("LogicAppUrl"), new StringContent(postBody, Encoding.UTF8, "application/json"));
                }                
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }

            try
            {
                if (events != null && events.Count > 0)
                {
                    //do the aggregate for each product...
                    foreach (var group in events.GroupBy(singleEvent => singleEvent.GetPropertyValue<int>("ItemId")))
                    {
                        int itemId = group.TakeLast<Document>(1).FirstOrDefault().GetPropertyValue<int>("ItemId");

                        //get the item aggregate record
                        Document doc = DbHelper.GetObject(itemId, "ItemAggregate");

                        ItemAggregate agg = new ItemAggregate();

                        if (doc != null)
                        {
                            agg = (dynamic)doc;
                            doc.SetPropertyValue("BuyCount", agg.BuyCount += group.Where(p => p.GetPropertyValue<string>("Event") == "buy").Count<Document>());
                            doc.SetPropertyValue("ViewDetailsCount", agg.ViewDetailsCount += group.Where(p => p.GetPropertyValue<string>("Event") == "details").Count<Document>());
                            doc.SetPropertyValue("AddToCartCount", agg.AddToCartCount += group.Where(p => p.GetPropertyValue<string>("Event") == "addToCart").Count<Document>());
                        }
                        else
                        {
                            agg.ItemId = itemId;
                            agg.BuyCount += group.Where(p => p.GetPropertyValue<string>("Event") == "buy").Count<Document>();
                            agg.ViewDetailsCount += group.Where(p => p.GetPropertyValue<string>("Event") == "details").Count<Document>();
                            agg.AddToCartCount += group.Where(p => p.GetPropertyValue<string>("Event") == "addToCart").Count<Document>();
                        }

                        DbHelper.SaveObject(doc, agg);
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
        }        
    }
}
