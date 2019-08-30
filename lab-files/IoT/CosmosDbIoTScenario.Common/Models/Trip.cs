using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Trip
    {
        [JsonProperty] public string partitionKey => tripId;
        public string tripId { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        [JsonProperty] public string entityType => "Trip";
        [JsonProperty] public string vin { get; set; }
        [JsonProperty] public string consignmentId { get; set; }
        [JsonProperty] public double plannedTripDistance { get; set; }
        [JsonProperty] public DateTime tripStarted { get; set; }
        [JsonProperty] public DateTime tripEnded { get; set; }
        [JsonProperty] public string status { get; set; }
        [JsonProperty] public DateTime timestamp { get; set; }
    }
}
