using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FleetManagementWebApp.Models
{
	public partial class BatteryPredictionResult
	{
		public Dictionary<string, List<Dictionary<string, double>>> Results { get; set; }
	}

	public partial class BatteryPredictionResult
	{
		public static BatteryPredictionResult FromJson(string json) => JsonConvert.DeserializeObject<BatteryPredictionResult>(json, Converter.Settings);
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
