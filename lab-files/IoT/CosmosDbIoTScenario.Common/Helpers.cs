using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CosmosDbIoTScenario.Common
{
    public static class Helpers
    {
        /// <summary>
        /// Calculates distance traveled in miles, based on velocity (mph) and
        /// time (milliseconds).
        /// </summary>
        /// <param name="velocity">Velocity measured in miles per hour.</param>
        /// <param name="time">Elapsed time measured in milliseconds.</param>
        /// <returns></returns>
        public static double DistanceTraveled(int velocity, long time)
        {
            return velocity == 0 || time == 0 ? 0 : ((double)velocity / 3600000) * time;
        }
    }
}
