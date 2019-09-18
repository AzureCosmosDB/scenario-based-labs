using System;
using System.Collections.Generic;
using System.Text;

namespace FleetDataGenerator
{
    public static class RefrigerationUnitNormalState
    {
        public struct RefrigerationUnitKw
        {
            public static double StandardDeviation = 1.0;
            public static double SamplingRate = 10000;
            public static double Frequency = 85;
            public static double Amplitude = 2.4;
            public static double InitialValue = 70.0;
        }

        public struct RefrigerationUnitTemp
        {
            public static double StandardDeviation = 0.8;
            public static double InitialValue = 20.0;
        }
    }
}
