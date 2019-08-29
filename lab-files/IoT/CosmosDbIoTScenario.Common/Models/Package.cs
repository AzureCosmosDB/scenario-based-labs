using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Package
    {
        public string partitionKey => packageId;
        public string packageId { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        public string entityType => "Package";
        public string tripId { get; set; }
        public string consignmentId { get; set; }
        public double height { get; set; }
        public double length { get; set; }
        public double depth { get; set; }
        public double grossWeight { get; set; }
        public string storageTemperature { get; set; }
        public bool highValue { get; set; }
        public PackageTrip trip { get; set; }
        public PackageConsignment consignment { get; set; }
        public DateTime timestamp { get; set; }
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
