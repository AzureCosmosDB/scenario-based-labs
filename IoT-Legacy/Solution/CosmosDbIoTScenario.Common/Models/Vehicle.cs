using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Vehicle
    {
        [JsonProperty] public string partitionKey => vin;
        public string id { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        [JsonProperty] public string entityType => WellKnown.EntityTypes.Vehicle;
        [JsonProperty] public string vin { get; set; }
        [JsonProperty] public DateTime lastServiceDate { get; set; }
        [JsonProperty] public int batteryAgeDays { get; set; }
        /// <summary>
        /// Typical battery cycle rating is 200.
        /// </summary>
        [JsonProperty] public double batteryRatedCycles { get; set; }
        [JsonProperty] public double lifetimeBatteryCyclesUsed { get; set; }
        [JsonProperty] public double averageDailyTripDuration { get; set; }
        /// <summary>
        /// Updated by prediction from deployed ML model.
        /// </summary>
        [JsonProperty] public bool batteryFailurePredicted { get; set; }
        [JsonProperty] public string stateVehicleRegistered { get; set; }
    }
}
