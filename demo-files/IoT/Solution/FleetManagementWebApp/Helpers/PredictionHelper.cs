using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementWebApp.Helpers
{
    public static class PredictionHelper
    {
        public static double ParsePredictionResult(string result)
        {
            var value = 0.0;

            // The scoring service result contains invalid JSON. Parse the string result into a double.
            if (result.Contains("[") && result.Contains("]"))
            {
                var beginning = result.IndexOf("[");
                var end = result.IndexOf("]");
                var stringValue = result.Substring(beginning + 1, end - beginning - 1);
                double.TryParse(stringValue, out value);
            }

            return value;
        }
    }
}
