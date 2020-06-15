using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;

namespace FleetDataGenerator
{
    /// <summary>
    /// Represents telemetry from the vehicle's refrigeration unit.
    /// </summary>
    public class RefrigerationUnitTelemetry
    {
        public IEnumerable<double> RefrigerationUnitKw { get; set; }
        public IEnumerable<double> RefrigerationUnitTemp { get; set; }

        public RefrigerationUnitTelemetry() { }

        public RefrigerationUnitTelemetry(IEnumerable<double> refrigerationUnitKw,
            IEnumerable<double> refrigerationUnitTemp)
        {
            var transformed = TransformValues(refrigerationUnitKw, refrigerationUnitTemp);
            RefrigerationUnitKw = transformed.RefrigerationUnitKw;
            RefrigerationUnitTemp = transformed.RefrigerationUnitTemp;
        }

        /// <summary>
        /// Returns a new collection of RefrigerationUnitTelemetryItems from the data stored in this object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RefrigerationUnitTelemetryItem> ToRefrigerationUnitTelemetryItems()
        {
            var numItems = RefrigerationUnitKw.Count();
            var telemetryItems = new List<RefrigerationUnitTelemetryItem>(numItems);
            for (var i = 0; i < numItems; i++)
            {
                telemetryItems.Add(new RefrigerationUnitTelemetryItem(RefrigerationUnitKw.ElementAt(i), RefrigerationUnitTemp.ElementAt(i)));
            }

            return telemetryItems;
        }

        public void GraduallyDeteriorateNormalToFailed(IEnumerable<double> refrigerationUnitKwNormal,
            IEnumerable<double> refrigerationUnitTempNormal,
            IEnumerable<double> refrigerationUnitKwFailed,
            IEnumerable<double> refrigerationUnitTempFailed,
            int failOverXIterations = 250)
        {
            var normal = TransformValues(refrigerationUnitKwNormal, refrigerationUnitTempNormal);
            var failed = TransformValues(refrigerationUnitKwFailed, refrigerationUnitTempFailed);

            if (failOverXIterations > 0)
            {
                var rnd = new Random();
                // Percentage of wobble during degradation when randomizing the next value.
                const double wobblePercentage = 0.02;

                // Retrieve the last normal entry:
                var refrigerationUnitKwLastNormal = normal.RefrigerationUnitKw.Last();
                var refrigerationUnitTempLastNormal = normal.RefrigerationUnitTemp.Last();

                // Retrieve the first failed entry:
                var refrigerationUnitKwFirstFailed = failed.RefrigerationUnitKw.First();
                var refrigerationUnitTempFirstFailed = failed.RefrigerationUnitTemp.First();

                // How much to subtract from original normal last value in each fail iteration?
                var refrigerationUnitKwSubtractBy = (refrigerationUnitKwLastNormal - refrigerationUnitKwFirstFailed) / failOverXIterations;
                var refrigerationUnitTempSubtractBy =
                    (refrigerationUnitTempLastNormal - refrigerationUnitTempFirstFailed) / failOverXIterations;

                // Keep track of the last value during loop:
                var lastRefrigerationUnitKwValue = refrigerationUnitKwLastNormal;
                var lastRefrigerationUnitTempValue = refrigerationUnitTempLastNormal;

                // Collection of gradiated values for gradual failure:
                var refrigerationUnitKwGradualFailure = new List<double>();
                var refrigerationUnitTempGradualFailure = new List<double>();

                for (var i = 0; i < failOverXIterations; i++)
                {
                    // Subtract values for this step:
                    lastRefrigerationUnitKwValue -= refrigerationUnitKwSubtractBy;
                    lastRefrigerationUnitTempValue -= refrigerationUnitTempSubtractBy;

                    // Add subtracted value to gradual failure collections, or the first failed value, whichever is greater:
                    refrigerationUnitKwGradualFailure.Add(rnd.Next(
                        Convert.ToInt32(lastRefrigerationUnitKwValue) -
                        Convert.ToInt32(lastRefrigerationUnitKwValue * wobblePercentage),
                        Convert.ToInt32(lastRefrigerationUnitKwValue) +
                        Convert.ToInt32(lastRefrigerationUnitKwValue * wobblePercentage))
                    );
                    refrigerationUnitTempGradualFailure.Add(rnd.Next(
                        Convert.ToInt32(lastRefrigerationUnitTempValue) -
                        Convert.ToInt32(lastRefrigerationUnitTempValue * wobblePercentage),
                        Convert.ToInt32(lastRefrigerationUnitTempValue) +
                        Convert.ToInt32(lastRefrigerationUnitTempValue * wobblePercentage))
                    );
                }

                // Concatenate the normal, gradual failure, and failure items and save to the class properties.
                RefrigerationUnitKw = normal.RefrigerationUnitKw.Concat(refrigerationUnitKwGradualFailure).Concat(failed.RefrigerationUnitKw);
                RefrigerationUnitTemp = normal.RefrigerationUnitTemp.Concat(refrigerationUnitTempGradualFailure)
                    .Concat(failed.RefrigerationUnitTemp);
            }
            else
            {
                // Concatenate the normal and failure items for immediate failure, and save to the class properties.
                RefrigerationUnitKw = normal.RefrigerationUnitKw.Concat(failed.RefrigerationUnitKw);
                RefrigerationUnitTemp = normal.RefrigerationUnitTemp.Concat(failed.RefrigerationUnitTemp);
            }
        }

        /// <summary>
        /// Converts the passed in values to their simplified counterparts and transforms the data.
        /// </summary>
        /// <param name="refrigerationUnitKw"></param>
        /// <param name="refrigerationUnitTemp"></param>
        /// <returns>The converted and transformed data.</returns>
        protected (IEnumerable<double> RefrigerationUnitKw,
            IEnumerable<double> RefrigerationUnitTemp) TransformValues(IEnumerable<double> refrigerationUnitKw,
                IEnumerable<double> refrigerationUnitTemp)
        {
            var refrigerationUnitKwTransformed = refrigerationUnitKw.Select(x => Math.Round(x, 2));
            var refrigerationUnitTempTransformed = refrigerationUnitTemp.Select(x => Math.Round(x, 2));

            return (refrigerationUnitKwTransformed, refrigerationUnitTempTransformed);
        }
    }
}
