using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;
using CosmosDbIoTScenario.Common.Models.Alerts;
using Azure.Messaging.EventHubs;

namespace Functions.CosmosDB
{
    public class Functions
    {
        private const int LEASES_COLLECTION_THROUGHPUT = 1000;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CosmosClient _cosmosClient;

        // Use Dependency Injection to inject the HttpClientFactory service and Cosmos DB client that were configured in Startup.cs.
        public Functions(IHttpClientFactory httpClientFactory, CosmosClient cosmosClient)
        {
            _httpClientFactory = httpClientFactory;
            _cosmosClient = cosmosClient;
        }

        [FunctionName(nameof(TripProcessor))]
        public async Task TripProcessor([CosmosDBTrigger(
            databaseName: WellKnown.COSMOSDB_DB_NAME,
            containerName: WellKnown.COSMOSDB_COLLECTION_NAME_TELEMETRY,
            Connection = WellKnown.COSMOSDB_CONNECTIONSTRING_NAME,
            LeaseContainerName = WellKnown.COSMOSDB_COLLECTION_NAME_LEASES,
            LeaseContainerPrefix = WellKnown.COSMOSDB_LEASE_PREFIX_TRIPS,
            LeasesContainerThroughput = LEASES_COLLECTION_THROUGHPUT,
            CreateLeaseContainerIfNotExists = true,
            StartFromBeginning = true)]IReadOnlyList<VehicleEvent> vehicleEvents,
            ILogger log)
        {
            log.LogInformation($"Evaluating {vehicleEvents.Count} events from Cosmos DB to optionally update Trip and Consignment metadata.");

            // Retrieve the Trip records by VIN, compare the odometer reading to the starting odometer reading to calculate miles driven,
            // and update the Trip and Consignment status and send an alert if needed once completed.
            var sendTripAlert = false;
            var database = WellKnown.COSMOSDB_DB_NAME;
            var metadataContainer = WellKnown.COSMOSDB_COLLECTION_NAME_METADATA;

            if (vehicleEvents.Count > 0)
            {
                foreach (var group in vehicleEvents.GroupBy(singleEvent => singleEvent.vin))
                {
                    var vin = group.Key;
                    var odometerHigh = group.Max(item => item.odometer);
                    var averageRefrigerationUnitTemp =
                        group.Average(item => item.refrigerationUnitTemp);

                    // First, retrieve the metadata Cosmos DB container reference:
                    var container = _cosmosClient.GetContainer(database, metadataContainer);

                    // Create a query, defining the partition key so we don't execute a fan-out query (saving RUs), where the entity type is a Trip and the status is not Completed, Canceled, or Inactive.
                    var query = container.GetItemLinqQueryable<Trip>(requestOptions: new QueryRequestOptions { PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey(vin) })
                        .Where(p => p.status != WellKnown.Status.Completed
                                    && p.status != WellKnown.Status.Canceled
                                    && p.status != WellKnown.Status.Inactive
                                    && p.entityType == WellKnown.EntityTypes.Trip)
                        .ToFeedIterator();

                    if (query.HasMoreResults)
                    {
                        // Only retrieve the first result.
                        var trip = (await query.ReadNextAsync()).FirstOrDefault();

                        if (trip != null)
                        {
                            // Retrieve the Consignment record.
                            var document = await container.ReadItemAsync<Consignment>(trip.consignmentId,
                                new Microsoft.Azure.Cosmos.PartitionKey(trip.consignmentId));
                            var consignment = document.Resource;
                            var updateTrip = false;
                            var updateConsignment = false;

                            // Calculate how far along the vehicle is for this trip.
                            var milesDriven = odometerHigh - trip.odometerBegin;
                            if (milesDriven >= trip.plannedTripDistance)
                            {
                                // The trip is completed!
                                trip.status = WellKnown.Status.Completed;
                                trip.odometerEnd = odometerHigh;
                                trip.tripEnded = DateTime.UtcNow;
                                consignment.status = WellKnown.Status.Completed;

                                // Update the trip and consignment records.
                                updateTrip = true;
                                updateConsignment = true;

                                sendTripAlert = true;
                            }
                            else
                            {
                                if (DateTime.UtcNow >= consignment.deliveryDueDate && trip.status != WellKnown.Status.Delayed)
                                {
                                    // The trip is delayed!
                                    trip.status = WellKnown.Status.Delayed;
                                    consignment.status = WellKnown.Status.Delayed;

                                    // Update the trip and consignment records.
                                    updateTrip = true;
                                    updateConsignment = true;

                                    sendTripAlert = true;
                                }
                            }

                            if (trip.tripStarted == null)
                            {
                                // Set the trip start date.
                                trip.tripStarted = DateTime.UtcNow;
                                // Set the trip and consignment status to Active.
                                trip.status = WellKnown.Status.Active;
                                consignment.status = WellKnown.Status.Active;

                                updateTrip = true;
                                updateConsignment = true;

                                sendTripAlert = true;
                            }

                            // Update the trip and consignment records.
                            if (updateTrip)
                            {
                                await container.ReplaceItemAsync(trip, trip.id, new Microsoft.Azure.Cosmos.PartitionKey(trip.partitionKey));
                            }

                            if (updateConsignment)
                            {
                                await container.ReplaceItemAsync(consignment, consignment.id, new Microsoft.Azure.Cosmos.PartitionKey(consignment.partitionKey));
                            }

                            // Send a trip alert.
                            if (sendTripAlert)
                            {
                                // Have the HttpClient factory create a new client instance.
                                var httpClient = _httpClientFactory.CreateClient(WellKnown.LOGIC_APP_CLIENT);

                                // Create the payload to send to the Logic App.
                                var payload = new LogicAppAlert
                                {
                                    consignmentId = trip.consignmentId,
                                    customer = trip.consignment.customer,
                                    deliveryDueDate = trip.consignment.deliveryDueDate,
                                    hasHighValuePackages = trip.packages.Any(p => p.highValue),
                                    id = trip.id,
                                    lastRefrigerationUnitTemperatureReading = averageRefrigerationUnitTemp,
                                    location = trip.location,
                                    lowestPackageStorageTemperature = trip.packages.Min(p => p.storageTemperature),
                                    odometerBegin = trip.odometerBegin,
                                    odometerEnd = trip.odometerEnd,
                                    plannedTripDistance = trip.plannedTripDistance,
                                    tripStarted = trip.tripStarted,
                                    tripEnded = trip.tripEnded,
                                    status = trip.status,
                                    vin = trip.vin,
                                    temperatureSetting = trip.temperatureSetting,
                                    recipientEmail = Environment.GetEnvironmentVariable(WellKnown.RECIPIENT_EMAIL_NAME)
                                };

                                var postBody = JsonConvert.SerializeObject(payload);

                                await httpClient.PostAsync(Environment.GetEnvironmentVariable(WellKnown.LOGIC_APP_URL_NAME), new StringContent(postBody, Encoding.UTF8, "application/json"));
                            }
                        }
                    }
                }
            }
        }

