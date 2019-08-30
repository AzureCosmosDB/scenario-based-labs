using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;

namespace FleetDataGenerator
{
    public static class DataGenerator
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates 1,000 vehicles.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Vehicle> GenerateVehicles()
        {
            var vehicles = new List<Vehicle>();
            const double defaultBatteryCycles = 200.0;
            var vins = VehicleTelemetryGenerator.GetVinMasterList();

            foreach (var vin in vins)
            {
                var batteryAgeDays = RandomIntegerInRange(20, 1600);
                var averageDailyBatteryCycles = RandomDoubleInRange(0.1852, 0.1056);
                vehicles.Add(new Vehicle
                {
                    vin = vin,
                    batteryRatedCycles = defaultBatteryCycles,
                    batteryAgeDays = batteryAgeDays,
                    lifetimeBatteryCyclesUsed = batteryAgeDays * averageDailyBatteryCycles,
                    lastServiceDate = RandomDateBeforeToday(400),
                    stateVehicleRegistered = VehicleTelemetryGenerator.GetLocation()
                });
            }

            return vehicles;
        }

        public static IEnumerable<Consignment> GenerateConsignments(int numberToGenerate = 1000)
        {
            var consignments = new List<Consignment>();

            for (var i = 0; i < numberToGenerate; i++)
            {
                consignments.Add(new Consignment
                {
                    consignmentId = Guid.NewGuid().ToString(),
                    customer = RandomCompanyName(),
                    status = WellKnown.Status.Pending,
                    deliveryDueDate = DateTime.UtcNow.AddMinutes(RandomIntegerInRange(240, -10))
                });
            }

            return consignments;
        }

        public static IEnumerable<Package> GeneratePackages(List<Consignment> addToConsignments)
        {
            var packages = new List<Package>();
            var averageStorageTemperature = 30.0;
            
            foreach (var consignment in addToConsignments)
            {
                var numberOfPackagesToAdd = RandomIntegerInRange(350, 25);

                for (var i = 0; i < numberOfPackagesToAdd; i++)
                {
                    packages.Add(new Package
                    {
                        packageId = Guid.NewGuid().ToString(),
                        consignmentId = consignment.consignmentId,
                        length = RandomDoubleInRange(56, 4),
                        height = RandomDoubleInRange(56, 4),
                        width = RandomDoubleInRange(56, 4),
                        grossWeight = RandomDoubleInRange(250, 1.5),
                        storageTemperature = RandomizeInitialValue(averageStorageTemperature, 0.3),
                        highValue = _random.Next(100) % 25 == 0,
                        consignment = new PackageConsignment
                        {
                            consignmentId = consignment.consignmentId,
                            customer = consignment.customer,
                            deliveryDueDate = consignment.deliveryDueDate
                        }
                });
                }
            }

            return packages;
        }

        public static IEnumerable<Trip> GenerateTrips(List<Package> packagesToAdd)
        {
            var trips = new List<Trip>();



            return trips;
        }

        public static double RandomDoubleInRange(double max, double min)
        {
            return _random.NextDouble() * (max - min) + min;
        }

        public static int RandomIntegerInRange(int max, int min)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Creates a random value in a range of += deviateUpDownPercentage (%) of the initial value.
        /// </summary>
        /// <param name="initialValue">The initial value you wish to randomize.</param>
        /// <returns></returns>
        private static double RandomizeInitialValue(double initialValue, double deviateUpDownPercentage = 0.02)
        {
            var upper = initialValue + (initialValue * deviateUpDownPercentage);
            var lower = initialValue - (initialValue * deviateUpDownPercentage);

            return _random.NextDouble() * (upper - lower) + lower;
        }

        public static DateTime RandomDateBeforeToday(int maxDaysAgo)
        {
            var randomDaysAgo = RandomIntegerInRange(maxDaysAgo, 1);
            // Subtract random days from current date and return.
            return DateTime.UtcNow.AddDays(-randomDaysAgo);
        }

        public static string RandomCompanyName()
        {
            var list = new List<string>() { "Adatum Corporation", "Adventure Works Cycles", "Alpine Ski House", "Bellows College",
                                            "Best For You Organics Company", "Contoso Pharmaceuticals", "Contoso Suites",
                                            "Consolidated Messenger", "Fabrikam, Inc.", "Fabrikam Residences", "Fincher Architects",
                                            "First Up Consultants", "Fourth Coffee", "Graphic Design Institute", "Humongous Insurance",
                                            "Lamna Healthcare Company", "Liberty's Delightful Sinful Bakery & Cafe", "Lucerne Publishing",
                                            "Margie's Travel", "Munson's Pickles and Preserves Farm", "Nod Publishers", "Northwind Traders",
                                            "Proseware, Inc.", "Relecloud", "School of Fine Art", "Southridge Video", "Tailspin Toys",
                                            "Trey Research", "The Phone Company", "VanArsdel, Ltd.", "Wide World Importers", "Wingtip Toys",
                                            "Woodgrove Bank" };
            var l = list.Count;
            var num = _random.Next(l);
            return list[num];
        }
    }
}
