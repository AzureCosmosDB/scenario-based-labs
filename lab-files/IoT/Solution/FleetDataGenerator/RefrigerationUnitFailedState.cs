using System;
using System.Collections.Generic;
using System.Text;

namespace FleetDataGenerator
{
    public static class RefrigerationUnitFailedState
    {
        public struct RefrigerationUnitKw
        {
            public static double StandardDeviation = 1.4;
            public static double SamplingRate = 10000;
            public static double Frequency = 70;
            public static double Amplitude = 1.8;
            public static double InitialValue = 15.0;
        }

        public struct RefrigerationUnitTemp
        {
            public static double StandardDeviation = 1.8;
            public static double InitialValue = 55.0;
        }
    }
}
