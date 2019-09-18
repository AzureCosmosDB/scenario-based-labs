using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common.Models;

namespace FleetManagementWebApp.ViewModels
{
    public class TripIndexViewModel
    {
        public string Status { get; set; }
        public IEnumerable<Trip> Trips { get; set; }
        public IEnumerable<string> StatusList { get; set; }
    }
}
