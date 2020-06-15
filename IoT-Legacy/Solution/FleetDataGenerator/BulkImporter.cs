using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using CosmosDbIoTScenario.Common.Models;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.CosmosDB.BulkExecutor.BulkImport;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using CosmosDbIoTScenario.Common;

namespace FleetDataGenerator
{
    /// <summary>
    /// Uses the bulk executor library to perform bulk import operations into Cosmos DB.
    /// </summary>
    public class BulkImporter
    {
        private readonly ConnectionPolicy _connectionPolicy = new ConnectionPolicy
        {
            ConnectionMode = ConnectionMode.Direct,
            ConnectionProtocol = Protocol.Tcp
        };

        private readonly DocumentClient _client;

        public BulkImporter(CosmosDbConnectionString cosmosDbConnectionString)
        {
            _client = new DocumentClient(
                cosmosDbConnectionString.ServiceEndpoint,
                cosmosDbConnectionString.AuthKey,
                _connectionPolicy);
        }

        public async Task BulkImport<T>(IEnumerable<T> documents,
            string databaseName, string collectionName,
            CancellationToken cancellationToken, int numberOfBatchesEachCollection = 10) where T : class
        {
            var dataCollection = GetCollectionIfExists(_client, databaseName, collectionName);
            if (dataCollection == null)
            {
                throw new Exception("The data collection does not exist");
            }

            // Set retry options high for initialization (default values).
            _client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 30;
            _client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 9;

            // IMPORTANT! CURRENTLY, ONLY THE FOLLOWING NUGET PACKAGE WORKS WITH THE BULKEXECUTOR: Microsoft.Azure.DocumentDB.Core 2.1.3 (https://github.com/Azure/azure-cosmos-dotnet-v2/issues/618)
            IBulkExecutor bulkExecutor = new BulkExecutor(_client, dataCollection);
            await bulkExecutor.InitializeAsync();

            // Set retries to 0 to pass control to bulk executor.
            _client.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds = 0;
            _client.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests = 0;

            BulkImportResponse bulkImportResponse = null;
            long totalNumberOfDocumentsInserted = 0;
            double totalRequestUnitsConsumed = 0;
            double totalTimeTakenSec = 0;
            var numberOfDocumentsPerBatch = (int)Math.Floor(((double)documents.Count()) / numberOfBatchesEachCollection);

            // Divide into batches of documents per desired number of documents per batch.
            var documentsToImportInBatch = documents.Partition(numberOfDocumentsPerBatch).ToList()
                .Select(x => x.ToList())
                .ToList();

            // Bulk write documents:
            foreach (var documentsToImport in documentsToImportInBatch)
            {
                var tasks = new List<Task>
                {
                    Task.Run(async () =>
                        {
                            do
                            {
                                try
                                {
                                    bulkImportResponse = await bulkExecutor.BulkImportAsync(
                                        documents: documentsToImport,
                                        enableUpsert: true,
                                        disableAutomaticIdGeneration: true,
                                        maxConcurrencyPerPartitionKeyRange: null,
                                        maxInMemorySortingBatchSize: null,
                                        cancellationToken: cancellationToken);
                                }
                                catch (DocumentClientException de)
                                {
                                    Console.WriteLine("Document client exception: {0}", de);
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Exception: {0}", e);
                                    break;
                                }
                            } while (bulkImportResponse.NumberOfDocumentsImported < documentsToImport.Count);

                            totalNumberOfDocumentsInserted += bulkImportResponse.NumberOfDocumentsImported;
                            totalRequestUnitsConsumed += bulkImportResponse.TotalRequestUnitsConsumed;
                            totalTimeTakenSec += bulkImportResponse.TotalTimeTaken.TotalSeconds;
                        },
                        cancellationToken)
                };

                await Task.WhenAll(tasks);
            }

            Console.WriteLine("Bulk import summary:");
            Console.WriteLine("--------------------------------------------------------------------- ");
            Console.WriteLine(String.Format("Inserted {0} docs @ {1} writes/s, {2} RU/s in {3} sec",
                totalNumberOfDocumentsInserted,
                Math.Round(totalNumberOfDocumentsInserted / totalTimeTakenSec),
                Math.Round(totalRequestUnitsConsumed / totalTimeTakenSec),
                totalTimeTakenSec));
            Console.WriteLine(String.Format("Average RU consumption per document: {0}",
                (totalRequestUnitsConsumed / totalNumberOfDocumentsInserted)));
            Console.WriteLine("--------------------------------------------------------------------- ");
        }

        /// <summary>
        /// Get the collection if it exists, null if it doesn't.
        /// </summary>
        /// <returns>The requested collection.</returns>
        internal static DocumentCollection GetCollectionIfExists(DocumentClient client, string databaseName, string collectionName)
        {
            if (GetDatabaseIfExists(client, databaseName) == null)
            {
                return null;
            }

            return client.CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(databaseName))
                .Where(c => c.Id == collectionName).AsEnumerable().FirstOrDefault();
        }

        /// <summary>
        /// Get the database if it exists, null if it doesn't.
        /// </summary>
        /// <returns>The requested database.</returns>
        internal static Database GetDatabaseIfExists(DocumentClient client, string databaseName)
        {
            return client.CreateDatabaseQuery().Where(d => d.Id == databaseName).AsEnumerable().FirstOrDefault();
        }

    }
}
