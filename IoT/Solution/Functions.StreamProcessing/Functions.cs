﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Messaging.EventHubs;

namespace Functions.StreamProcessing
{
    public static class Functions
	{
		[FunctionName(nameof(IoTHubTrigger))]
		public static async Task IoTHubTrigger([IoTHubTrigger(WellKnown.IOT_HUB_NAME, Connection = WellKnown.IOT_HUB_CONNECTION_NAME)] EventData[] vehicleEventData,
			[CosmosDB(
				databaseName: WellKnown.COSMOSDB_DB_NAME,
				collectionName: WellKnown.COSMOSDB_COLLECTION_NAME_TELEMETRY,
				ConnectionStringSetting = WellKnown.COSMOSDB_CONNECTIONSTRING_NAME)]
			IAsyncCollector<VehicleEvent> vehicleTelemetryOut,
			ILogger log)
		{
			var exceptions = new List<Exception>();
			log.LogInformation($"IoT Hub trigger function processing {vehicleEventData.Length} events.");

			foreach (var eventData in vehicleEventData)
			{
				try
				{
					var messageBody = Encoding.UTF8.GetString(eventData.EventBody.ToArray());
					var vehicleEvent = JsonConvert.DeserializeObject<VehicleEvent>(messageBody);

					// Update the partitionKey value.
					// The partitionKey property represents a synthetic composite partition key for the
					// Cosmos DB container, consisting of the VIN + current year/month. Using a composite
					// key instead of simply the VIN provides us with the following benefits:
					// (1) Distributing the write workload at any given point in time over a high cardinality
					// of partition keys.
					// (2) Ensuring efficient routing on queries on a given VIN - you can spread these across
					// time, e.g. SELECT * FROM c WHERE c.partitionKey IN (�VIN123-2019-01�, �VIN123-2019-02�, �)
					// (3) Scale beyond the 10GB quota for a single partition key value.
					vehicleEvent.partitionKey = $"{vehicleEvent.vin}-{DateTime.UtcNow:yyyy-MM}";

					// Set the TTL to expire the document after 60 days.
					vehicleEvent.ttl = 60 * 60 * 24 * 60;

					vehicleEvent.timestamp = DateTime.UtcNow;

					await vehicleTelemetryOut.AddAsync(vehicleEvent);
				}
				catch (Exception e)
				{
					// We need to keep processing the rest of the batch - capture this exception and continue.
					// Also, consider capturing details of the message that failed processing so it can be processed again later.
					exceptions.Add(e);
				}
			}

			// Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

			if (exceptions.Count > 1)
				throw new AggregateException(exceptions);

			if (exceptions.Count == 1)
				throw exceptions.Single();
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
			var ioTHubConnection = Environment.GetEnvironmentVariable(WellKnown.IOT_HUB_CONNECTION_NAME);

			var variableList = new List<string>();
			if (string.IsNullOrWhiteSpace(cosmosDbConnection)) variableList.Add(WellKnown.COSMOSDB_CONNECTIONSTRING_NAME);
			if (string.IsNullOrWhiteSpace(ioTHubConnection)) variableList.Add(WellKnown.IOT_HUB_CONNECTION_NAME);

			if (variableList.Count > 0)
			{
                return new BadRequestObjectResult($"The service is missing one or more application settings: {string.Join(", ", variableList)}");
			}

			return new OkObjectResult($"The service contains expected application settings");
		}
	}
}