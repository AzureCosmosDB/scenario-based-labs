using System;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace FleetDataGenerator
{
    internal class Program
    {
        private static IConfigurationRoot _configuration;

        private static readonly object LockObject = new object();
        // AutoResetEvent to signal when to exit the application.
        private static readonly AutoResetEvent WaitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            
        }

        /// <summary>
        /// Extracts properties from either the appsettings.json file or system environment variables.
        /// </summary>
        /// <returns>
        /// EventHubConnectionString: The primary Event Hubs connection string for sending telemetry.
        /// CosmosDbConnectionString: The primary or secondary connection string copied from your Cosmos DB properties.
        /// MillisecondsToRun: The maximum amount of time to allow the generator to run before stopping transmission of data. The default value is 600. Data will also stop transmitting after the included Untagged_Transactions.csv file's data has been sent.
        /// MillisecondsToLead: The amount of time to wait before sending payment transaction data. Default value is 0.
        /// </returns>
        private static (string EventHubConnectionString, 
            string CosmosDbConnectionString,
            int MillisecondsToRun,
            int MillisecondsToLead) ParseArguments()
        {
            try
            {
                // The Configuration object will extract values either from the machine's environment variables, or the appsettings.json file.
                var eventHubConnectionString = _configuration["EVENT_HUB_CONNECTION_STRING"];
                var cosmosDbEndpointUrl = _configuration["COSMOS_DB_CONNECTION_STRING"];
                var numberOfMillisecondsToRun = (int.TryParse(_configuration["SECONDS_TO_RUN"], out var outputSecondToRun) ? outputSecondToRun : 0) * 1000;
                var numberOfMillisecondsToLead = (int.TryParse(_configuration["SECONDS_TO_LEAD"], out var outputSecondsToLead) ? outputSecondsToLead : 0) * 1000;

                if (string.IsNullOrWhiteSpace(cosmosDbEndpointUrl))
                {
                    throw new ArgumentException("COSMOS_DB_CONNECTION_STRING must be provided");
                }

                return (eventHubConnectionString, cosmosDbEndpointUrl, numberOfMillisecondsToRun, numberOfMillisecondsToLead);
            }
            catch (Exception e)
            {
                WriteLineInColor(e.Message, ConsoleColor.Red);
                Console.ReadLine();
                throw;
            }
        }

        public static void WriteLineInColor(string msg, ConsoleColor color)
        {
            lock (LockObject)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
        }
    }
}
