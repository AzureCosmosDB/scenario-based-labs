using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.Movies.Offers.Models
{
    public class OfferedProduct
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public string ThumbnailPath { get; set; }

        public double? UnitPrice { get; set; }

        /// <summary>
        /// Percentage off the unit price for calculating the sale price.
        /// </summary>
        public double SalePercentage { get; set; }

        /// <summary>
        /// Stores a generated sale price. We are not generating the random
        /// sales price within the getter due to potential performance
        /// problems as well as high likelihood of duplicate numbers caused
        /// by creating a new instance of the Random class within multiple
        /// subsequent calls.
        /// </summary>
        public double? SalePrice { get; set; }
    }
}