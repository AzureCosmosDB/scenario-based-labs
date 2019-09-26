using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosDbIoTScenario.Common.Models.Alerts
{
    /// <summary>
    /// For sending alerts to Logic Apps. Contains information for sending emails.
    /// </summary>
    public class LogicAppAlert
    {
        public string id { get; set; }
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
        [JsonProperty] public bool hasHighValuePackages { get; set; }
        [JsonProperty] public double lowestPackageStorageTemperature { get; set; }
        [JsonProperty] public double lastRefrigerationUnitTemperatureReading { get; set; }
        [JsonProperty] public string customer { get; set; }
        [JsonProperty] public DateTime deliveryDueDate { get; set; }
        [JsonProperty] public string recipientEmail { get; set; }
    }
}
