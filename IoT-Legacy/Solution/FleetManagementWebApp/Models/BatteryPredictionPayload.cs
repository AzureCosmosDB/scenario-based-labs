using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementWebApp.Models
{
    public class BatteryPredictionPayload
    {

        public List<BatteryPredictionPayloadItem> data { get; set; }

        public BatteryPredictionPayload(int batteryAgeDays, double dailyTripDuration)
        {
            data = new List<BatteryPredictionPayloadItem>
            {
                new BatteryPredictionPayloadItem
                {
                    Battery_Age_Days = batteryAgeDays, Daily_Trip_Duration = dailyTripDuration
                }
            };
        }

        public class BatteryPredictionPayloadItem
        {
            public DateTime Date => DateTime.UtcNow;
            public int Battery_ID => 0;
            public int Battery_Age_Days { get; set; }
            public double Daily_Trip_Duration { get; set; }
        }
    }
}
