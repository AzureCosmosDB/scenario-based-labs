using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models
{
    public class VehicleEvent
    {
        /// <summary>
        /// The partitionKey property represents a synthetic composite partition key for the
        /// Cosmos DB container, consisting of the VIN + current year/month. Using a composite
        /// key instead of simply the VIN provides us with the following benefits:
        /// (1) Distributing the write workload at any given point in time over a high cardinality
        /// of partition keys.
        /// (2) Ensuring efficient routing on queries on a given VIN - you can spread these across
        /// time, e.g. SELECT * FROM c WHERE c.partitionKey IN (“VIN123-2019-01”, “VIN123-2019-02”, …)
        /// (3) Scale beyond the 10GB quota for a single partition key value.
        /// </summary>
        [JsonProperty] public string partitionKey { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        [JsonProperty] public string entityType => WellKnown.EntityTypes.VehicleTelemetry;
        // Used to set the expiration policy (time to live).
        [JsonProperty] public int? ttl { get; set; }
        [JsonProperty] public string tripId { get; set; }
        [JsonProperty] public string vin { get; set; }
        [JsonProperty] public string state { get; set; }
        [JsonProperty] public string region { get; set; }
        [JsonProperty] public double outsideTemperature { get; set; }
        [JsonProperty] public double engineTemperature { get; set; }
        [JsonProperty] public int speed { get; set; }
        [JsonProperty] public int fuel { get; set; }
        [JsonProperty] public double fuelRate { get; set; }
        [JsonProperty] public int engineoil { get; set; }
        [JsonProperty] public int tirepressure { get; set; }
        [JsonProperty] public double odometer { get; set; }
        [JsonProperty] public int accelerator_pedal_position { get; set; }
        [JsonProperty] public bool parking_brake_status { get; set; }
        [JsonProperty] public bool brake_pedal_status { get; set; }
        [JsonProperty] public bool headlamp_status { get; set; }
        [JsonProperty] public int transmission_gear_position { get; set; }
        [JsonProperty] public bool ignition_status { get; set; }
        [JsonProperty] public bool windshield_wiper_status { get; set; }
        [JsonProperty] public bool abs { get; set; }
        [JsonProperty] public double refrigerationUnitKw { get; set; }
        [JsonProperty] public double refrigerationUnitTemp { get; set; }
        [JsonProperty] public DateTime timestamp { get; set; }
    }
}
