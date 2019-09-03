using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;

namespace FleetDataGenerator
{
    public static class VehicleTelemetryGenerator
    {
        private static readonly Random Random = new Random();
        private const double HighSpeedProbabilityPower = 0.3;
        private const double LowSpeedProbabilityPower = 0.9;
        private const double HighOilProbabilityPower = 0.3;
        private const double LowOilProbabilityPower = 1.2;
        private const double HighTirePressureProbabilityPower = 0.5;
        private const double LowTirePressureProbabilityPower = 1.7;
        private const double HighOutsideTempProbabilityPower = 0.3;
        private const double LowOutsideTempProbabilityPower = 1.2;
        private const double HighEngineTempProbabilityPower = 0.3;
        private const double LowEngineTempProbabilityPower = 1.2;
        private static List<string> VinList = new List<string>();

        public static void Init()
        {
            VinList = GetVinMasterList().ToList();
        }

        public static VehicleEvent GenerateMessage(string vin, string state, double outsideTemperature)
        {
            //var state = GetLocation();
            return new VehicleEvent()
            {
                vin = vin,
                state = state,
                outsideTemperature = outsideTemperature,
                engineTemperature = GetEngineTemp(state),
                speed = GetSpeed(state),
                fuel = Random.Next(0, 40),
                fuelRate = GetEngineFuelRateValue(),
                engineoil = GetOil(state),
                tirepressure = GetTirePressure(state),
                //odometer = Random.Next(0, 200000),
                accelerator_pedal_position = Random.Next(0, 100),
                parking_brake_status = GetRandomBoolean(),
                headlamp_status = GetRandomBoolean(),
                brake_pedal_status = GetRandomBoolean(),
                transmission_gear_position = GetGearPos(),
                ignition_status = GetRandomBoolean(),
                windshield_wiper_status = GetRandomBoolean(),
                abs = GetRandomBoolean(),
                timestamp = DateTime.UtcNow
            };
        }

        private static void OnAsyncMethodFailed(Task task)
        {
            System.Console.WriteLine(task.Exception?.ToString() ?? "null error");
        }

        private static int GetSpeed(string state)
        {
            string[] statesWithHigherSpeed =
            {
                "AZ", "CA", "DE", "FL", "IL", "IN", "MN", "MT", "NM",
                "NV", "NY", "PA", "RI", "SC", "SD", "TX", "VA", "VT",
                "WA", "WI"
            };
            if (statesWithHigherSpeed.Contains(state.ToUpper()))
            {
                return GetRandomWeightedNumber(100, 0, HighSpeedProbabilityPower);
            }
            return GetRandomWeightedNumber(100, 0, LowSpeedProbabilityPower);
        }

        private static int GetOil(string state)
        {
            string[] statesWithLowerOilPressure =
            {
                "AK", "AR", "AZ", "CO", "CT", "DE", "HI", "KS", "KY",
                "LA", "MA", "MD", "MO", "NC", "ND", "NM", "OH", "PA",
                "SC", "TN", "UT", "VA", "VT", "WA", "WI", "WV", "WY"
            };
            if (statesWithLowerOilPressure.Contains(state.ToUpper()))
            {
                return GetRandomWeightedNumber(50, 0, LowOilProbabilityPower);
            }
            return GetRandomWeightedNumber(50, 0, HighOilProbabilityPower);
        }

        private static int GetTirePressure(string state)
        {
            string[] statesWithLowerTirePressure =
            {
                "AL", "AZ", "CA", "CT", "DE", "FL", "IA", "IN", "KY",
                "MA", "ME", "MN", "RI", "SD", "TX", "UT", "VA", "WA"
            };
            if (statesWithLowerTirePressure.Contains(state.ToUpper()))
            {
                return GetRandomWeightedNumber(50, 0, LowTirePressureProbabilityPower);
            }
            return GetRandomWeightedNumber(50, 0, HighTirePressureProbabilityPower);
        }

        private static int GetEngineTemp(string state)
        {
            string[] statesWithHigherEngineTemp =
            {
                "AL", "AR", "AZ", "CA", "DE", "FL", "IA", "IL", "IN",
                "KS", "LA", "MO", "MS", "MT", "NJ", "NM", "NV", "NY",
                "OK", "OR", "TX", "VA", "VT", "WA", "WY"
            };
            if (statesWithHigherEngineTemp.Contains(state.ToUpper()))
            {
                return GetRandomWeightedNumber(500, 0, HighEngineTempProbabilityPower);
            }
            return GetRandomWeightedNumber(500, 0, LowEngineTempProbabilityPower);
        }

        public static int GetOutsideTemp(string state)
        {
            string[] statesWithHigherOutsideTemp =
            {
                "AL", "AZ", "CA", "FL", "GA", "LA", "NM", "NV", "TX"
            };
            if (statesWithHigherOutsideTemp.Contains(state.ToUpper()))
            {
                return GetRandomWeightedNumber(110, 40, HighOutsideTempProbabilityPower);
            }
            return GetRandomWeightedNumber(90, 0, LowOutsideTempProbabilityPower);
        }

        private static int GetRandomWeightedNumber(int max, int min, double probabilityPower)
        {
            var randomDouble = Random.NextDouble();

            var result = Math.Floor(min + (max + 1 - min) * (Math.Pow(randomDouble, probabilityPower)));
            return (int)result;
        }

        private static string GetGearPos()
        {
            var list = new List<string>() { "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eight" };
            var l = list.Count;
            var num = Random.Next(l);
            return list[num];
        }

        private static double GetEngineFuelRateValue()
        {
            var rate = Random.NextDouble() * 6 + 8;
            return rate;
        }

        public static bool GetRandomBoolean()
        {
            return Random.Next(100) % 2 == 0;
        }

        public static IEnumerable<string> GetVinMasterList(int maxVINs = 1000)
        {
            var vins = new List<string>();
            using (var reader = new StreamReader(File.OpenRead(@"VINMasterList.csv")))
            {
                while (!reader.EndOfStream && vins.Count < maxVINs)
                {
                    var line = reader.ReadLine();
                    if (line == null) continue;
                    var values = line.Split(';');

                    vins.Add(values[0]);
                }
            }

            return vins;
        }

        public static string GetRandomVin()
        {
            var randomIndex = Random.Next(1, VinList.Count - 1);
            return VinList[randomIndex];
        }

        public static string GetLocation()
        {
            string[] states =
            {
                "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "IA", "ID", "IL", "IN", "KS", "KY",
                "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM", "NV", "NY",
                "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"
            };
            var num = Random.Next(50);
            return states[num];
        }
    }
}
