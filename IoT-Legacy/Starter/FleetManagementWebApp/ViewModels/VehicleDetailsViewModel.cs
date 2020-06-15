using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common.Models;

namespace FleetManagementWebApp.ViewModels
{
    public class VehicleDetailsViewModel
    {
        public Vehicle Vehicle { get; set; }
        public IEnumerable<Trip> Trips { get; set; }
    }
}
