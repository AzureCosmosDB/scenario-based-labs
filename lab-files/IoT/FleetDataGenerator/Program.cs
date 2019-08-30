using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common;
using FleetDataGenerator.OutputHelpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Database = Microsoft.Azure.Cosmos.Database;
using DataType = Microsoft.Azure.Cosmos.DataType;
using ExcludedPath = Microsoft.Azure.Cosmos.ExcludedPath;
using IndexingMode = Microsoft.Azure.Cosmos.IndexingMode;
using IndexingPolicy = Microsoft.Azure.Cosmos.IndexingPolicy;

namespace FleetDataGenerator
{
    internal class Program
    {
        private static CosmosClient _cosmosDbClient;
        private static Database _database = null;
        private static IConfigurationRoot _configuration;

        private const string DatabaseName = "ContosoAuto";
        private const string TelemetryContainerName = "telemetry";
        private const string MetadataContainerName = "metadata";
        private const string PartitionKey = "partitionKey";

        private static readonly object LockObject = new object();
        // AutoResetEvent to signal when to exit the application.
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);

        static async Task Main(string[] args)
        {
            // Setup configuration to either read from the appsettings.json file (if present) or environment variables.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            var arguments = ParseArguments();
            var cosmosDbConnectionString = new CosmosDbConnectionString(arguments.CosmosDbConnectionString);
            // Set an optional timeout for the generator.
            var cancellationSource = arguments.MillisecondsToRun == 0 ? new CancellationTokenSource() : new CancellationTokenSource(arguments.MillisecondsToRun);
            var cancellationToken = cancellationSource.Token;
            var statistics = new Statistic[0];

            // Set the Cosmos DB connection policy.
            var connectionPolicy = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Direct
            };

            var numberOfMillisecondsToLead = arguments.MillisecondsToLead;

            var taskWaitTime = 0;

            if (numberOfMillisecondsToLead > 0)
            {
                taskWaitTime = numberOfMillisecondsToLead;
            }

            var progress = new Progress<Progress>();
            progress.ProgressChanged += (sender, progressArgs) =>
            {
                foreach (var message in progressArgs.Messages)
                {
                    WriteLineInColor(message.Message, message.Color.ToConsoleColor());
                }
                statistics = progressArgs.Statistics;
            };

            WriteLineInColor("Fleet Data Generator", ConsoleColor.White);
            Console.WriteLine("======");
            WriteLineInColor("Press Ctrl+C or Ctrl+Break to cancel.", ConsoleColor.Cyan);
            Console.WriteLine("Statistics for generated vehicle and related telemetry data will be updated for every 500 sent");
            Console.WriteLine(string.Empty);

            // Handle Control+C or Control+Break.
            Console.CancelKeyPress += (o, e) =>
            {
                WriteLineInColor("Stopped generator. No more events are being sent.", ConsoleColor.Yellow);
                cancellationSource.Cancel();

                // Allow the main thread to continue and exit...
                WaitHandle.Set();
            };

            // Instantiate Cosmos DB client and start sending messages:
            using (_cosmosDbClient = new CosmosClient(cosmosDbConnectionString.ServiceEndpoint.OriginalString, cosmosDbConnectionString.AuthKey, connectionPolicy))
            {
                await InitializeCosmosDb();

                // Find and output the container details, including # of RU/s.
                var dataCollection = _database.GetContainer(TelemetryContainerName);

                var offer = await dataCollection.ReadThroughputAsync();

                //var offer = (OfferV2)_cosmosDbClient.CreateOfferQuery().Where(o => o.ResourceLink == dataCollection.SelfLink).AsEnumerable().FirstOrDefault();
                if (offer != null)
                {
                    var currentCollectionThroughput = offer ?? 0;
                    WriteLineInColor($"Found collection `{TelemetryContainerName}` with {currentCollectionThroughput} RU/s ({currentCollectionThroughput} reads/second; {currentCollectionThroughput / 5} writes/second @ 1KB doc size)", ConsoleColor.Green);
                    var estimatedCostPerMonth = 0.06 * currentCollectionThroughput;
                    var estimatedCostPerHour = estimatedCostPerMonth / (24 * 30);
                    WriteLineInColor($"The collection will cost an estimated ${estimatedCostPerHour:0.00} per hour (${estimatedCostPerMonth:0.00} per month (per write region))", ConsoleColor.Green);
                }

                // Start sending data to Cosmos DB.
                //SendData(100, taskWaitTime, cancellationToken, progress).Wait();
            }

            cancellationSource.Cancel();
            Console.WriteLine();
            WriteLineInColor("Done sending generated vehicle telemetry data", ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();

            // Keep the console open.
            Console.ReadLine();
            WaitHandle.WaitOne();
        }

