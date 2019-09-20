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
    public class TripsController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;

        public TripsController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<IActionResult> Index(int page = 1, string status = "")
        {
            if (status == null) status = "";

            var statusList = new List<string>
            {
                "",
                WellKnown.Status.Active,
                WellKnown.Status.Canceled,
                WellKnown.Status.Completed,
                WellKnown.Status.Delayed,
                WellKnown.Status.Pending
            };

            var vm = new TripIndexViewModel
            {
                Status = status,
                Trips = await _cosmosDbService.GetItemsWithPagingAsync<Trip>(
                    x => x.entityType == WellKnown.EntityTypes.Trip &&
                         (string.IsNullOrWhiteSpace(status) ||
                          (x.status.ToLower() == status.ToLower())), page, 10),
                StatusList = statusList
            };

            return View(vm);
        }
    }
}