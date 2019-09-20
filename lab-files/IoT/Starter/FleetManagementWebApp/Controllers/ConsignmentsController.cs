using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;
using FleetManagementWebApp.Services;
using FleetManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementWebApp.Controllers
{
    public class ConsignmentsController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;

        public ConsignmentsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            if (search == null) search = "";

            var vm = new ConsignmentIndexViewModel
            {
                Search = search,
                Consignments = await _cosmosDbService.GetItemsWithPagingAsync<Consignment>(
                    x => x.entityType == WellKnown.EntityTypes.Consignment &&
                         (string.IsNullOrWhiteSpace(search) ||
                         (x.customer.ToLower().Contains(search.ToLower()) || x.id.ToLower() == search.ToLower())), page, 10)
            };

            return View(vm);
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            // Consignments are partitioned by their ID.
            var consignment = await _cosmosDbService.GetItemAsync<Consignment>(id, id);
            var vm = new ConsignmentDetailsViewModel
            {
                Consignment = consignment,
                Packages = await _cosmosDbService.GetItemsAsync<Package>(x => x.entityType == WellKnown.EntityTypes.Package &&
                    consignment.packages.Contains(x.id),
                    partitionKey: consignment.id)
            };
            return View(vm);
        }
    }
}