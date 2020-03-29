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
        private readonly CosmosClient _cosmosClient;
        private IHttpClientFactory _httpClientFactory;
        private ILogger log;
        private IConfigurationRoot config;

        // Use Dependency Injection to inject the HttpClientFactory service that was configured in Startup.cs.
        public FuncChangeFeed(IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
        {
            _httpClientFactory = httpClientFactory;
            _cosmosClient = cosmosClient;
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

            var databaseName = "movies";

            //config
            config = new ConfigurationBuilder()
                .SetBasePath(ctx.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            DbHelper.client = _cosmosClient;
            DbHelper.databaseId = databaseName;

            //TODO #1 - Aggregate into cosmos for top products calcluation
            DoAggregateCalculations(events);

            //TODO #2 - Event Hub
            AddEventToEventHub(events);

            //TODO #3 - Fire the logic app
            CallLogicApp(events);            
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

        public async void CallLogicApp(IReadOnlyList<Document> events)
        {
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
        }

        public void AddEventToEventHub(IReadOnlyList<Document> events)
        {
            try
            {
                //event hub connection
                EventHubClient eventHubClient;
                string EventHubConnectionString = config["eventHubConnection"];
                string EventHubName = config["eventHub"];

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
        }
    }
}
