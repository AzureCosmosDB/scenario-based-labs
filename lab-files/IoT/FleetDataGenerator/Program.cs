using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;
using FleetDataGenerator.OutputHelpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FleetDataGenerator
{
    internal class Program
    {
        private static CosmosClient _cosmosDbClient;
        private static Database _database = null;
        private static IConfigurationRoot _configuration;
        private static List<SimulatedVehicle> _simulatedVehicles = new List<SimulatedVehicle>();
        private static CancellationTokenSource _cancellationSource;
        private static Dictionary<string, Task> _runningVehicleTasks;

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
            _cancellationSource = arguments.MillisecondsToRun == 0 ? new CancellationTokenSource() : new CancellationTokenSource(arguments.MillisecondsToRun);
            var cancellationToken = _cancellationSource.Token;
            var statistics = new Statistic[0];
            List<Trip> trips;

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
                CancelAll();

                // Allow the main thread to continue and exit...
                WaitHandle.Set();
            };

            // Instantiate Cosmos DB client and start sending messages:
            using (_cosmosDbClient = new CosmosClient(cosmosDbConnectionString.ServiceEndpoint.OriginalString,
                cosmosDbConnectionString.AuthKey, connectionPolicy))
            {
                await InitializeCosmosDb();

                // Find and output the container details, including # of RU/s.
                var container = _database.GetContainer(MetadataContainerName);

                var offer = await container.ReadThroughputAsync(cancellationToken);

                if (offer != null)
                {
                    var currentCollectionThroughput = offer ?? 0;
                    WriteLineInColor(
                        $"Found collection `{MetadataContainerName}` with {currentCollectionThroughput} RU/s.",
                        ConsoleColor.Green);
                }

                // Initially seed the Cosmos DB database with metadata if empty.
                await SeedDatabase(cosmosDbConnectionString, cancellationToken);
                trips = await GetTripsFromDatabase(arguments.NumberSimulatedTrucks, container);
            }

            try
            {
                // Start sending telemetry from simulated vehicles to Event Hubs:
                _runningVehicleTasks = await SetupVehicleTelemetryRunTasks(arguments.NumberSimulatedTrucks,
                    trips, arguments.EventHubConnectionString);
                var tasks = _runningVehicleTasks.Select(t => t.Value).ToList();
                while (tasks.Count > 0)
                {
                    try
                    {
                        Task.WhenAll(tasks).Wait(cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        //expected
                    }

                    tasks = _runningVehicleTasks.Where(t => !t.Value.IsCompleted).Select(t => t.Value).ToList();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The vehicle telemetry operation was canceled.");
                // No need to throw, as this was expected.
            }
            
            CancelAll();
            Console.WriteLine();
            WriteLineInColor("Done sending generated vehicle telemetry data", ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();

            // Keep the console open.
            Console.ReadLine();
            WaitHandle.WaitOne();
        }

        /// <summary>
        /// Retrieves trips from Cosmos DB.
        /// </summary>
        /// <param name="numberOfSimulatedTrucks"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private static async Task<List<Trip>> GetTripsFromDatabase(int numberOfSimulatedTrucks, Container container)
        {
            var trips = new List<Trip>();

            WriteLineInColor($"\nRetrieving trip data for {numberOfSimulatedTrucks} vehicles from Cosmos DB.", ConsoleColor.DarkCyan);

            var query = new QueryDefinition("SELECT TOP @limit * FROM c WHERE c.entityType = @entityType AND c.status = @status ORDER BY c.plannedTripDistance")
                .WithParameter("@limit", numberOfSimulatedTrucks)
                .WithParameter("@entityType", WellKnown.EntityTypes.Trip)
                .WithParameter("@status", WellKnown.Status.Pending);

            var results = container.GetItemQueryIterator<Trip>(query);

            while (results.HasMoreResults)
            {
                foreach (var trip in await results.ReadNextAsync())
                {
                    trips.Add(trip);
                }
            }

            return trips;
        }

        /// <summary>
        /// Creates the set of tasks that will send telemetry data to Event Hubs.
        /// </summary>
        /// <returns></returns>
        private static async Task<Dictionary<string, Task>> SetupVehicleTelemetryRunTasks(int numberOfSimulatedTrucks, List<Trip> trips, string eventHubsConnectionString)
        {
            var vehicleTelemetryRunTasks = new Dictionary<string, Task>();
            WriteLineInColor($"\nFound {trips.Count} trips. Setting up simulated vehicles...", ConsoleColor.Cyan);
            var vehicleNumber = 1;

            foreach (var trip in trips)
            {
                // 8% probability of a refrigeration unit failure.
                var causeRefrigerationUnitFailure = DataGenerator.GetRandomWeightedBoolean(8);
                // 30% of immediate vs. gradual failure if a failure occurs.
                var immediateFailure = DataGenerator.GetRandomWeightedBoolean(30);

                _simulatedVehicles.Add(new SimulatedVehicle(trip, causeRefrigerationUnitFailure, immediateFailure, eventHubsConnectionString, vehicleNumber));

                vehicleNumber++;
            }

            foreach (var simulatedVehicle in _simulatedVehicles)
            {
                vehicleTelemetryRunTasks.Add(simulatedVehicle.TripId, simulatedVehicle.RunVehicleSimulationAsync());
            }

            return vehicleTelemetryRunTasks;
        }

        private static void CancelAll()
        {
            foreach (var simulatedVehicle in _simulatedVehicles)
            {
                simulatedVehicle.CancelCurrentRun();
            }
            _cancellationSource.Cancel();
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

                if (string.IsNullOrWhiteSpace(eventHubConnectionString))
                {
                    throw new ArgumentException("EVENT_HUB_CONNECTION_STRING must be provided");
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

            // Set initial performance to 50,000 RU/s for bulk import performance.
            await _database.CreateContainerIfNotExistsAsync(metadataContainerDefinition, throughput: 50000);
            #endregion
        }

        private static async Task ChangeContainerPerformance(Container container, int desiredThroughput)
        {
            // Retrieve the existing throughput.
            var throughputResponse = await container.ReadThroughputAsync();

            WriteLineInColor($"\nThe {container.Id} is configured with the existing throughput: {throughputResponse}\nChanging throughput to {desiredThroughput}", ConsoleColor.White);

            // Change the throughput performance.
            await container.ReplaceThroughputAsync(desiredThroughput);

            // Verify the changed throughput.
            throughputResponse = await container.ReadThroughputAsync();

            WriteLineInColor($"\nChanged {container.Id}'s requested throughput to {throughputResponse}", ConsoleColor.Cyan);
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
        private static async Task<Container> GetContainerIfExists(string containerName)
        {
            return _database.GetContainer(containerName);
        }

        /// <summary>
        /// Seeds the Cosmos DB metadata container.
        /// </summary>
        private static async Task SeedDatabase(CosmosDbConnectionString cosmosDbConnectionString, CancellationToken cancellationToken)
        {
            // TODO: Check if data exists before seeding.
            var count = 0;
            var query = new QueryDefinition($"SELECT VALUE COUNT(1) FROM c");
            var container = await GetContainerIfExists(MetadataContainerName);
            var resultSetIterator = container.GetItemQueryIterator<int>(query, requestOptions: new QueryRequestOptions() { MaxItemCount = 1});
            
            if (resultSetIterator.HasMoreResults)
            {
                var result = await resultSetIterator.ReadNextAsync();
                if (result.Count > 0) count = result.FirstOrDefault();
            }

            if (count == 0)
            {
                WriteLineInColor("Generating data to seed database...", ConsoleColor.Cyan);

                var bulkImporter = new BulkImporter(cosmosDbConnectionString);
                var vehicles = DataGenerator.GenerateVehicles().ToList();
                var consignments = DataGenerator.GenerateConsignments(900).ToList();
                var packages = DataGenerator.GeneratePackages(consignments.ToList()).ToList();
                var trips = DataGenerator.GenerateTrips(consignments.ToList(), vehicles.ToList()).ToList();

                WriteLineInColor("Generated data to seed database. Saving metadata to Cosmos DB...", ConsoleColor.Cyan);

                // Save vehicles:
                WriteLineInColor($"Adding {vehicles.Count()} vehicles...", ConsoleColor.Green);
                await bulkImporter.BulkImport(vehicles, DatabaseName, MetadataContainerName, cancellationToken, 1);

                // Save consignments:
                WriteLineInColor($"Adding {consignments.Count()} consignments...", ConsoleColor.Green);
                await bulkImporter.BulkImport(consignments, DatabaseName, MetadataContainerName, cancellationToken, 1);

                // Save packages:
                WriteLineInColor($"Adding {packages.Count()} packages...", ConsoleColor.Green);
                await bulkImporter.BulkImport(packages, DatabaseName, MetadataContainerName, cancellationToken, 4);

                // Save trips:
                WriteLineInColor($"Adding {trips.Count()} trips...", ConsoleColor.Green);
                await bulkImporter.BulkImport(trips, DatabaseName, MetadataContainerName, cancellationToken, 1);

                WriteLineInColor("Finished seeding Cosmos DB.", ConsoleColor.Cyan);
                
                // Scale down the requested throughput (RU/s) for the metadata container:
                await ChangeContainerPerformance(container, 15000);
            }
            else
            {
                WriteLineInColor("\nCosmos DB already contains data. Skipping database seeding step...", ConsoleColor.Yellow);
            }
        }
    }
}
