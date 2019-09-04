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

        public struct EntityTypes
        {
            public static string Vehicle = "Vehicle";
            public static string VehicleTelemetry = "VehicleTelemetry";
            public static string Consignment = "Consignment";
            public static string Package = "Package";
            public static string Trip = "Trip";
        }

        /// <summary>
        /// Status used for packages and consignments.
        /// </summary>
        public struct Status
        {
            /// <summary>
            /// Pending trips are queued for future vehicle assignments.
            /// </summary>
            public static string Pending = "Pending";
            /// <summary>
            /// Active trips are assigned to a vehicle. A vehicle can only be assigned one active trip at a time.
            /// </summary>
            public static string Active = "Active";
            /// <summary>
            /// Delayed means that the trip or consignment is, or has the potential to be, beyond the desired due date/time.
            /// </summary>
            public static string Delayed = "Delayed";
            /// <summary>
            /// The trip or consignment has been withdrawn.
            /// </summary>
            public static string Canceled = "Canceled";
            /// <summary>
            /// The trip or consignment is successfully completed.
            /// </summary>
            public static string Completed = "Completed";
        }
    }
}
