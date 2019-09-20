using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common.Models;

namespace FleetManagementWebApp.ViewModels
{
    public class ConsignmentDetailsViewModel
    {
        public Consignment Consignment { get; set; }
        public IEnumerable<Package> Packages { get; set; }

        /// <summary>
        /// Package statistics:
        ///
        /// TotalPackageVolume: Total volume in feet.
        /// TotalPackageWeight: Total gross weight of all packages.
        /// ContainsHighValue: True if any package is marked as a high value package.
        /// </summary>
        /// <returns></returns>
        public (double TotalPackageVolume, double TotalPackageWeight, bool ContainsHighValue) PackageStatistics()
        {
            var volume = 0.0;
            var weight = 0.0;
            var containsHighValue = false;

            if (Packages != null)
            {
                foreach (var package in Packages)
                {
                    if (package.highValue) containsHighValue = true;

                    volume += (package.height * package.width * package.length);
                    weight += package.grossWeight;
                }
            }

            // To convert from cubic inches to cubic feet, divide by 1728 (as there are 1728 cubic inches in a cubic foot).
            var volumeInCubicFeet = Math.Round(volume / 1728, 2);

            return (volumeInCubicFeet, Math.Round(weight, 2), containsHighValue);
        }
    }
}
