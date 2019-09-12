using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace FleetDataGenerator
{
    class DeviceManager
    {
        static string connectionString;
        static RegistryManager registryManager;

        public static string HostName { get; set; }

        public static void IotHubConnect(string cnString)
        {
            connectionString = cnString;

            // Create an instance of the RegistryManager from the IoT Hub connection string.
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            var builder = IotHubConnectionStringBuilder.Create(cnString);

            HostName = builder.HostName;
        }

        /// <summary>
        /// Register a single device with IoT Hub.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task<string> RegisterDevicesAsync(string connectionString, string deviceId)
        {
            //Make sure we're connected
            if (registryManager == null)
                IotHubConnect(connectionString);

            // Create a new device.
            var device = new Device(deviceId) {Status = DeviceStatus.Enabled};

            try
            {
                // Register the new device.
                device = await registryManager.AddDeviceAsync(device);
            }
            catch (Exception ex)
            {
                if (ex is DeviceAlreadyExistsException ||
                    ex.Message.Contains("DeviceAlreadyExists"))
                {
                    // Device already exists, get the registered device.
                    device = await registryManager.GetDeviceAsync(deviceId);

                    // Ensure the device is activated.
                    device.Status = DeviceStatus.Enabled;

                    // Update IoT Hub with the device status change.
                    await registryManager.UpdateDeviceAsync(device);
                }
                else
                {
                    Program.WriteLineInColor($"An error occurred while registering IoT device '{deviceId}':\r\n{ex.Message}", ConsoleColor.Red);
                }
            }

            // Return the device key.
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        /// <summary>
        /// Un-register a single device from the IoT Hub Registry.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static async Task UnregisterDevicesAsync(string connectionString, string deviceId)
        {
            // Make sure we're connected.
            if (registryManager == null)
                IotHubConnect(connectionString);

            // Remove the device from the Registry.
            await registryManager.RemoveDeviceAsync(deviceId);
        }

        /// <summary>
        /// Un-register all the devices managed by the Fleet Data Generator.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static async Task UnregisterAllDevicesAsync(string connectionString)
        {
            // Make sure we're connected.
            if (registryManager == null)
                IotHubConnect(connectionString);

            for (var i = 0; i <= 9; i++)
            {
                var deviceId = "Device" + i.ToString();

                // Remove the device from the Registry.
                await registryManager.RemoveDeviceAsync(deviceId);
            }
        }
    }
}
