using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common.Models;

namespace FleetManagementWebApp.ViewModels
{
    public class VehicleIndexViewModel
    {
        public string Search { get; set; }
        public IEnumerable<Vehicle> Vehicles { get; set; }
    }
}
