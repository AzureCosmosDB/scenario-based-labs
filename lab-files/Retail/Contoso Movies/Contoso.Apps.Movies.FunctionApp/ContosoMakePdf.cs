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



namespace ContosoFunctionApp
{
    public static class ContosoMakePdf
    {
        [FunctionName("ContosoMakePDF")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,         ILogger log)
        {
            OrderViewModel Order = null;

            try
            {
                string jsonContent = await req.ReadAsStringAsync();
                dynamic payload = JsonConvert.DeserializeObject(jsonContent);
                if (payload != null && payload.Order != null)
                {
                    var base64EncodedData = payload.Order.Value;
                    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                    Order = JsonConvert.DeserializeObject<OrderViewModel>(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));

                }

                if (Order == null || Order.OrderId <= 0)
                {
                    string error = "Please pass an Order property in the input object containing a Base64 representation of the Order to process. Example: { \"Order\": \"eyAiT3JkZXJJZCIgOiAiMzgiLCAiT3JkZXJEYXRlIiA6ICIyMDE3LTAzLTEwVDE5OjQ4OjAyLjZaIiwgIkZpcnN0TmFtZSIgOiAiQm9iIiwgIkxhc3ROYW1lIiA6ICJMb2JsYXciLCAiQWRkcmVzcyIgOiAiMTMxMyBNb2NraW5nYmlyZCBMYW5lIiwgIkNpdHkiIDogIlZpcmdpbmlhIEJlYWNoIiwgIlN0YXRlIiA6ICJWQSIsICJQb3N0YWxDb2RlIiA6ICIyMzQ1NiIsICJDb3VudHJ5IiA6ICJVbml0ZWQgU3RhdGVzIiwgIlBob25lIiA6ICI1NTUxMjM0NTY3OCIsICJFbWFpbCIgOiAiYm9ibG9ibGF3QGNvbnRvc29zcG9ydHNsZWFndWUuY29tIiwgIlRvdGFsIiA6ICI4NzkuNDUiIH0=\" }";
                    return new BadRequestObjectResult(error);
                }

                log.LogInformation($"Webhook was triggered! Order: {Order.OrderId} ");

                return new OkObjectResult(await ProcessOrder(Order, log));
            }
            catch (Exception ex)
            {
                var error = $"Error Processing Order: {ex.Message}";
                return new BadRequestObjectResult(error);
            }
              

        }

        static async Task<OrderViewModel> ProcessOrder(OrderViewModel Order, ILogger log)
        {
            string fileName = string.Format("ContosoSportsLeague-Store-Receipt-{0}.pdf", Order.OrderId);
            log.LogInformation($"Using Filename {fileName}");

            var receipt = await PdfUtility.CreatePdfReport(Order, fileName, log);
            log.LogInformation("PDF generated. Saving to blob storage...");
            Order.ReceiptUrl = await StorageMethods.UploadPdfToBlob(receipt, fileName, log);
            log.LogInformation($"Using Order.ReceiptUrl {Order.ReceiptUrl}");
            return Order;
        }
    }
}
