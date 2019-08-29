using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Vehicle
    {
        public string partitionKey => vin;
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        public string entityType => "Vehicle";
        // Used to set the expiration policy (time to live).
        public string vin { get; set; }
    }
}
