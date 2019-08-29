using System;
using System.Collections.Generic;
using System.Text;

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
        public string partitionKey { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        public string entityType => "VehicleTelemetry";
        // Used to set the expiration policy (time to live).
        public int? ttl { get; set; }
        public string tripId { get; set; }
        public string vin { get; set; }
        public string state { get; set; }
        public string region { get; set; }
        public int outsideTemperature { get; set; }
        public int engineTemperature { get; set; }
        public int speed { get; set; }
        public int fuel { get; set; }
        public double fuelRate { get; set; }
        public int engineoil { get; set; }
        public int tirepressure { get; set; }
        public int odometer { get; set; }
        public int accelerator_pedal_position { get; set; }
        public bool parking_brake_status { get; set; }
        public bool brake_pedal_status { get; set; }
        public bool headlamp_status { get; set; }
        public string transmission_gear_position { get; set; }
        public bool ignition_status { get; set; }
        public bool windshield_wiper_status { get; set; }
        public bool abs { get; set; }
        public DateTime timestamp { get; set; }
    }
}