        [FunctionName(nameof(SendToEventHubsForReporting))]
        public async Task SendToEventHubsForReporting([CosmosDBTrigger(
            databaseName: WellKnown.COSMOSDB_DB_NAME,
            containerName: WellKnown.COSMOSDB_COLLECTION_NAME_TELEMETRY,
            Connection = WellKnown.COSMOSDB_CONNECTIONSTRING_NAME,
            LeaseContainerName = WellKnown.COSMOSDB_COLLECTION_NAME_LEASES,
            LeaseContainerPrefix = WellKnown.COSMOSDB_LEASE_PREFIX_REPORTING,
            LeasesContainerThroughput = LEASES_COLLECTION_THROUGHPUT,
            CreateLeaseContainerIfNotExists = true,
            StartFromBeginning = true)]IReadOnlyList<VehicleEvent> vehicleEvents,
            [EventHub(WellKnown.EVENT_HUB_NAME, Connection = WellKnown.EVENT_HUB_CONNECTION_NAME)] IAsyncCollector<EventData> vehicleEventsOut,
            ILogger log)
        {
            log.LogInformation($"Sending {vehicleEvents.Count} Cosmos DB records to Event Hubs for reporting.");

            if (vehicleEvents.Count > 0)
            {
                foreach (var vehicleEvent in vehicleEvents)
                {
                    // Add to the Event Hub output collection.
                    await vehicleEventsOut.AddAsync(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(vehicleEvent))));
                }
            }
        }

        [FunctionName(nameof(HealthCheck))]
        public static async Task<IActionResult> HealthCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Performing health check on the Cosmos DB processing Function App.");

            // This is a very simple health check that ensures each configuration setting exists and has a value.
            // More thorough checks would validate each value against an expected format or by connecting to each service as required.
            // The function will return an HTTP status of 200 (OK) if all values contain non-zero strings.
            // If any are null or empty, the function will return an error, indicating which values are missing.

            var cosmosDbConnection = Environment.GetEnvironmentVariable(WellKnown.COSMOSDB_CONNECTIONSTRING_NAME);
            var eventHubsConnection = Environment.GetEnvironmentVariable(WellKnown.EVENT_HUB_CONNECTION_NAME);
            var logicAppUrl = Environment.GetEnvironmentVariable(WellKnown.LOGIC_APP_URL_NAME);
            var recipientEmail = Environment.GetEnvironmentVariable(WellKnown.RECIPIENT_EMAIL_NAME);

            var variableList = new List<string>();
            if (string.IsNullOrWhiteSpace(cosmosDbConnection)) variableList.Add(WellKnown.COSMOSDB_CONNECTIONSTRING_NAME);
            if (string.IsNullOrWhiteSpace(eventHubsConnection)) variableList.Add(WellKnown.EVENT_HUB_CONNECTION_NAME);
            if (string.IsNullOrWhiteSpace(logicAppUrl)) variableList.Add(WellKnown.LOGIC_APP_URL_NAME);
            if (string.IsNullOrWhiteSpace(recipientEmail)) variableList.Add(WellKnown.RECIPIENT_EMAIL_NAME);

            if (variableList.Count > 0)
            {
                return new BadRequestObjectResult($"The service is missing one or more application settings: {string.Join(", ", variableList)}");
            }

            return new OkObjectResult($"The service contains expected application settings");
        }
    }
}
