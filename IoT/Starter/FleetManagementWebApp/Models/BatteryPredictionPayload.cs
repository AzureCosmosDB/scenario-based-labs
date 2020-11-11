using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementWebApp.Models
{
    public class BatteryPredictionPayload
    {

        public List<BatteryPredictionPayloadItem> data { get; set; }

        public BatteryPredictionPayload(string vin, int batteryAgeDays, double batteryRatedCycles, double lifetimeBatteryCyclesUsed, double dailyTripDuration)
        {
            data = new List<BatteryPredictionPayloadItem>
            {
                new BatteryPredictionPayloadItem
                {
                    vin = vin,
                    batteryAgeDays = batteryAgeDays,
                    batteryRatedCycles = batteryRatedCycles,
                    lifetimeBatteryCyclesUsed = lifetimeBatteryCyclesUsed,
                    tripDurationMinutes = dailyTripDuration
                }
            };
        }

        public class BatteryPredictionPayloadItem
        {
            public string vin { get; set; }
            public int batteryAgeDays { get; set; }
            public double batteryRatedCycles { get; set; }
            public double lifetimeBatteryCyclesUsed { get; set; }
            public double tripDurationMinutes { get; set; }
        }
    }
}
