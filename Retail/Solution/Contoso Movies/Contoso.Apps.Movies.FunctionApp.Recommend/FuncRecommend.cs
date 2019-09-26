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
using Contoso.Apps.Movies.Logic;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace ContosoFunctionApp
{
    public class FuncRecommend
    {
        private readonly CosmosClient _cosmosClient;

        // Use Dependency Injection to inject the HttpClientFactory service and Cosmos DB client that were configured in Startup.cs.
        public FuncRecommend(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [FunctionName("Recommend")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext ctx)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(ctx.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            List<Item> products = new List<Item>();

            log.LogInformation($"Recommend http was triggered");

            RecommendationHelper.client = _cosmosClient;
            RecommendationHelper.databaseId = config["databaseId"];
            RecommendationHelper.Init();

            try
            {
                string jsonContent = await req.ReadAsStringAsync();
                dynamic payload = JsonConvert.DeserializeObject(jsonContent);

                log.LogInformation($"Payload recv: {payload}"); 

                if (payload != null && payload.UserId != null)
                {
                    log.LogInformation($"Getting recommendations.");

                    products = RecommendationHelper.Get(payload.Algo.ToString(), (int)payload.UserId, (int)payload.ContentId, (int)payload.Take);

                    return new OkObjectResult(products);
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
