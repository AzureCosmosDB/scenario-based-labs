using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;
using FleetManagementWebApp.Helpers;
using FleetManagementWebApp.Models;
using FleetManagementWebApp.Services;
using FleetManagementWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FleetManagementWebApp.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly Random _random = new Random();

        public VehiclesController(ICosmosDbService cosmosDbService, IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _cosmosDbService = cosmosDbService;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            if (search == null) search = "";
            //var query = new QueryDefinition("SELECT TOP @limit * FROM c WHERE c.entityType = @entityType")
            //    .WithParameter("@limit", 100)
            //    .WithParameter("@entityType", WellKnown.EntityTypes.Vehicle);
            // await _cosmosDbService.GetItemsAsync<Vehicle>(query);

            var vm = new VehicleIndexViewModel
            {
                Search = search,
                Vehicles = await _cosmosDbService.GetItemsWithPagingAsync<Vehicle>(
                    x => x.entityType == WellKnown.EntityTypes.Vehicle &&
                         (string.IsNullOrWhiteSpace(search) ||
                         (x.vin.ToLower().Contains(search.ToLower()) || x.stateVehicleRegistered.ToLower() == search.ToLower())), page, 10)
            };

            return View(vm);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            var vm = new VehicleCreateViewModel
            {
                lastServiceDate = DateTime.UtcNow.AddDays(-7),
                batteryRatedCycles = 200,
                StatesList = WellKnown.StatesList
            };

            return View(vm);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("vin,lastServiceDate,batteryAgeDays,batteryRatedCycles,lifetimeBatteryCyclesUsed,stateVehicleRegistered")] VehicleCreateViewModel item)
        {
            if (ModelState.IsValid)
            {
                var newVehicle = new Vehicle
                {
                    id = Guid.NewGuid().ToString(),
                    vin = item.vin,
                    lastServiceDate = item.lastServiceDate,
                    batteryAgeDays = item.batteryAgeDays,
                    batteryRatedCycles = item.batteryRatedCycles,
                    lifetimeBatteryCyclesUsed = item.lifetimeBatteryCyclesUsed,
                    stateVehicleRegistered = item.stateVehicleRegistered
                };

                // Add the new vehicle to the metadata Cosmos DB container:
                await _cosmosDbService.AddItemAsync(newVehicle, newVehicle.partitionKey);

                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("id,partitionKey,vin,lastServiceDate,batteryAgeDays,batteryRatedCycles,lifetimeBatteryCyclesUsed,stateVehicleRegistered")] VehicleEditViewModel item)
        {
            if (ModelState.IsValid)
            {
                var existingVehicle = await _cosmosDbService.GetItemAsync<Vehicle>(item.id, item.partitionKey);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                existingVehicle.lastServiceDate = item.lastServiceDate;
                existingVehicle.batteryAgeDays = item.batteryAgeDays;
                existingVehicle.batteryRatedCycles = item.batteryRatedCycles;
                existingVehicle.lifetimeBatteryCyclesUsed = item.lifetimeBatteryCyclesUsed;
                existingVehicle.stateVehicleRegistered = item.stateVehicleRegistered;

                await _cosmosDbService.UpdateItemAsync(existingVehicle, existingVehicle.partitionKey);
                return RedirectToAction("Index");
            }

            // Invalid. Re-populate states list and send back to the edit form.
            item.StatesList = WellKnown.StatesList;
            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id, string pk)
        {
            if (id == null || pk == null)
            {
                return BadRequest();
            }

            var item = await _cosmosDbService.GetItemAsync<Vehicle>(id, pk);
            if (item == null)
            {
                return NotFound();
            }

            var vm = new VehicleEditViewModel
            {
                id = item.id,
                partitionKey = item.partitionKey,
                vin = item.vin,
                lastServiceDate = item.lastServiceDate,
                batteryAgeDays = item.batteryAgeDays,
                batteryRatedCycles = item.batteryRatedCycles,
                lifetimeBatteryCyclesUsed = item.lifetimeBatteryCyclesUsed,
                stateVehicleRegistered = item.stateVehicleRegistered,
                StatesList = WellKnown.StatesList
            };

            return View(vm);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id, string pk)
        {
            pk = pk.ToUpper();

            if (id == null || pk == null)
            {
                return BadRequest();
            }

            var item = await _cosmosDbService.GetItemAsync<Vehicle>(id, pk);
            if (item == null)
            {
                return NotFound();
            }

            var vm = new VehicleDetailsViewModel
            {
                Vehicle = item,
                Trips = await _cosmosDbService.GetItemsAsync<Trip>(x => x.entityType == WellKnown.EntityTypes.Trip,
                    partitionKey: pk)
            };

            return View(vm);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("id,partitionKey")] VehicleEditViewModel item)
        {
            // We're just deleting the vehicle and not any related trips.
            // In a real-life scenario, we'd probably just soft-delete the vehicle and any related records,
            // or remove the vehicle's VIN from related trips.
            await _cosmosDbService.DeleteItemAsync<Vehicle>(item.id, item.partitionKey);

            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id, string pk)
        {
            pk = pk.ToUpper();
            var vm = new VehicleDetailsViewModel
            {
                Vehicle = await _cosmosDbService.GetItemAsync<Vehicle>(id, pk),
                Trips = await _cosmosDbService.GetItemsAsync<Trip>(x => x.entityType == WellKnown.EntityTypes.Trip,
                    partitionKey: pk)
            };

            return View(vm);
        }

        [ActionName("DetailsByVIN")]
        public async Task<ActionResult> DetailsByVINAsync(string vin)
        {
            vin = vin.ToUpper();

            // Since we only have the VIN and not the vehicle ID, we need to conduct a search.
            // There should only be one result where the partition key is VIN and the entity typ is 'Vehicle'.
            // We will grab the first result anyway for the view model.
            var vehicles = await _cosmosDbService.GetItemsAsync<Vehicle>(x =>
                x.entityType == WellKnown.EntityTypes.Vehicle &&
                x.vin == vin, partitionKey: vin);

            var vm = new VehicleDetailsViewModel
            {
                Vehicle = vehicles.FirstOrDefault(),
                Trips = await _cosmosDbService.GetItemsAsync<Trip>(x => x.entityType == WellKnown.EntityTypes.Trip,
                    partitionKey: vin)
            };

            return View("Details", vm);
        }

        [ActionName("BatteryPrediction")]
        public async Task<ActionResult> BatteryPredictionAsync(int batteryAgeDays, double batteryRatedCycles, double lifetimeBatteryCyclesUsed, double dailyTripDuration)
        {
            var payload = new BatteryPredictionPayload(batteryAgeDays, dailyTripDuration);

            var httpClient = _clientFactory.CreateClient(NamedHttpClients.ScoringService);

            // Create the payload to send to the Logic App.
            var postBody = JsonConvert.SerializeObject(payload);

            var httpResponse = await httpClient.PostAsync(_configuration["ScoringUrl"],
                new StringContent(postBody, Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();

            var result = BatteryPredictionResult.FromJson(await httpResponse.Content.ReadAsAsync<string>());

            // The results return in an array of doubles. We only expect a single result for this prediction.
            var predictedDailyCyclesUsed = result.Result[0];

            // Multiply the predictedCyclesConsumed * 30 (days), add that value to the lifetime cycles used, then see if it exceeds the battery's rated cycles.
            var predictedToFail = predictedDailyCyclesUsed * 30 + lifetimeBatteryCyclesUsed > batteryRatedCycles;

            return Json(predictedToFail);
        }
    }
}