        /// <summary>
        /// Extracts properties from either the appsettings.json file or system environment variables.
        /// </summary>
        /// <returns>
        /// EventHubConnectionString: The primary Event Hubs connection string for sending telemetry.
        /// CosmosDbConnectionString: The primary or secondary connection string copied from your Cosmos DB properties.
        /// NumberSimulatedTrucks: The number of trucks to simulate. Must be a number between 1 and 1,000.
        /// MillisecondsToRun: The maximum amount of time to allow the generator to run before stopping transmission of data. The default value is 14,400.
        /// MillisecondsToLead: The amount of time to wait before sending simulated data. Default value is 0.
        /// </returns>
        private static (string EventHubConnectionString,
            string CosmosDbConnectionString,
            int NumberSimulatedTrucks,
            int MillisecondsToRun,
            int MillisecondsToLead) ParseArguments()
        {
            try
            {
                // The Configuration object will extract values either from the machine's environment variables, or the appsettings.json file.
                var eventHubConnectionString = _configuration["EVENT_HUB_CONNECTION_STRING"];
                var cosmosDbConnectionString = _configuration["COSMOS_DB_CONNECTION_STRING"];
                var numberOfMillisecondsToRun = (int.TryParse(_configuration["SECONDS_TO_RUN"], out var outputSecondToRun) ? outputSecondToRun : 0) * 1000;
                var numberOfMillisecondsToLead = (int.TryParse(_configuration["SECONDS_TO_LEAD"], out var outputSecondsToLead) ? outputSecondsToLead : 0) * 1000;
                var numberOfSimulatedTrucks = int.TryParse(_configuration["NUMBER_SIMULATED_TRUCKS"], out var outputSimulatedTrucks) ? outputSimulatedTrucks : 0;

                if (string.IsNullOrWhiteSpace(cosmosDbConnectionString))
                {
                    throw new ArgumentException("COSMOS_DB_CONNECTION_STRING must be provided");
                }

                if (numberOfSimulatedTrucks < 1 || numberOfSimulatedTrucks > 1000)
                {
                    throw new ArgumentException("The NUMBER_SIMULATED_TRUCKS value must be a number between 1 and 1000");
                }

                return (eventHubConnectionString, cosmosDbConnectionString, numberOfSimulatedTrucks, numberOfMillisecondsToRun, numberOfMillisecondsToLead);
            }
            catch (Exception e)
            {
                WriteLineInColor(e.Message, ConsoleColor.Red);
                Console.ReadLine();
                throw;
            }
        }

        public static void WriteLineInColor(string msg, ConsoleColor color)
        {
            lock (LockObject)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }

        private static async Task InitializeCosmosDb()
        {
            _database = await _cosmosDbClient.CreateDatabaseIfNotExistsAsync(DatabaseName);

            #region Telemetry container
            // Define a new container.
            var telemetryContainerDefinition =
                new ContainerProperties(id: TelemetryContainerName, partitionKeyPath: $"/{PartitionKey}")
                {
                    IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
                };

            // Tune the indexing policy for write-heavy workloads by only including regularly queried paths.
            // Be careful when using an opt-in policy as we are below. Excluding all and only including certain paths removes
            // Cosmos DB's ability to proactively add new properties to the index.
            telemetryContainerDefinition.IndexingPolicy.ExcludedPaths.Clear();
            telemetryContainerDefinition.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/*" }); // Exclude all paths.
            telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Clear();
            telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/vin/?" });
            telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/state/?" });
            telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/tripId/?" });
            telemetryContainerDefinition.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/partitionKey/?" });

            // Create the container with a throughput of 15000 RU/s.
            await _database.CreateContainerIfNotExistsAsync(telemetryContainerDefinition, throughput: 15000);
            #endregion

            #region Metadata container
            // Define a new container (collection).
            var metadataContainerDefinition =
                new ContainerProperties(id: MetadataContainerName, partitionKeyPath: $"/{PartitionKey}")
                {
                    // Set the indexing policy to consistent and use the default settings because we expect read-heavy workloads in this container (includes all paths (/*) with all range indexes).
                    // Indexing all paths when you have write-heavy workloads may impact performance and cost more RU/s than desired.
                    IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
                };

            // Create with a throughput of 15000 RU/s.
            await _database.CreateContainerIfNotExistsAsync(metadataContainerDefinition, throughput: 15000);
            #endregion
        }

        /// <summary>
        /// Get the database if it exists, null if it doesn't.
        /// </summary>
        /// <returns>The requested database</returns>
        private static async Task<Database> GetDatabaseIfExists(string databaseName)
        {
            return await _cosmosDbClient.CreateDatabaseIfNotExistsAsync(databaseName);
        }

        /// <summary>
        /// Get the collection if it exists, null if it doesn't.
        /// </summary>
        /// <returns>The requested collection</returns>
        private static async Task<Container> GetContainerIfExists(string databaseName, string containerName)
        {
            return _database.GetContainer(containerName);
        }
    }
}
