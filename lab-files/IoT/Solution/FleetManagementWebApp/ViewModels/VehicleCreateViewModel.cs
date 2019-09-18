using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace FleetManagementWebApp.ViewModels
{
    public class VehicleCreateViewModel
    {
        [Required]
        [StringLength(17, MinimumLength = 16)]
        [Display(Name = "VIN")]
        public string vin { get; set; }
        [Display(Name = "Last Service Date")]
        public DateTime lastServiceDate { get; set; }
        [Display(Name = "Battery Age (Days)")]
        public int batteryAgeDays { get; set; }
        /// <summary>
        /// Typical battery cycle rating is 200.
        /// </summary>
        [Display(Name = "Battery Rated Cycles")]
        public double batteryRatedCycles { get; set; }
        [Display(Name = "Lifetime Cycles")]
        public double lifetimeBatteryCyclesUsed { get; set; }
        [Required]
        [Display(Name = "State Registered")]
        public string stateVehicleRegistered { get; set; }

        public List<string> StatesList { get; set; }

    }
}
