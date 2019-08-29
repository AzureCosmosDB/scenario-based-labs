using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbIoTScenario.Common
{
    public static class WellKnown
    {
        public struct Regions
        {
            public static string Southwest = "Southwest";
            public static string Central = "Central";
            public static string Southeast = "Southeast";
        }

        /// <summary>
        /// Status used for packages and consignments.
        /// </summary>
        public struct Status
        {
            public static string Pending = "Pending";
            public static string Active = "Active";
            public static string Delayed = "Delayed";
            public static string Canceled = "Canceled";
            public static string Completed = "Completed";
        }
    }
}
