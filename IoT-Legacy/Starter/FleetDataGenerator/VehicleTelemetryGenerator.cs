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
    public class VehicleTelemetryGenerator
    {
        private readonly Random _random = new Random();
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
        private readonly string _state;
        private readonly string _vin;
        private static List<string> VinList = new List<string>();

        public VehicleTelemetryGenerator(string vin)
        {
            _vin = vin;
        }

        public VehicleEvent GenerateMessage(string state, double outsideTemperature)
        {
            //var state = GetLocation();
            return new VehicleEvent()
            {
                vin = _vin,
                state = state,
                outsideTemperature = outsideTemperature,
                engineTemperature = GetEngineTemp(state),
                speed = GetSpeed(state),
                fuel = _random.Next(0, 40),
                fuelRate = GetEngineFuelRateValue(),
                engineoil = GetOil(state),
                tirepressure = GetTirePressure(state),
                accelerator_pedal_position = _random.Next(0, 100),
                parking_brake_status = GetRandomBoolean(),
                headlamp_status = GetRandomBoolean(),
                brake_pedal_status = GetRandomBoolean(),
                transmission_gear_position = _random.Next(1, 8),
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

        private int GetSpeed(string state)
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

        private int GetOil(string state)
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

        private int GetTirePressure(string state)
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

        private int GetEngineTemp(string state)
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

        public int GetOutsideTemp(string state)
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

        private int GetRandomWeightedNumber(int max, int min, double probabilityPower)
        {
            var randomDouble = _random.NextDouble();

            var result = Math.Floor(min + (max + 1 - min) * (Math.Pow(randomDouble, probabilityPower)));
            return (int)result;
        }

        private double GetEngineFuelRateValue()
        {
            var rate = Math.Round(_random.NextDouble() * 6 + 8, 2);
            return rate;
        }

        public bool GetRandomBoolean()
        {
            return _random.Next(100) % 2 == 0;
        }

        public string GetRandomVin()
        {
            var randomIndex = _random.Next(1, VinList.Count - 1);
            return VinList[randomIndex];
        }

    }
}
