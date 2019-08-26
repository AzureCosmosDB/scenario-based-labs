using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbIoTScenario.Common.Models
{
    public class Consignment
    {
        public string consignmentId { get; set; }
        // This property is used to indicate the type of document this is within the container.
        // This allows consumers to query documents stored within the container by the type.
        // This is needed because a container can contain any number of document types within,
        // since it does not enforce any type of schema.
        public string entityType => "Consignment";
        public string customer { get; set; }
        public string description { get; set; }
        public DateTime deliveryDueDate { get; set; }
        /// <summary>
        /// List of package IDs associated with the consignment.
        /// </summary>
        public IEnumerable<string> packages { get; set; }
        public DateTime timestamp { get; set; }
    }
}
