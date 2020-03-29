using Contoso.Apps.Common;
using Contoso.Apps.Movies.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ContosoFunctionApp
{
    public class FuncChangeFeed
    {
        private IHttpClientFactory _httpClientFactory;
        private CosmosClient _cosmosClient;
        private ILogger log;
        private IConfigurationRoot config;

        // Use Dependency Injection to inject the HttpClientFactory service that was configured in Startup.cs.
        public FuncChangeFeed(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [FunctionName("ChangeFeed")]
        public void Run(
            [CosmosDBTrigger(
            databaseName: "movies",
            collectionName: "events",
            ConnectionStringSetting = "CosmosDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true,
            StartFromBeginning = false)]IReadOnlyList<Document> events,
            ILogger inlog, 
            ExecutionContext ctx)
        {
            log = inlog;

            var databaseId = "movies";

            //config
            config = new ConfigurationBuilder()
                .SetBasePath(ctx.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            //cosmob connection
            CosmosClient client = new CosmosClient(config["dbConnectionUrl"], config["dbConnectionKey"]);
            
            DbHelper.client = client;
            DbHelper.databaseId = databaseId;

            DoAggregateCalculations(events);

            //TODO #5 - Event Hub
            

            //TODO #6 - Fire the logic app
            
        }        

        public async void DoAggregateCalculations(IReadOnlyList<Document> events)
        {
            try
            {
                if (events != null && events.Count > 0)
                {
                    //do the aggregate for each product...
                    foreach (var group in events.GroupBy(singleEvent => singleEvent.GetPropertyValue<int>("ItemId")))
                    {
                        int itemId = group.TakeLast<Document>(1).FirstOrDefault().GetPropertyValue<int>("ItemId");

                        //get the item aggregate record
                        ItemAggregate doc = await DbHelper.GetObject<ItemAggregate>(itemId, "ItemAggregate", itemId.ToString());

                        ItemAggregate agg = new ItemAggregate();

                        if (doc != null)
                        {
                            agg.BuyCount += group.Where(p => p.GetPropertyValue<string>("Event") == "buy").Count<Document>();
                            agg.ViewDetailsCount += group.Where(p => p.GetPropertyValue<string>("Event") == "details").Count<Document>();
                            agg.AddToCartCount += group.Where(p => p.GetPropertyValue<string>("Event") == "addToCart").Count<Document>();

                            /*
                            agg = (dynamic)doc;
                            doc.SetPropertyValue("BuyCount", agg.BuyCount += group.Where(p => p.GetPropertyValue<string>("Event") == "buy").Count<Document>());
                            doc.SetPropertyValue("ViewDetailsCount", agg.ViewDetailsCount += group.Where(p => p.GetPropertyValue<string>("Event") == "details").Count<Document>());
                            doc.SetPropertyValue("AddToCartCount", agg.AddToCartCount += group.Where(p => p.GetPropertyValue<string>("Event") == "addToCart").Count<Document>());
                            */
                        }
                        else
                        {
                            agg.ItemId = itemId;
                            agg.BuyCount += group.Where(p => p.GetPropertyValue<string>("Event") == "buy").Count<Document>();
                            agg.ViewDetailsCount += group.Where(p => p.GetPropertyValue<string>("Event") == "details").Count<Document>();
                            agg.AddToCartCount += group.Where(p => p.GetPropertyValue<string>("Event") == "addToCart").Count<Document>();
                        }

                        await DbHelper.SaveObject(agg);
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
