using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common.Models;

namespace FleetManagementWebApp.ViewModels
{
    public class ConsignmentIndexViewModel
    {
        public string Search { get; set; }
        public IEnumerable<Consignment> Consignments { get; set; }
    }
}
