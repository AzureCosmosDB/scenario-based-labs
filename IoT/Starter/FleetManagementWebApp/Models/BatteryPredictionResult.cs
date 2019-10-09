using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FleetManagementWebApp.Models
{
    public partial class BatteryPredictionResult
    {
        [JsonProperty("result")]
        public double[] Result { get; set; }
    }

    public partial class BatteryPredictionResult
    {
        public static BatteryPredictionResult FromJson(string json) => JsonConvert.DeserializeObject<BatteryPredictionResult>(json, FleetManagementWebApp.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this BatteryPredictionResult self) => JsonConvert.SerializeObject(self, FleetManagementWebApp.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
