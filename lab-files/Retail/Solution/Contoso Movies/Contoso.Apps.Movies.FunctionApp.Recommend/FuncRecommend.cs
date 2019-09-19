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

namespace ContosoFunctionApp
{
    public static class FuncRecommend
    {
        [FunctionName("Recommend")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext ctx)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(ctx.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            List<Item> products = new List<Item>();

            log.LogInformation($"Webhook was triggered!");

            RecommendationHelper.databaseId = config["databaseId"];
            RecommendationHelper.endpointUrl = config["dbConnectionUrl"];
            RecommendationHelper.authorizationKey = config["dbConnectionKey"];
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
