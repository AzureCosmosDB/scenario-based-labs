using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementWebApp.Models
{
    public class BatteryPredictionResult
    {
        public string result { get; set; }

        public double ParseResult()
        {
            var value = 0.0;

            if (result.Contains("["))
            {
                var stringValue = result.Substring(result.IndexOf("["), result.IndexOf("]"));
                double.TryParse(stringValue, out value);
            }

            return value;
        }
    }
}
