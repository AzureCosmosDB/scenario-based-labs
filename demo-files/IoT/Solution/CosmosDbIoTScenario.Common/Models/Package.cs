using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Package
    {
        /// <summary>
        /// Partition: consignmentId
        /// </summary>
        [JsonProperty] public string partitionKey => consignmentId;
        [JsonProperty] public string id { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        [JsonProperty] public string entityType => WellKnown.EntityTypes.Package;
        [JsonProperty] public string tripId { get; set; }
        [JsonProperty] public string consignmentId { get; set; }
        [JsonProperty] public double height { get; set; }
        [JsonProperty] public double length { get; set; }
        [JsonProperty] public double width { get; set; }
        [JsonProperty] public double grossWeight { get; set; }
        [JsonProperty] public double storageTemperature { get; set; }
        [JsonProperty] public bool highValue { get; set; }
        [JsonProperty] public PackageTrip trip { get; set; }
        [JsonProperty] public PackageConsignment consignment { get; set; }
        [JsonProperty] public DateTime timestamp { get; set; }
    }

    public class PackageTrip
    {
        public string tripId { get; set; }
        public string vin { get; set; }
        public double plannedTripDistance { get; set; }
    }

    public class PackageConsignment
    {
        public string consignmentId { get; set; }
        public string customer { get; set; }
        public DateTime deliveryDueDate { get; set; }
    }
}
