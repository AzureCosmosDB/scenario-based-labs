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

namespace ContosoFunctionApp
{
    public static class FuncRecommend
    {
        [FunctionName("Recommend")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            List<Item> products = new List<Item>();

            try
            {
                string jsonContent = await req.ReadAsStringAsync();
                dynamic payload = JsonConvert.DeserializeObject(jsonContent);

                if (payload != null && payload.UserId != null)
                {
                    var base64EncodedData = payload.Order.Value;
                    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);

                    log.LogInformation($"Webhook was triggered!");
 
                    products = RecommendationHelper.Get(payload.Algo, payload.UserId, payload.ContentId);

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
