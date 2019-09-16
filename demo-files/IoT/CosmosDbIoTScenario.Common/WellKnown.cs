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
            /// Inactive trips are queued for future vehicle assignments.
            /// </summary>
            public static string Inactive = "Inactive";
            /// <summary>
            /// Pending trips are assigned to a vehicle and will become active when the vehicle starts its trip.
            /// </summary>
            public static string Pending = "Pending";
            /// <summary>
            /// When a trip starts, it becomes active. A vehicle can only be assigned one active trip at a time.
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

        public static List<string> StatesList = new List<string>
        {
            "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "IA", "ID", "IL", "IN", "KS", "KY",
            "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM", "NV", "NY",
            "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"
        };
    }
}
