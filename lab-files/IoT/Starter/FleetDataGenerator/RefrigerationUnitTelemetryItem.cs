using System;
using System.Collections.Generic;
using System.Text;

namespace FleetDataGenerator
{
    public class RefrigerationUnitTelemetryItem
    {
        public double RefrigerationUnitKw { get; set; }
        public double RefrigerationUnitTemp { get; set; }

        public RefrigerationUnitTelemetryItem(double refrigerationUnitkW, double refrigerationUnitTemp)
        {
            RefrigerationUnitKw = refrigerationUnitkW;
            RefrigerationUnitTemp = refrigerationUnitTemp;
        }
    }
}
