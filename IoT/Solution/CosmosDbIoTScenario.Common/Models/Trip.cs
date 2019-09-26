using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Trip
    {
        /// <summary>
        /// Partition: vin
        /// </summary>
        [JsonProperty] public string partitionKey => vin;
        public string id { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        [JsonProperty] public string entityType => WellKnown.EntityTypes.Trip;
        [JsonProperty] public string vin { get; set; }
        [JsonProperty] public string consignmentId { get; set; }
        [JsonProperty] public double plannedTripDistance { get; set; }
        [JsonProperty] public string location { get; set; }
        [JsonProperty] public double odometerBegin { get; set; }
        [JsonProperty] public double odometerEnd { get; set; }
        [JsonProperty] public double temperatureSetting { get; set; }
        [JsonProperty] public DateTime? tripStarted { get; set; }
        [JsonProperty] public DateTime? tripEnded { get; set; }
        [JsonProperty] public string status { get; set; }
        [JsonProperty] public DateTime timestamp { get; set; }
        [JsonProperty] public IEnumerable<TripPackage> packages { get; set; }
        [JsonProperty] public TripConsignment consignment { get; set; }
    }

    public class TripPackage
    {
        [JsonProperty] public string packageId { get; set; }
        [JsonProperty] public double storageTemperature { get; set; }
        [JsonProperty] public bool highValue { get; set; }
    }

    public class TripConsignment
    {
        [JsonProperty] public string consignmentId { get; set; }
        [JsonProperty] public string customer { get; set; }
        [JsonProperty] public DateTime deliveryDueDate { get; set; }
    }
}
