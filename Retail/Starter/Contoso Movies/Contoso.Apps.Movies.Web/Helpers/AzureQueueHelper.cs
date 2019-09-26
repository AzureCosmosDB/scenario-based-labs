using Contoso.Apps.Movies.Data.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Contoso.Apps.Movies.Web.Helpers
{
    public class AzureQueueHelper
    {
        CloudStorageAccount storageAccount;
        CloudQueueClient queueClient;
        CloudQueue queue;

        public AzureQueueHelper()
        {
            // Retrieve the storage account from a connection string in the web.config file.
            storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["AzureQueueConnectionString"]);

            // Create the cloud queue client.
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to our queue.
            queue = queueClient.GetQueueReference("receiptgenerator");
        }

        /// <summary>
        /// Create a message in our Azure Queue, which will be sent to our Worker Role in order
        /// to generate a Pdf file that gets saved to blob storage, and can be emailed to the client.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task QueueReceiptRequest(Order order)
        {
            // Create the queue if it doesn't already exist.
            if (await queue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Queue '{0}' Created", queue.Name);
            }
            else
            {
                Console.WriteLine("Queue '{0}' Exists", queue.Name);
            }

            String jsonOrder = JsonConvert.SerializeObject(order);
            // Create a message and add it to the queue.
            CloudQueueMessage message = new CloudQueueMessage(jsonOrder);
            
            // Async enqueue the message.
            await queue.AddMessageAsync(message);
        }
    }
}