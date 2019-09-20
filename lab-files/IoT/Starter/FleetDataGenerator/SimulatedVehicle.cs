using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CosmosDbIoTScenario.Common;
using CosmosDbIoTScenario.Common.Models;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace FleetDataGenerator
{
    public class SimulatedVehicle
    {
        // The amount of time to delay between sending telemetry.
        private readonly TimeSpan CycleTime = TimeSpan.FromMilliseconds(100);
        private DeviceClient _DeviceClient;
        private string _IotHubUri { get; set; }
        public string DeviceId { get; set; }
        public string DeviceKey { get; set; }
        private const string TelemetryEventHubName = "telemetry";
        private int _messagesSent = 0;
        private readonly int _vehicleNumber = 0;
        private readonly bool _causeRefrigerationUnitFailure = false;
        private readonly bool _immediateRefrigerationUnitFailure = false;
        private double _distanceRemaining = 0;
        private double _distanceTraveled = 0;
        private readonly Trip _trip;
        private readonly string _tripId;
        private readonly EventHubClient _eventHubClient;
        private readonly CancellationTokenSource _localCancellationSource = new CancellationTokenSource();

        public string TripId => _tripId;

        /// <summary>
        /// The total number of messages sent by this device to Event Hubs.
        /// </summary>
        public int MessagesSent => _messagesSent;

        public SimulatedVehicle(Trip trip, bool causeRefrigerationUnitFailure,
            bool immediateRefrigerationUnitFailure, int vehicleNumber,
            string iotHubUri, string deviceId, string deviceKey)
        {
            _vehicleNumber = vehicleNumber;
            _trip = trip;
            _tripId = trip.id;
            _distanceRemaining = trip.plannedTripDistance + 3; // Pad a little bit extra distance to ensure all events captured.
            _causeRefrigerationUnitFailure = causeRefrigerationUnitFailure;
            _immediateRefrigerationUnitFailure = immediateRefrigerationUnitFailure;
            _IotHubUri = iotHubUri;
            DeviceId = deviceId;
            DeviceKey = deviceKey;
            _DeviceClient = DeviceClient.Create(_IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
        }

        /// <summary>
        /// Creates an asynchronous task for sending all data for the vehicle.
        /// </summary>
        /// <returns>Task for asynchronous device operation</returns>
        public async Task RunVehicleSimulationAsync()
        {
            await SendDataToHub(_localCancellationSource.Token).ConfigureAwait(false);
        }

        public void CancelCurrentRun()
        {
            _localCancellationSource.Cancel();
        }

        /// <summary>
        /// Takes a set of RefrigerationUnitTelemetryItem data for a device in a dataset and sends the
        /// data to the message with a configurable delay between each message.
        /// </summary>
        /// <param name="RefrigerationUnitTelemetry">The set of data to send as messages to the IoT Central.</param>
        /// <returns></returns>
        private async Task SendDataToHub(CancellationToken cancellationToken)
        {
            // Generate simulated refrigeration unit data.
            const int sampleSize = 10000;
            const int failOverXIterations = 625;
            var vehicleTelemetryGenerator = new VehicleTelemetryGenerator(_trip.vin);
            var telemetryTimer = new Stopwatch();
            var refrigerationTelemetry = DataGenerator.GenerateRefrigerationUnitTelemetry(_trip.temperatureSetting, sampleSize,
                _causeRefrigerationUnitFailure, _immediateRefrigerationUnitFailure ? 0 : failOverXIterations).ToArray();

            var refrigerationTelemetryCount = refrigerationTelemetry.Length;
            var idx = 0;
            var outsideTemperature = vehicleTelemetryGenerator.GetOutsideTemp(_trip.location);

            telemetryTimer.Start();
            while (!_localCancellationSource.IsCancellationRequested && _distanceRemaining >= 0)
            {
                // Reset the refrigeration unit telemetry if we've run out of items. This will also reset failed events, if applicable.
                if (idx >= refrigerationTelemetryCount) idx = 0;
                var vehicleTelemetry = vehicleTelemetryGenerator.GenerateMessage(_trip.location, outsideTemperature);
                vehicleTelemetry.refrigerationUnitKw = refrigerationTelemetry[idx].RefrigerationUnitKw;
                vehicleTelemetry.refrigerationUnitTemp = refrigerationTelemetry[idx].RefrigerationUnitTemp;
                var distanceTraveled = Helpers.DistanceTraveled(vehicleTelemetry.speed, telemetryTimer.ElapsedMilliseconds);
                telemetryTimer.Restart();
                
                _distanceTraveled += distanceTraveled;
                _distanceRemaining -= distanceTraveled;
                vehicleTelemetry.odometer = Math.Round(_trip.odometerBegin + _distanceTraveled, 2);

                // Serialize data and send to Event Hubs:
                await SendEvent(JsonConvert.SerializeObject(vehicleTelemetry), cancellationToken).ConfigureAwait(false);

                idx++;

                await Task.Delay(CycleTime, cancellationToken).ConfigureAwait(false);
            }

            if (_distanceRemaining < 0)
            {
                Program.WriteLineInColor($"Vehicle {_vehicleNumber} has completed its trip.", ConsoleColor.Yellow);
            }

            telemetryTimer.Stop();
        }

        /// <summary>
        /// Uses the EventHubClient to send a message to Event Hubs.
        /// </summary>
        /// <param name="message">JSON string representing serialized telemetry data.</param>
        /// <returns>Task for async execution.</returns>
        private async Task SendEvent(string message, CancellationToken cancellationToken)
        {
            using (var eventData = new Message(Encoding.ASCII.GetBytes(message)))
            {
                
                // Send telemetry to IoT Hub. All messages are partitioned by the Device Id, guaranteeing message ordering.
                var sendEventAsync = _DeviceClient?.SendEventAsync(eventData, cancellationToken);
                if (sendEventAsync != null) await sendEventAsync.ConfigureAwait(false);

                // Keep track of messages sent and update progress periodically.
                var currCount = Interlocked.Increment(ref _messagesSent);
                if (currCount % 50 == 0)
                {
                    Console.WriteLine($"Vehicle {_vehicleNumber}: {_trip.vin} Message count: {currCount} -- {Math.Round(_distanceRemaining, 2)} miles remaining");
                }
            }
        }
    }
}
