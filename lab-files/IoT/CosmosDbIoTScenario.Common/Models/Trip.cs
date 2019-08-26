using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Trip
    {
        public string tripId { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        public string entityType => "Trip";
        public string vin { get; set; }
        public string consignmentId { get; set; }
        public double plannedTripDistance { get; set; }
        public DateTime tripStarted { get; set; }
        public DateTime tripEnded { get; set; }
        public string status { get; set; }
        public DateTime timestamp { get; set; }
    }
}
