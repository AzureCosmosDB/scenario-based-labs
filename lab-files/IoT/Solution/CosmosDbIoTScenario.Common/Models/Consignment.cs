using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Consignment
    {
        [JsonProperty] public string partitionKey => id;
        [JsonProperty] public string id { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        [JsonProperty] public string entityType => WellKnown.EntityTypes.Consignment;
        [JsonProperty] public string customer { get; set; }
        [JsonProperty] public string description { get; set; }
        [JsonProperty] public string status { get; set; }
        [JsonProperty] public DateTime deliveryDueDate { get; set; }
        /// <summary>
        /// List of package IDs associated with the consignment.
        /// </summary>
        [JsonProperty] public IEnumerable<string> packages { get; set; }
        [JsonProperty] public DateTime timestamp { get; set; }

        /// <summary>
        /// Not persisted. Used for the data generator only.
        /// </summary>
        [JsonIgnore]
        public List<Package> attachedPackages { get; set; }
    }
}
