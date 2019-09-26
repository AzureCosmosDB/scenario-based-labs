using Contoso.Apps.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace Contoso.Apps.PaymentGateway
{
    /// <summary>
    /// Processor methods called from the API controller.
    /// </summary>
    public static class ProcessorMethods
    {
        /// <summary>
        /// Mock validator that checks the passed in credentials against hard-coded values.
        /// Not for production use!
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ValidateRequestCredentials(NVPCodec data)
        {
            bool validated = false;

            // Our hard-coded authentication values:
            string APIUsername = "ContosoUser";
            string APIPassword = "ContosoPassword";
            string APISignature = "ABCDEFGHIJKLMNOP1234567890";

            if (data[NVPProperties.Properties.USER] == APIUsername &&
                data[NVPProperties.Properties.PWD] == APIPassword &&
                data[NVPProperties.Properties.SIGNATURE] == APISignature)
            {
                validated = true;
            }

            return validated;
        }

        /// <summary>
        /// Authorize the requested payment amount (first step).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static bool AuthorizePayment(NVPCodec data, ref string token)
        {
            bool status = false;

            // TODO: Match up the token in the data parameter to the stored token from the
            // payment authorization step.

            // TODO: Write output to a log file.
            Debug.WriteLine("Authorizing payment amount: " + data[NVPProperties.Properties.PAYMENTREQUEST_AMT]);

            // Generate an Id for this transaction.
            token = new Randomizer().GetRandomString(16);
            status = true;

            Debug.WriteLine("Authorization token: " + token);

            return status;
        }

        /// <summary>
        /// Process the credit card payment (final step).
        /// </summary>
        /// <param name="data"></param>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        public static bool ProcessPayment(NVPCodec data, ref string transactionId)
        {
            bool status = false;

            // TODO: Match up the token in the data parameter to the stored token from the
            // payment authorization step.

            // TODO: Write output to a log file.
            Debug.WriteLine("Processing payment amount: " + data[NVPProperties.Properties.PAYMENTREQUEST_AMT]);

            // Generate an Id for this transaction.
            transactionId = new Randomizer().GetRandomString(12);
            status = true;

            Debug.WriteLine("Payment complete for authorization: " + data[NVPProperties.Properties.TOKEN]);
            Debug.WriteLine("Transaction Id: " + transactionId);

            return status;
        }
    }
}